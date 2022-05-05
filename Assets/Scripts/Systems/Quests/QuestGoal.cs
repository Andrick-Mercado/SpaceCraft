using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestGoal 
{
    public GoalType goalType;
    public int requiredAmount;
    public int currentAmount;

    public bool IsReached()
    {
        if (currentAmount >= requiredAmount)
        {
            Debug.Log("Completed Quest: "+ goalType);
            return true;
        }
        return false;

    }

    public void EnemyKilled()
    {
        if (goalType == GoalType.Kill)
            currentAmount++;
    }

    public void ItemCollectedFlint()
    {
        if (goalType == GoalType.GatherFlint)
            currentAmount++;
    }
    public void ItemCollectedMineral()
    {
        if (goalType == GoalType.GatherMineral)
            currentAmount++;
    }
    public void ItemCollectedTree()
    {
        if (goalType == GoalType.GatherTree)
            currentAmount++;
    }
    public void DeliverPackage()
    {
        if (goalType == GoalType.Deliver)
            currentAmount++;
    }
}

public enum GoalType
{
    GatherFlint,
    GatherMineral,
    GatherTree,
    Kill,
    Deliver
}