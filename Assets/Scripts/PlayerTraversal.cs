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

    void Start()
    {
        bodies = NBodySimulation.Bodies;
        rb = gameObject.GetComponent<Rigidbody>();
        pc = gameObject.GetComponent<PlayerController>();
        //traversalText = GameObject.Find("Traversal_Text").GetComponent<TextMeshProUGUI>();
        //traversalYes = GameObject.Find("Traversal_Yes").GetComponent<Button>();
        //traversalNo = GameObject.Find("Traversal_No").GetComponent<Button>();

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

    // Update is called once per frame
    void Update()
    {
        PingPlanet();
    }

    public void PingPlanet()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, 99999f))
            {
                foreach(CelestialBody celeBody in bodies)
                {
                    if(celeBody.name == hit.transform.name)
                    {
                        traversalText.text = hit.transform.name;
                        MenuManager.Instance.OpenMenu("traversalMenu");
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                }
            }
        }
    }

    public void TraversalYesInput()
    {
        Debug.Log("Traversal Yes");
    }

    public void TraversalNoInput()
    {
        MenuManager.Instance.CloseMenu("traversalMenu");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
