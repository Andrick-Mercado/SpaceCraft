using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    public Vector3 targetVelocity;
    public LayerMask walkableMask;


    public GameObject PlayerPrefab;
    public CelestialBody referenceBody;

    public float moveSpeed;
    public int spawnPlanetPadding;

    public Rigidbody rb;

    public Transform feet;



    CelestialBody[] bodies;
    Vector3 gravityOfNearestBody;
    float nearestSurfaceDst;

    // Initialize this Boid on Awake()
    void Awake () {

        rb = GetComponent<Rigidbody>();

        PlayerPrefab = GameObject.FindGameObjectWithTag("Player");


        // Give the Boid a random color
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
        CalculateGravity();

        //CelestialBody startBody = FindObjectOfType<GameSetUp>().startBody;
        //Vector3 pointAbovePlanet = referenceBody.transform.position + Vector3.right * referenceBody.radius * 1.1f;
        //Vector3 awayFromPlanet = transform.position - referenceBody.transform.position;
        //awayFromPlanet = -awayFromPlanet.normalized;
        Debug.Log("BoidStart");
        Vector3 disAway = transform.position - referenceBody.transform.position;
        
        
        int count = 0;
        while(disAway.magnitude < referenceBody.radius + spawnPlanetPadding)
        {
            //Debug.Log("Boid from Planet Radius: " + disAway.magnitude);
            transform.position += transform.up * 5f;
            disAway = transform.position - referenceBody.transform.position;
            if (count > 10)
            {
                break;
            }
            count++;
        }
        disAway = transform.position - referenceBody.transform.position;
        Debug.Log("planet: " + referenceBody.transform.name);

        Vector3 dirOfPlanet = (referenceBody.transform.position - transform.position);
        //Debug.DrawLine(transform.position, referenceBody.transform.position, Color.green, 10000f);
        Debug.DrawRay(transform.position, dirOfPlanet, Color.red, 9999f);
        if (Physics.Raycast(transform.position, dirOfPlanet, out var hit, 10000f))
        {
 
            //Debug.DrawLine(transform.position, hit.point, Color.white, 10000f);
            //Debug.Log(hit.transform.name);
            //Debug.Log("Distance: " + (transform.position - hit.point).magnitude);



        }
            
            
            
        
    }

    // Update is called once per frame
    void Update () {
        HandleMovement();
        
    }

    
    void FixedUpdate() {

        CalculateGravity();

        // Move
        rb.MovePosition(rb.position + targetVelocity * Time.fixedDeltaTime);
        
    }

    void CalculateGravity()
    {
        bodies = NBodySimulation.Bodies;
        gravityOfNearestBody = Vector3.zero;
        nearestSurfaceDst = float.MaxValue;

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
        //rb.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * rb.rotation;
        //transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
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
