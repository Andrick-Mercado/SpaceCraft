using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Quest : MonoBehaviour
{
    public List<Goal> Goals { get; set; } = new List<Goal>();
    public string Descript { get; set; }
    public bool Completed { get; set; }
    public int CurrAmt { get; set; }
    public int ReqAmt { get; set; }

    public InventoryItemData ItemReward { get; set; }

    public void CheckGoals()
    {
        Completed = Goals.All(g => g.Completed); //will go through all goals and check to see if they are completed/ true
    }

    public void GiveReward()
    {
        if (ItemReward != null)
        {
            InventorySystem.Instance.Get(ItemReward);
        }
    }
}
