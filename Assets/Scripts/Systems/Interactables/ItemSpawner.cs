using System.Collections;
using UnityEngine;
using System.IO;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

public class ItemSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] [Tooltip("Use only prefabs in the resources folder")]
    private GameObject objectToSpawn;

    [Header("Settings for Spawner")] 
    [SerializeField] [Range(0f,360f)]
    private int offsetRotation;
    [SerializeField] 
    private AxisRotation onAxis;
    [SerializeField]
    private float rayCastRange;
    [SerializeField]
    private float numberOfSpawnedObjects;
    [SerializeField]
    private LayerMask layersToFind;
    
    private enum AxisRotation 
    {
        Default,
        Back,
        Forward,
        Down,
        Up,
        Left,
        Right
    }
    
    //inner methods
    private PhotonView _view;
    private Vector3 _onAxis;
    
    private void Awake()
    {
        _view = GetComponent<PhotonView>();
        
        AxisSetUp();
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
                PhotonNetwork.Instantiate(Path.Combine("Interactables/"+objectToSpawn.name),
                    hit.point, Quaternion.AngleAxis(offsetRotation,
                        _onAxis == Vector3.zero ?  hit.normal: _onAxis)).transform.parent=gameObject.transform;//hit.normal
            }
        }
    }

    private void AxisSetUp()
    {
        switch (onAxis)
        {
            case AxisRotation.Default:
                _onAxis = Vector3.zero;
                break;
            case AxisRotation.Back:
                _onAxis = Vector3.back;
                break;
            case AxisRotation.Forward:
                _onAxis = Vector3.forward;
                break;
            case AxisRotation.Down:
                _onAxis = Vector3.down;
                break;
            case AxisRotation.Up:
                _onAxis = Vector3.up;
                break;
            case AxisRotation.Left:
                _onAxis = Vector3.left;
                break;
            case AxisRotation.Right:
                _onAxis = Vector3.right;
                break;
        }
    }
}
