using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    static public List<Boid> boids;


    public Vector3 velocity;       // The current velocity
    public List<Boid> neighbors;      // All nearby Boids
    public List<Boid> collisionRisks; // All Boids that are too close
    public Boid closest;        // The single closest Boid


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
        // Define the boids List if it is still null
        if (boids == null)
        {
            boids = new List<Boid>();
        }
        // Add this Boid to boids
        boids.Add(this);

        // Initialize the two Lists
        neighbors = new List<Boid>();
        collisionRisks = new List<Boid>();

        // Give the Boid a random color
        Color randColor = Color.black;
        while ( randColor.r + randColor.g + randColor.b < 1.0f ) {
           randColor = new Color(Random.value, Random.value, Random.value);
        }
        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach ( Renderer r in rends ) {
            if (r.transform.name == "EyeL" || r.transform.name == "EyeR")
            {
                r.material.color = Color.black;
            }
            else
            {
                r.material.color = randColor;
            }
            
        }
        
        
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        PlayerPrefab = GameObject.FindGameObjectWithTag("Player");

        CalculateGravity();

        //CelestialBody startBody = FindObjectOfType<GameSetUp>().startBody;
        //Vector3 pointAbovePlanet = referenceBody.transform.position + Vector3.right * referenceBody.radius * 1.1f;
        //Vector3 awayFromPlanet = transform.position - referenceBody.transform.position;
        //awayFromPlanet = -awayFromPlanet.normalized;

        Vector3 disAway = transform.position - referenceBody.transform.position;
        int count = 0;
        while(disAway.magnitude < referenceBody.radius + spawnPlanetPadding)
        {
            //Debug.Log("Boid from Planet Radius: " + disAway.magnitude);
            transform.position += transform.up * 2f;
            disAway = transform.position - referenceBody.transform.position;
            if (count > 10)
            {
                break;
            }
            count++;
        }
        disAway = transform.position - referenceBody.transform.position;
        //Debug.Log(disAway.magnitude);
        //Debug.Log("planet: " + referenceBody.transform.name);
    }

    // Update is called once per frame
    void Update () {
        //bool isGrounded = IsGrounded();
        targetVelocity = rb.velocity;

        // Get the list of potential nearby Boids
        List<Boid> neighbors = GetNeighbors(this);

        //Debug.Log("# neighbors: " + neighbors.Count);
        // If the Boid has neighbors, adjust directional vector for flocking
        if(neighbors.Count != 0)
        {
            // Get average vel of neighbor boids
            Vector3 neighborVel = GetAverageVelocity(neighbors);
            // Adjust boid velocity to match better with surrounding boids
            targetVelocity += neighborVel * BoidSpawner.S.velocityMatchingAmt;

            // Flock towards center of neighbors
            Vector3 neighborCenterOffset = GetAveragePosition(neighbors) - this.transform.position;
            targetVelocity += neighborCenterOffset * BoidSpawner.S.flockCenteringAmt;

            // Avoid running into Boids if collision risk
            Vector3 dist;
            if (collisionRisks.Count > 0)
            {
                Vector3 collisionAveragePos = GetAveragePosition(collisionRisks);
                dist = collisionAveragePos - this.transform.position;
                targetVelocity += dist * BoidSpawner.S.collisionAvoidanceAmt;
            }

        }
        // Adjust the current velocity based on targetVelocity using a linear interpolation
        velocity = (1 - BoidSpawner.S.velocityLerpAmt) * velocity + BoidSpawner.S.velocityLerpAmt * targetVelocity;

        
        // Make sure boid velocity magnitude is within min and max limits
        // Only used in dynamic boid speeds
        /*if (velocity.magnitude > BoidSpawner.S.maxVelocity)
        {
            velocity = velocity.normalized * BoidSpawner.S.maxVelocity;
        }
        if (velocity.magnitude < BoidSpawner.S.minVelocity)
        {
            velocity = velocity.normalized * BoidSpawner.S.minVelocity;
        }*/
    }

    
    void FixedUpdate() {
        CalculateGravity();

        Vector3 dirOfPlanet = (referenceBody.transform.position - transform.position);
        //Debug.DrawLine(transform.position, referenceBody.transform.position, Color.green, 10000f);
        //Debug.DrawRay(transform.position, dirOfPlanet, Color.red, 9999f);

        if (Physics.Raycast(transform.position, dirOfPlanet, out var hit, 10000f))
        {
            //Normalized direction vector for flocking behavior
            velocity = velocity.normalized;

            //Apply flocking vector to forward movement and move speed.
            Vector3 movementVec = transform.forward + velocity * BoidSpawner.S.moveSpeed * Time.fixedDeltaTime;

            Vector3 playerToRayVec = (hit.point - transform.position);
            if (playerToRayVec.magnitude > BoidSpawner.S.maxDistancefromPlanet)
            {
                //Debug.Log("TOO HIGH: " + (hit.point - transform.position).magnitude);
                //Debug.DrawRay(transform.position, dirOfPlanet, Color.white, 20);
                rb.AddForce(-transform.up * BoidSpawner.S.moveSpeed, ForceMode.Force);
            }
            else if(playerToRayVec.magnitude < BoidSpawner.S.minDistancefromPlanet)
            {
                //Debug.DrawRay(transform.position, dirOfPlanet, Color.red, 20);
                //Debug.Log("TOO LOW: " + playerToRayVec.magnitude);
                rb.AddForce(transform.up * BoidSpawner.S.moveSpeed, ForceMode.Force);
            }
            
            //Apply resulting velocity vector to players rigidbody
            rb.velocity = movementVec;
            //rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeed);

            //Rotation after finding direction vector to rotate object towards velocity vector
            rb.rotation = Quaternion.FromToRotation(transform.forward, rb.velocity.normalized) * rb.rotation;
            //transform.rotation = Quaternion.LookRotation(rb.velocity); 
        }
        else
        {
            Debug.Log("BOID RAYCAST FAIL");
            CalculateGravity();
        }

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
            //rb.AddForce(acceleration, ForceMode.Acceleration);

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

    public List<Boid> GetNeighbors(Boid boi)
    {
        float closestDist = float.MaxValue;
        Vector3 delta;
        float dist;
        neighbors.Clear();
        collisionRisks.Clear();

        foreach (Boid b in boids)
        {
            if (b == boi) continue;
            delta = b.transform.position - boi.transform.position;
            dist = delta.magnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = b;
            }
            if (dist < BoidSpawner.S.nearDist)
            {
                neighbors.Add(b);
            }
            if (dist < BoidSpawner.S.collisionDist)
            {
                collisionRisks.Add(b);
            }
        }
        return (neighbors);
    }

    // Get the average position of the Boids in a List<Boid>
    public Vector3 GetAveragePosition(List<Boid> someBoids)
    {
        Vector3 sum = Vector3.zero;
        foreach (Boid b in someBoids)
        {
            sum += b.transform.position;
        }
        Vector3 center = sum / someBoids.Count;
        return (center);
    }

    // Get the average velocity of the Boids in a List<Boid>
    public Vector3 GetAverageVelocity(List<Boid> someBoids)
    {
        Vector3 sum = Vector3.zero;
        foreach (Boid b in someBoids)
        {
            sum += b.rb.velocity;
        }
        Vector3 avg = sum / someBoids.Count;
        return (avg);
    }


}
