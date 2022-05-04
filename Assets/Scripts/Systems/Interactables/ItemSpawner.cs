using System;
using System.Collections;
using UnityEngine;
using System.IO;
using Photon.Pun;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] [Tooltip("Use only prefabs in the resources folder")]
    private GameObject objectToSpawn;
    [SerializeField] [Tooltip("Planet for the objets to face gravity")]
    private GameObject planetToSpawnInObject;

    [Header("Settings for Spawner")] 
    [SerializeField]
    private float offsetDistanceFromHome;
    [SerializeField]
    private float rayCastRange;
    [SerializeField]
    private float numberOfSpawnedObjects;
    [SerializeField] 
    private float lateStartTime;
    [SerializeField][TagSelector]
    private string tagToFind;
    [SerializeField] 
    private LayerMask layersThatPreventSpawning;

    private void Start()
    {
        StartCoroutine(LateStartOffline(lateStartTime));
    }

    [ContextMenu("Spawn Objects for Offline")]
    private void SpawnObjectsOffline()
    {
        if(Application.isPlaying)
            StartCoroutine(LateStartOffline(lateStartTime));
    }
    
    private IEnumerator LateStartOffline(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        float currentSuccessfulAttemps = 0;
        while (currentSuccessfulAttemps < numberOfSpawnedObjects)
        {
            if (Physics.Raycast(transform.position, Random.onUnitSphere,
                    out var hit, rayCastRange, ~layersThatPreventSpawning) && hit.transform.CompareTag(tagToFind))
            {
                currentSuccessfulAttemps++;
                Transform obj = Instantiate(objectToSpawn,
                    hit.point, Quaternion.identity).transform;
                
                obj.parent = gameObject.transform;
                
                

                Vector3 gravityUp = ( obj.position - planetToSpawnInObject.transform.position ).normalized;
                Vector3 localUp = obj.transform.up;
                
                obj.rotation = Quaternion.FromToRotation(localUp, gravityUp) * obj.rotation;
                obj.localPosition += new Vector3(0f, offsetDistanceFromHome, 0f);
            }
        }
    }
    
    
}
