using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BoidSpawner : MonoBehaviour
{

    
    // This is a Singleton of the BoidSpawner. There is only one instance
    //   of BoidSpawner, so we can store it in a static variable named S.
    static public BoidSpawner S;

    [Header("Customise Boid Properties")]
    // These fields allow you to adjust the behavior of the Boids as a group
    public float moveSpeed = 300f;
    //public float minVelocity = 300f;
    //public float maxVelocity = 999f;
    public float moveQuicklySpeed = 500f;
    public float moveReallyQuickSpeed = 1000f;
    //public float minVelocity = 0f;
    //public float maxVelocity = 30f;
    public float nearDist = 30f;
    public float collisionDist = 5f;
    public float velocityMatchingAmt = 0.01f;
    public float flockCenteringAmt = 0.15f;
    public float collisionAvoidanceAmt = -0.15f;
    public float attractionAmt = 0.01f;
    public float attractionAmtClose = 0.5f;
    public float avoidanceAmt = 0.75f;
    public float avoidanceDist = 15f;
    public float velocityLerpAmt = 0.15f;

    public float maxDistancefromPlanet = 5;
    public float minDistancefromPlanet = 3;

    public int spawnPlanetPadding = 20;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Set the Singleton S to be this instance of BoidSpawner
            S = this;
        }

        
    }
   
    

    

}