using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public Quest Quest { get; set; }
    public string Descript { get; set; }
    public bool Completed { get; set; }
    public int CurrAmt { get; set; }
    public int ReqAmt { get; set; }

    public virtual void Init()
    {

    }

    public void Evaluate()
    {
        if (CurrAmt >= ReqAmt)
        {
            Complete();
        }
    }

    public void Complete()
    {
        Quest.CheckGoals();
        Completed = true;
        Debug.Log("Goal marked as completed.");
    }
}
