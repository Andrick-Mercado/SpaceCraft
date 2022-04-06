using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EndlessManager : MonoBehaviour {

    public float distanceThreshold = 1000;
    public List<Transform> physicsObjects;
    PlayerController player;
    public Camera playerCamera;

    public event System.Action PostFloatingOriginUpdate;
    
    //for multiplayer
    private PhotonView _view;

    void Awake () {
        var player = FindObjectOfType<PlayerController> ();
        var bodies = FindObjectsOfType<CelestialBody> ();

        physicsObjects = new List<Transform> ();
        //physicsObjects.Add (player.transform);
        foreach (var c in bodies) 
        {
            physicsObjects.Add (c.transform);
        }
        
        
    }

    private void Start()
    {
        playerCamera = Camera.main;
    }

    void LateUpdate () {
        if(playerCamera == null) return;
        
        UpdateFloatingOrigin ();
        if (PostFloatingOriginUpdate != null) {
            PostFloatingOriginUpdate ();
        }
    }

    void UpdateFloatingOrigin () 
    {
        Vector3 originOffset = playerCamera.transform.position;
        float dstFromOrigin = originOffset.magnitude;

        if (dstFromOrigin > distanceThreshold) {
            foreach (Transform t in physicsObjects) {
                t.position -= originOffset;
            }
        }
    }

}