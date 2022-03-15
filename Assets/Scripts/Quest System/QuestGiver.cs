using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestGiver : MonoBehaviour
{
    public Quest quest;
    public Player player;

    public GameObject questWindow;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI XPText;
    public TextMeshProUGUI goldText;

    public void OpenQuestWindow()
    {
        questWindow.SetActive(true);
        titleText.text = quest.title;
        descriptionText.text = quest.description;
        XPText.text = quest.XPReward.ToString();
        goldText.text = quest.goldReward.ToString();
    }

    public void AcceptQuest()
    {
        questWindow.SetActive(false);
        quest.isActive = true;

        //give the quest to player
        player.quest = quest; //only accepts one quest at a time, if want multiple go into Player and change into List
    }

}
