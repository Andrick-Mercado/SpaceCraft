using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutFlagOnPlanetSurface : MonoBehaviour
{
    
    void Start()
    {
        CelestialBody startBody = FindObjectOfType<GameSetUp>().startBody;
        Vector3 rayToPlanet = startBody.transform.position - transform.position;

        if(Physics.Raycast(transform.position, rayToPlanet, out var hit, 9999f))
        {
            //Move flag to hit position
            transform.position = hit.point;

            //Rotate flag rightway to normal of planets ray hit
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            //Put the flagpole above the ground
            transform.position += transform.up * 1.5f;
        }
    }

}
