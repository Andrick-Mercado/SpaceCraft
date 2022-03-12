using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharController : MonoBehaviour
{
    //Movement/Camera properties
    public float walkSpeed = 7f;
    public float runSpeed = 11f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;
    public float camSpeed = 2f;
    public float lookUpDownLimit = 45f;
    public float acceleration;
    private Rigidbody rb;
    public Transform closestMass;
    public KeyCode runKey;
    public KeyCode jumpKey;
    //Grab reference to player Camera
    public Camera playerCamera;
    //Declare reference to character controller inspector component

    //Vars used to calculate Camera and Player movement
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float rotationY = 0;
    float curSpeedX;
    float curSpeedY;

    //Bool to use in future to dictate when player is not allowed to move
    public bool canMove = true;

    bool camAtPlayer = false;
    
    //for multiplayer
    private PhotonView _view;

    
    void Start()
    {
        //closestMass
        closestMass = GameObject.Find("Planet").transform;
        //grab reference to the Character's rigidbody
        rb = GetComponent<Rigidbody>();


        //Locks and makes Mouse Cursor invisible when focused on game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        //get photon view component
        _view = GetComponent<PhotonView>();
    }

    void FixedUpdate()
    {
        //prevent other players from moving others
        if (!_view.IsMine) return;
        
        //Get transform position for forward and right based on current direction facing
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //If canMove, if playing holding shift, apply running speed, else, apply walking speed
        if (canMove)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                curSpeedX = runSpeed * Input.GetAxis("Vertical");
                curSpeedY = runSpeed * Input.GetAxis("Horizontal");
            }
            else
            {
                curSpeedX = walkSpeed * Input.GetAxis("Vertical");
                curSpeedY = walkSpeed * Input.GetAxis("Horizontal");
            }
        }
        else
        {
            curSpeedX = curSpeedY = 0;
        }

        //Move Character
        //calculate desired movement direction from input
        Vector3 movementImpulse = (forward* Input.GetAxis("Vertical") + right* Input.GetAxis("Horizontal"));
        if (movementImpulse.magnitude > 1)
            movementImpulse.Normalize();
        //decide on player speed
        if (Input.GetKey(runKey))
        {
            movementImpulse *= runSpeed * Time.fixedDeltaTime;
        }
        else
        {
            movementImpulse *= walkSpeed * Time.fixedDeltaTime;
        }
        //decide on gravity direction
        Vector3 gravityVector = Vector3.zero;
        gravityVector = transform.TransformDirection(Vector3.down);
        gravityVector = Vector3.Scale(gravityVector, rb.velocity);
        gravityVector -= gravity * transform.up;
        //apply jump velocity
        if (Input.GetKey(jumpKey)){
            gravityVector += jumpSpeed * transform.up;
        }
        //apply movement and gravity
        rb.velocity = movementImpulse+gravityVector;

    
        //Camera Rotations
        if (canMove)
        {
            //Calculate X rotation movement
            rotationX += -Input.GetAxis("Mouse Y") * camSpeed;
            //Calculate X rotation clamp so character doesent bend neck backwards or into self
            rotationX = Mathf.Clamp(rotationX, -lookUpDownLimit, lookUpDownLimit);

            //Rotate Camera on X axis
            if (camAtPlayer)
            {
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 180, 0);
            }
            else playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            //Rotate Player Character on Y axis
            rotationY += Input.GetAxis("Mouse X") * camSpeed;
            Vector3 up = (transform.position - closestMass.position).normalized;
            transform.rotation *= Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X") * camSpeed,0));
            //make the player' bottom point towards the ground
            transform.rotation = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
            
            //Vector3 LookAt = Vector3.Cross(up, -transform.right) + up;
            //rb.transform.LookAt(LookAt, up);
        }
                    //First Person
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camAtPlayer = false;
            playerCamera.transform.localPosition = new Vector3(0, 1, 0);
        }
        //Third Person
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camAtPlayer = false;
            playerCamera.transform.localPosition = new Vector3(0, 2, -5);
        }
        //Third Person Looking at Character
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            camAtPlayer = true;
            playerCamera.transform.localPosition = new Vector3(0, 2, 5);
        }
    }
}
