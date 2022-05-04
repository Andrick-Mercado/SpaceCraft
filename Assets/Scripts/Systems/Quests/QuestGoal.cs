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

    public void ItemCollected()
    {
        if (goalType == GoalType.Gathering)
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
    Gathering,
    Kill,
    Deliver
}