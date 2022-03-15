using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public Quest quest;

    public int itemCount = 0;
    public TextMeshProUGUI itemCountText;

    public GameObject questComplete;
    public void GoCollect()
    {          

        if (quest.isActive)
        {
            itemCount++;
            itemCountText.text = itemCount.ToString();
            quest.goal.ItemCollected();
            if (quest.goal.isReached())
            {
                //XP = quest.XPReward;
                //gold = quest.goldReward;
                quest.Complete();
                questComplete.SetActive(true);
            }
        }
    }

}
