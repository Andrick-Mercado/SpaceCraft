using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : NPC
{
    public bool AssignedQuest { get; set; }
    public bool Helped { get; set; } //NPC tracker to see if quest is completed (have I already been helped?)

    [SerializeField]
    private GameObject quests;

    [SerializeField]
    private string questType; //how to pass individual quest name/titles
    public Quest Quest { get; set; } // tracks which quest and what status it is at

    public override void Interact()
    {
        
        if (!AssignedQuest && !Helped)
        {
            base.Interact();
            AssignQuest();
        }
        else if (AssignedQuest && !Helped)
        {
            CheckQuest();
        }
        else
        {
            //DialogueSystem.Instance.AddNewDialogue(new string[] {"Great! I can now help you rebuild your ship.", "more dialogue"}, name);
        }

    }

    void AssignQuest()
    {
        AssignedQuest = true;
        Quest = (Quest)quests.AddComponent(System.Type.GetType(questType)); //grabs all the information of a single quest
        
    }

    void CheckQuest()
    {
        if (Quest.Completed)
        {
            //Quest.GiveReward(); (the reward is helping rebuild the ship)
            Helped = true;
            AssignedQuest = false;
        }
        else
        {
            //DialogueSystem.Instance.AddNewDialogue(new string[] {"You don't have enough still! Maybe you need more help.", "more dialogue"}, name);
        }
    }
}
