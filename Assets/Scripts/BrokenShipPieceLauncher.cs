using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenShipPieceLauncher : MonoBehaviour
{
    public float range;
    public float successfulAttemps;
    public LayerMask planets;
    public GameObject ShipPiecePrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LateStart(0.1f));
    }
    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Your Function You Want to Call
        float currentSuccessfulAttemps = 0;
        while (currentSuccessfulAttemps < successfulAttemps)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Random.onUnitSphere, out hit, range, planets))
            {
                currentSuccessfulAttemps++;
                Instantiate(ShipPiecePrefab, hit.point, Quaternion.AngleAxis(0, hit.normal)).transform.parent=gameObject.transform;

                
            }
            
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
