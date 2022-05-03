using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{

    
    // This is a Singleton of the BoidSpawner. There is only one instance
    //   of BoidSpawner, so we can store it in a static variable named S.
    static public BoidSpawner S;

    [Header("Customise Boid Properties")]
    // These fields allow you to adjust the behavior of the Boids as a group
    public int numBoids = 100;
    public GameObject boidPrefab;
    public float moveSpeed = 20f;
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

    public float maxDistancefromPlanet = 20;
    public float minDistancefromPlanet = 3;

    private void Awake()
    {
        // Set the Singleton S to be this instance of BoidSpawner
        S = this;
    }
   
    

    

}