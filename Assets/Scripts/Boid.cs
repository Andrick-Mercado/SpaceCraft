using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    public Vector3 targetVelocity;
    public LayerMask walkableMask;


    public GameObject PlayerPrefab;
    CelestialBody referenceBody;

    
    public float moveSpeed;

    public Rigidbody rb;

    public Transform feet;
        
    // Initialize this Boid on Awake()
    void Awake () {

        rb = GetComponent<Rigidbody>();

        PlayerPrefab = GameObject.FindGameObjectWithTag("Player");


        // Give the Boid a random color, but make sure it's not too dark
        Color randColor = Color.black;
        while ( randColor.r + randColor.g + randColor.b < 1.0f ) {
           randColor = new Color(Random.value, Random.value, Random.value);
        }
        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach ( Renderer r in rends ) {
            r.material.color = randColor;
        }

    }

    private void Start()
    {
        /*CelestialBody startBody = FindObjectOfType<GameSetUp>().startBody;
        Vector3 pointAbovePlanet = startBody.transform.position + Vector3.right * startBody.radius * 1.1f;
        transform.position = pointAbovePlanet;
        */
    }

    // Update is called once per frame
    void Update () {
        HandleMovement();
    }

    
    void FixedUpdate() {

        CelestialBody[] bodies = NBodySimulation.Bodies;
        Vector3 gravityOfNearestBody = Vector3.zero;
        float nearestSurfaceDst = float.MaxValue;

        // Gravity
        foreach (CelestialBody body in bodies)
        {
            float sqrDst = (body.Position - rb.position).sqrMagnitude;
            Vector3 forceDir = (body.Position - rb.position).normalized;
            Vector3 acceleration = forceDir * Universe.gravitationalConstant * body.mass / sqrDst;
            rb.AddForce(acceleration, ForceMode.Acceleration);

            float dstToSurface = Mathf.Sqrt(sqrDst) - body.radius;

            // Find body with strongest gravitational pull 
            if (dstToSurface < nearestSurfaceDst)
            {
                nearestSurfaceDst = dstToSurface;
                gravityOfNearestBody = acceleration;
                referenceBody = body;
            }
        }

        // Rotate to align with gravity up
        Vector3 gravityUp = -gravityOfNearestBody.normalized;
        rb.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * rb.rotation;

        // Move
        rb.MovePosition(rb.position + targetVelocity * Time.fixedDeltaTime);
        
    }

    void HandleMovement()
    {     
        // Movement
        bool isGrounded = IsGrounded();
        Vector3 dir = transform.forward;
        
        targetVelocity = transform.TransformDirection(dir.normalized) * moveSpeed;
        

    }
    bool IsGrounded()
    {
        // Sphere must not overlay terrain at origin otherwise no collision will be detected
        // so rayRadius should not be larger than controller's capsule collider radius
        const float rayRadius = .3f;
        const float groundedRayDst = .2f;
        bool grounded = false;

        if (referenceBody)
        {
            var relativeVelocity = rb.velocity - referenceBody.velocity;
            
            
            RaycastHit hit;
            Vector3 offsetToFeet = (feet.position - transform.position);
            Vector3 rayOrigin = rb.position + offsetToFeet + transform.up * rayRadius;
            Vector3 rayDir = -transform.up;

            grounded = Physics.SphereCast(rayOrigin, rayRadius, rayDir, out hit, groundedRayDst, walkableMask);
            
        }

        return grounded;
    }

    public void SetVelocity(Vector3 velocity)
    {
        rb.velocity = velocity;
    }


}
