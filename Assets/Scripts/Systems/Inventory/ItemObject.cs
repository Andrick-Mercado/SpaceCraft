using System;
using Photon.Pun;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    private PhotonView _view;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    public void OnHandlePickupItem()
    { 
        InventorySystem.Instance.Add(referenceItem);
        
        _view.RPC(nameof(OnHandlePickupItemOverNetwork), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void OnHandlePickupItemOverNetwork()
    {
        if (QuestGiver.Instance.GetCurrentQuest() != null 
            && QuestGiver.Instance.GetCurrentQuest().isActive 
                && QuestGiver.Instance.GetCurrentQuest().questGoal[0].goalType == GoalType.GatherFlint 
                    && referenceItem.displayName == "Flint")
        {
            QuestGiver.Instance.GetCurrentQuest().questGoal[0].ItemCollectedFlint();

            if (QuestGiver.Instance.GetCurrentQuest().questGoal[0].IsReached())
            {
                QuestGiver.Instance.GetCurrentQuest().Complete();
                QuestGiver.Instance.CurrentQuest++;
            }
        }
        if (QuestGiver.Instance.GetCurrentQuest() != null 
            && QuestGiver.Instance.GetCurrentQuest().isActive 
                && QuestGiver.Instance.GetCurrentQuest().questGoal[0].goalType == GoalType.GatherMineral
                    && referenceItem.displayName == "Mineral")
        {
            QuestGiver.Instance.GetCurrentQuest().questGoal[0].ItemCollectedMineral();

            if (QuestGiver.Instance.GetCurrentQuest().questGoal[0].IsReached())
            {
                QuestGiver.Instance.GetCurrentQuest().Complete();
                QuestGiver.Instance.CurrentQuest++;
            }
        }
        if (QuestGiver.Instance.GetCurrentQuest() != null 
            && QuestGiver.Instance.GetCurrentQuest().isActive 
                && QuestGiver.Instance.GetCurrentQuest().questGoal[0].goalType == GoalType.GatherTree
                    && referenceItem.displayName == "Tree")
        {
            QuestGiver.Instance.GetCurrentQuest().questGoal[0].ItemCollectedTree();

            if (QuestGiver.Instance.GetCurrentQuest().questGoal[0].IsReached())
            {
                QuestGiver.Instance.GetCurrentQuest().Complete();
                QuestGiver.Instance.CurrentQuest++;
            }
        }
    }
}
