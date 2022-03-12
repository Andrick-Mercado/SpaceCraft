using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{

    [Header("Select between Lazy Flight and Follow the Leader flocking mode")]
    [Tooltip("Randomly generates a waypoint for Boids to flock towards. New waypoint is generated if Boid passes over previous.")]
    public bool LazyFlight;
    [Tooltip("Boids will flock to the current Mouse Position")]
    public bool FollowTheLeader;


    // This is a Singleton of the BoidSpawner. There is only one instance
    //   of BoidSpawner, so we can store it in a static variable named S.
    static public BoidSpawner S;

    [Header("Customise Boid Properties")]
    // These fields allow you to adjust the behavior of the Boids as a group
    public int numBoids = 100;
    public GameObject boidPrefab;                             
    public float spawnRadius = 100f;
    public float spawnVelocity = 10f;
    public float minVelocity = 0f;
    public float maxVelocity = 30f;
    public float nearDist = 30f;
    public float collisionDist = 5f;
    public float velocityMatchingAmt = 0.01f;
    public float flockCenteringAmt = 0.15f;
    public float collisionAvoidanceAmt = -0.5f;
    public float attractionAmt = 0.01f;
    public float attractionAmtClose = 0.5f;
    public float avoidanceAmt = 0.75f;
    public float avoidanceDist = 15f;
    public float velocityLerpAmt = 0.25f;

    
    [Header("Tracking of Mouse Position and Position of Randomly Generated Waypoint")]                                      
    public Vector3 mousePos;

    public Vector3 newRandPos;

    private GameObject wayPoint;

    private void Awake()
    {
        // Set the Singleton S to be this instance of BoidSpawner
        S = this;
    }
    void Start()
    {
                                                            
        // Instantiate numBoids Boids
        for (int i = 0; i < numBoids; i++)
        {
             Instantiate(boidPrefab);
        }

        //Create create within Boid spawning radius to become visual cue for new Lazy Flight mode location
        wayPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Generate a new Random position for the Lazy Flight mode
        GenerateNewPos();
        //Set waypoint to inactive until Lazy Flight mode is selected in Inspector
        wayPoint.SetActive(false);
    }

    void LateUpdate()
    {                                               
        // Track the mouse position. This keeps it the same for all Boids.
        Vector3 mousePos2d = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.transform.position.y);
        //mousePos = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenToWorldPoint(mousePos2d);
        mousePos = GameObject.Find("Player").transform.position;
        

        //Make sure only one Flight Mode is active when changing in Inspector
        if (LazyFlight)
        {
            FollowTheLeader = false;
            wayPoint.SetActive(true);
        }
        else
        {
            LazyFlight = false;
            wayPoint.SetActive(false);
        }
    }

    //Generates a new random position within the spawn radius of the Main Camera for new Lazy Flight mode waypoint.
    public void GenerateNewPos()
    {
        newRandPos = this.transform.position + Random.insideUnitSphere * BoidSpawner.S.spawnRadius;
        newRandPos.y = 1;
        wayPoint.transform.position = newRandPos;
    }

}