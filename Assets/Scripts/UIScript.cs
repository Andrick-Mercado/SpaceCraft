using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    Button FTL, LF;
    Text FlockingMode;

    string LazyF = "Lazy Flight",  FollowTL= "Follow The Leader";


    private void Start()
    {
        FTL = GameObject.Find("Button_Follow").GetComponent<Button>();
        FTL.onClick.AddListener(() => SelectFTL());
        LF = GameObject.Find("Button_Lazy").GetComponent<Button>();
        LF.onClick.AddListener(() => SelectLF());
        FlockingMode = GameObject.Find("Text_Mode").GetComponent<Text>();

        SelectLF();
    }

    void SelectLF()
    {
        FlockingMode.text = LazyF;
        BoidSpawner.S.LazyFlight = true;
        BoidSpawner.S.FollowTheLeader = false;
    }
    void SelectFTL()
    {
        FlockingMode.text = FollowTL;
        BoidSpawner.S.LazyFlight = false;
        BoidSpawner.S.FollowTheLeader = true;
    }
}
