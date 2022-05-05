using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    public void OnHandlePickupItem()
    { 
        InventorySystem.Instance.Add(referenceItem);
        
        if (QuestGiver.Instance.GetCurrentQuest() != null 
            && QuestGiver.Instance.GetCurrentQuest().isActive 
                && QuestGiver.Instance.GetCurrentQuest().questGoal[0].goalType == GoalType.Gather)
        {
            QuestGiver.Instance.GetCurrentQuest().questGoal[0].ItemCollected();

            if (QuestGiver.Instance.GetCurrentQuest().questGoal[0].IsReached())
            {
                QuestGiver.Instance.GetCurrentQuest().Complete();
                QuestGiver.Instance.CurrentQuest++;
            }
        }
    }
}
