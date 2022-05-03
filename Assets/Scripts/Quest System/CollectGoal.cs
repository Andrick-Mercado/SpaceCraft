using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGoal : Goal
{
    public int ItemID { get; set; }

    public CollectGoal(Quest quest, int itemID, string descript, bool completed, int currAmt, int reqAmt)
    {
        this.Quest = quest;
        this.ItemID = itemID;
        this.Descript = descript;
        this.Completed = completed;
        this.CurrAmt = currAmt;
        this.ReqAmt = reqAmt;
    }
}
