using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenShipQuest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Broken Ship Quest assigned!");
        //QuestName = "Broken Ship: Lost Travelers";
        
        Descript = "Collect material to fix your ship";
        //ItemReward = do i need this??? => get back home or continue exploring?
        
        //Goals.Add(new CollectGoal(this, 0, "Collect 10 trees", false, 0, 1);
        //Goals.Add(new CollectGoal(this, 1, "Collect 5 flints", false, 0, 1);

        //Goals.ForEach(g => g.Init());
    }

}
