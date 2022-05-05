using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerTraversal : MonoBehaviour
{
    private CelestialBody[] bodies;

    public PlayerController pc;
    public Rigidbody rb;

    public TextMeshProUGUI traversalText;
    public Button traversalYes;
    public Button traversalNo;

    Vector3 rayHit;
    Vector3 rayHitNormal;

    bool inTraversal = false;

    void Start()
    {
        bodies = NBodySimulation.Bodies;
        rb = gameObject.GetComponent<Rigidbody>();
        pc = gameObject.GetComponent<PlayerController>();

        GameObject canvas = GameObject.Find("menuCanvas");
        Button[] menubuttons = canvas.transform.GetComponentsInChildren<Button>(true);
        foreach(Button buttton in menubuttons)
        {
            if (buttton.name == "Traversal_Yes") traversalYes = buttton;
            if (buttton.name == "Traversal_No") traversalNo = buttton;
        }
        traversalYes.onClick.AddListener(() => TraversalYesInput());
        traversalNo.onClick.AddListener(() => TraversalNoInput());

        TextMeshProUGUI[] guitext = canvas.transform.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach(TextMeshProUGUI traversaltext in guitext)
        {
            if (traversaltext.name == "Traversal_Text") traversalText = traversaltext;
        }
    }

    void Update()
    {
        PingPlanet();
    }

    public void PingPlanet()
    {
        if (!inTraversal)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, 99999f))
                {
                    if (hit.transform.name != pc.GetClosestPlanetName() && hit.transform.name != "Sun")
                    {
                        foreach (CelestialBody celeBody in bodies)
                        {
                            if (celeBody.name == hit.transform.name)
                            {
                                rayHit = hit.point;
                                rayHitNormal = hit.normal;
                                traversalText.text = hit.transform.name;
                                MenuManager.Instance.OpenMenu("traversalMenu");
                                Cursor.visible = true;
                                Cursor.lockState = CursorLockMode.None;
                            }
                        }
                    }

                }
            }
        }
    }

    public void TraversalYesInput()
    {
        inTraversal = true;
        StartCoroutine(MoveToPlanet(transform.position));
    }

    public void TraversalNoInput()
    {
        MenuManager.Instance.CloseMenu("traversalMenu");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator MoveToPlanet(Vector3 currPos)
    {
        TraversalNoInput();
        MenuManager.Instance.TurnOffCrosshair();
        float time = 0;
        rayHit = rayHit + rayHitNormal * 1.5f;
        while(time < 1)
        {
            time += Time.deltaTime * .1f;
            if (time > 1) time = 1;
            transform.position = Vector3.Lerp(currPos, rayHit, time);
            yield return null;
        }
        rb.velocity = Vector3.zero;
        inTraversal = false;
        MenuManager.Instance.TurnOnCrosshair();
    }
}
