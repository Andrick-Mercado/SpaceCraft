using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
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

    public Rigidbody rb;

    public Transform feet;



    CelestialBody[] bodies;
    Vector3 gravityOfNearestBody;
    float nearestSurfaceDst;

    // Initialize this Boid on Awake()
    void Awake () {
        // Give the Boid a random color
        Color randColor = Color.black;
        while (randColor.r + randColor.g + randColor.b < 1.0f)
        {
            randColor = new Color(Random.value, Random.value, Random.value);
        }
        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            if (r.transform.name == "EyeL" || r.transform.name == "EyeR")
            {
                r.material.color = Color.black;
            }
            else
            {
                r.material.color = randColor;
            }

        }
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
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


        
        
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        

        PlayerPrefab = GameObject.FindGameObjectWithTag("Player");

        CalculateGravity();

        //CelestialBody startBody = FindObjectOfType<GameSetUp>().startBody;
        //Vector3 pointAbovePlanet = referenceBody.transform.position + Vector3.right * referenceBody.radius * 1.1f;
        //Vector3 awayFromPlanet = transform.position - referenceBody.transform.position;
        //awayFromPlanet = -awayFromPlanet.normalized;

        Vector3 disAway = transform.position - referenceBody.transform.position;
        int count = 0;
        while(disAway.magnitude < referenceBody.radius + BoidSpawner.S.spawnPlanetPadding)
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
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
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
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
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
                rb.AddForce(-transform.up * BoidSpawner.S.moveQuicklySpeed, ForceMode.Force);
            }
            else if(playerToRayVec.magnitude < BoidSpawner.S.minDistancefromPlanet && playerToRayVec.magnitude > 1f)
            {
                //Debug.DrawRay(transform.position, dirOfPlanet, Color.red, 20);
                //Debug.Log("TOO LOW: " + playerToRayVec.magnitude);
                //Debug.Log("quickup " + playerToRayVec.magnitude);
                rb.AddForce(transform.up * BoidSpawner.S.moveQuicklySpeed, ForceMode.Force);
            }
            else if(playerToRayVec.magnitude < 1f)
            {
                //Debug.Log("VERYquickup " + playerToRayVec.magnitude);
                Debug.DrawRay(transform.position, playerToRayVec, Color.blue, 50);
                rb.AddForce(transform.up * BoidSpawner.S.moveReallyQuickSpeed, ForceMode.Force);
            }

            //Apply resulting velocity vector to players rigidbody
            rb.velocity = movementVec;
            //rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeed);

            //Rotation after finding velocity vector to rotate object towards the direction of velocity vector
            rb.rotation = Quaternion.FromToRotation(transform.forward, rb.velocity.normalized) * rb.rotation;
            //transform.rotation = Quaternion.LookRotation(rb.velocity); 
        }
        else
        {
            //If raycast fail, either Boid spawned inside planet or some void. Better to destroy self than use resources
            Debug.Log("BOID RAYCAST FAIL");
            Destroy(gameObject);
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

    //If neighboring Boids are either "nearDist" or "collisionDist" away from this Boid, add them to a List<Boid> of neighbors
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
            //Can only have 3 other boid neighbors to flock. Help stop flocking of EVERY boid
            if (neighbors.Count == 3) break;
            
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
