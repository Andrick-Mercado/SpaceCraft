using System.Collections;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private float rayCastRange;
    [SerializeField]
    private float numberOfSpawnedObjects;
    [SerializeField]
    private LayerMask layersToFind;

    [SerializeField] 
    private GameObject objectToSpawn;
    
    //inner methods
    private PhotonView _view;
    
    private void Awake()
    {
        _view = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(LateStart(0.1f));
        }
    }
    
    private IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        float currentSuccessfulAttemps = 0;
        while (currentSuccessfulAttemps < numberOfSpawnedObjects)
        {
            if (Physics.Raycast(transform.position, Random.onUnitSphere, out var hit, rayCastRange, layersToFind))
            {
                currentSuccessfulAttemps++;
                PhotonNetwork.Instantiate(Path.Combine(objectToSpawn.name),
                    hit.point, Quaternion.AngleAxis(0, hit.normal)).transform.parent=gameObject.transform;
            }
        }
    }
}
