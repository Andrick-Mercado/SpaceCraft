using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : GravityObject
{
	public static PlayerController Instance;
	
	// Exposed variables
	[Header ("Movement settings")]
	public float walkSpeed = 8;
	public float runSpeed = 14;
	public float jumpForce = 20;
	public float vSmoothTime = 0.1f;
	public float airSmoothTime = 0.5f;
	public float stickToGroundForce = 8;

	public float jetpackForce = 10;
	public float jetpackDuration = 2;
	public float jetpackRefuelTime = 2;
	public float jetpackRefuelDelay = 2;

	[Header ("Mouse settings")]
	public float mouseSensitivity = 10;
	public float rotationSmoothTime = 0.1f;
	public Vector2 pitchMinMax = new Vector2 (-40, 85);
	public bool lockCursor;

	[Header ("Other")]
	public float mass = 70;
	public LayerMask walkableMask;
	public Transform feet;
	
	[Header("Quests")] 
	public Quest quest;

	// Private
	Rigidbody rb;

	float yaw;
	float pitch;
	float smoothYaw;
	float smoothPitch;

	float yawSmoothV;
	float pitchSmoothV;

	Vector3 targetVelocity;
	Vector3 cameraLocalPos;
	Vector3 smoothVelocity;
	Vector3 smoothVRef;

	// Jetpack
	bool usingJetpack;
	float jetpackFuelPercent = 1;
	float lastJetpackUseTime;

	CelestialBody referenceBody;

	Camera cam;
	bool readyToFlyShip;
	bool debug_playerFrozen;
	Animator animator;
	
	//for multiplayer
	private PhotonView _view;
	private bool _paused;
	private bool _lockPlayerMovement;

	void Awake () {
		cam = GetComponentInChildren<Camera> ();
		cameraLocalPos = cam.transform.localPosition;
		InitRigidbody ();
		if (lockCursor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		animator = GetComponentInChildren<Animator> ();
		CelestialBody startBody = FindObjectOfType<GameSetUp>().startBody;
		Vector3 pointAbovePlanet = startBody.transform.position + Vector3.right * startBody.radius * 1.1f;
		transform.position = pointAbovePlanet;
		SetVelocity(startBody.initialVelocity);
		FindObjectOfType<EndlessManager>().physicsObjects.Add(transform);
		FindObjectOfType<EndlessManager>().playerCamera = GetComponentInChildren<Camera>();
		FindObjectOfType<LODHandler>().cam = cam;
		FindObjectOfType<LODHandler>().camT = cam.transform;

		//get photon view component
		_view = GetComponent<PhotonView>();
		_paused = false;
		_lockPlayerMovement = false;
	}

	private void Start()
	{
		if(_view)
		if (!_view.IsMine)
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
			return;
		}

		QuestGiver.Instance.OnLockPlayerMovementEvent += OnLockPlayerMovement;
		QuestGiver.Instance.OnUnlockPlayerMovementEvent += OnUnlockPlayerMovement;
		QuestGiver.Instance.OnGiveQuestEvent += OnSetQuest;
		MenuManager.Instance.TurnOnCrosshair();
	}

	void InitRigidbody () {
		rb = GetComponent<Rigidbody> ();
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		rb.useGravity = false;
		rb.isKinematic = false;
		rb.mass = mass;
	}

	void Update () 
	{
		if (_view)
			if (!_view.IsMine) return;//prevent other players from using this script
		
		HandleMovement ();
		PauseGame();
		
	}

	void HandleMovement () 
	{
		if (_view)
			//prevent other players from moving others or if game is paused
			if (!_view.IsMine || _paused) return;
		
		if (_lockPlayerMovement) return;
		
		HandleEditorInput ();
		// Look input
		yaw += Input.GetAxisRaw ("Mouse X") * mouseSensitivity;
		pitch -= Input.GetAxisRaw ("Mouse Y") * mouseSensitivity;
		pitch = Mathf.Clamp (pitch - Input.GetAxisRaw ("Mouse Y") * mouseSensitivity, pitchMinMax.x, pitchMinMax.y);
		smoothPitch = Mathf.SmoothDampAngle (smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
		float smoothYawOld = smoothYaw;
		smoothYaw = Mathf.SmoothDampAngle (smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);
		if (!debug_playerFrozen) {
			cam.transform.localEulerAngles = Vector3.right * smoothPitch;
			transform.Rotate (Vector3.up * Mathf.DeltaAngle (smoothYawOld, smoothYaw), Space.Self);
		}

		// Movement
		bool isGrounded = IsGrounded ();
		Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		bool running = Input.GetKey (KeyCode.LeftShift);
		targetVelocity = transform.TransformDirection (input.normalized) * ((running) ? runSpeed : walkSpeed);
		smoothVelocity = Vector3.SmoothDamp (smoothVelocity, targetVelocity, ref smoothVRef, (isGrounded) ? vSmoothTime : airSmoothTime);

		//bool inWater = referenceBody
		if (isGrounded) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				rb.AddForce (transform.up * jumpForce, ForceMode.VelocityChange);
				isGrounded = false;
			} else {
				// Apply small downward force to prevent player from bouncing when going down slopes
				rb.AddForce (-transform.up * stickToGroundForce, ForceMode.VelocityChange);
			}
		} else {
			// Press (and hold) spacebar while above ground to engage jetpack
			if (Input.GetKeyDown (KeyCode.Space)) {
				usingJetpack = true;
			}
		}

		if (usingJetpack && Input.GetKey (KeyCode.Space) && jetpackFuelPercent > 0) {
			lastJetpackUseTime = Time.time;
			jetpackFuelPercent -= Time.deltaTime / jetpackDuration;
			rb.AddForce (transform.up * jetpackForce, ForceMode.Acceleration);
		} else {
			usingJetpack = false;
		}

		// Refuel jetpack
		if (Time.time - lastJetpackUseTime > jetpackRefuelDelay) {
			jetpackFuelPercent = Mathf.Clamp01 (jetpackFuelPercent + Time.deltaTime / jetpackRefuelTime);
		}

		// Handle animations
		float currentSpeed = smoothVelocity.magnitude;
		float animationSpeedPercent = (currentSpeed <= walkSpeed) ? currentSpeed / walkSpeed / 2 : currentSpeed / runSpeed;
		animator.SetBool ("Grounded", isGrounded);
		animator.SetFloat ("Speed", animationSpeedPercent);
	}

	bool IsGrounded () {
		// Sphere must not overlay terrain at origin otherwise no collision will be detected
		// so rayRadius should not be larger than controller's capsule collider radius
		const float rayRadius = .3f;
		const float groundedRayDst = .2f;
		bool grounded = false;

		if (referenceBody) {
			var relativeVelocity = rb.velocity - referenceBody.velocity;
			// Don't cast ray down if player is jumping up from surface
			if (relativeVelocity.y <= jumpForce * .5f) {
				RaycastHit hit;
				Vector3 offsetToFeet = (feet.position - transform.position);
				Vector3 rayOrigin = rb.position + offsetToFeet + transform.up * rayRadius;
				Vector3 rayDir = -transform.up;

				grounded = Physics.SphereCast (rayOrigin, rayRadius, rayDir, out hit, groundedRayDst, walkableMask);
			}
		}

		return grounded;
	}

	void FixedUpdate ()
	{
		if (rb == null) return;
		
		CelestialBody[] bodies = NBodySimulation.Bodies;
		Vector3 gravityOfNearestBody = Vector3.zero;
		float nearestSurfaceDst = float.MaxValue;

		// Gravity
		foreach (CelestialBody body in bodies) {
			float sqrDst = (body.Position - rb.position).sqrMagnitude;
			Vector3 forceDir = (body.Position - rb.position).normalized;
			Vector3 acceleration = forceDir * Universe.gravitationalConstant * body.mass / sqrDst;
			//rb.AddForce (acceleration, ForceMode.Acceleration);

			float dstToSurface = Mathf.Sqrt (sqrDst) - body.radius;

			// Find body with strongest gravitational pull 
			if (dstToSurface < nearestSurfaceDst) {
				nearestSurfaceDst = dstToSurface;
				gravityOfNearestBody = acceleration;
				referenceBody = body;
			}
		}
		rb.AddForce (gravityOfNearestBody, ForceMode.Acceleration);
		// Rotate to align with gravity up
		Vector3 gravityUp = -gravityOfNearestBody.normalized;
		rb.rotation = Quaternion.FromToRotation (transform.up, gravityUp) * rb.rotation;

		// Move
		rb.MovePosition (rb.position + smoothVelocity * Time.fixedDeltaTime);
	}

	void HandleEditorInput () {
		if (Application.isEditor) {
			if (Input.GetKeyDown (KeyCode.O)) {
				Debug.Log ("Debug mode: Toggle freeze player");
				debug_playerFrozen = !debug_playerFrozen;
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void SetVelocity (Vector3 velocity) {
		rb.velocity = velocity;
	}

	public void ExitFromSpaceship () {
		cam.transform.parent = transform;
		cam.transform.localPosition = cameraLocalPos;
		smoothYaw = 0;
		yaw = 0;
		smoothPitch = cam.transform.localEulerAngles.x;
		pitch = smoothPitch;
	}
	public Camera Camera {
		get {
			return cam;
		}
	}

	public Rigidbody Rigidbody {
		get {
			return rb;
		}
	}
	
	private void PauseGame()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (_paused)
			{
				MenuManager.Instance.OpenMenu("UIPanel");
				MenuManager.Instance.TurnOnCrosshair();
				_paused = false;
				
				OnUnlockPlayerMovement();
			}
			else
			{
				MenuManager.Instance.OpenMenu("pauseMenu");
				MenuManager.Instance.TurnOffCrosshair();
				_paused = true;
				
				OnLockPlayerMovement();
			}
		}
	}


	public string GetClosestPlanetName()
    {
		return referenceBody.name;
    }
	public void OnLockPlayerMovement()
	{
		if (_view)
			if (!_view.IsMine) return;
		
		_lockPlayerMovement = true;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	public void OnUnlockPlayerMovement()
	{
		if (_view)
			if (!_view.IsMine) return;
		
		_lockPlayerMovement = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void OnSetQuest(Quest setQuest)
	{
		quest = setQuest;
	}
	
	
	

}