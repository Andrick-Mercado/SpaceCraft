using TMPro;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public static QuestGiver Instance;
    
    public Quest[] quest;
    public PlayerController playerController;

    public TextMeshProUGUI questName;
    public TextMeshProUGUI questDescription;
    public TextMeshProUGUI questExperience;

    [HideInInspector] public int CurrentQuest;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenQuestWindow()
    {
        if (CurrentQuest >= quest.Length)
        {
            MenuManager.Instance.OpenMenu("questMenu");
            questName.text = "You Done";
            questDescription.text = "You Finished all the available quests";
            questExperience.text = "";
            PlayerController.Instance.LockPlayerMovement(true);
            return;
        }
        
        MenuManager.Instance.OpenMenu("questMenu");
        questName.text = quest[CurrentQuest].title;
        questDescription.text = quest[CurrentQuest].description;
        questExperience.text = $"EXP: {quest[CurrentQuest].experienceReward}";
        
        PlayerController.Instance.LockPlayerMovement(true);

    }

    public void AcceptQuest()
    {
        if (CurrentQuest >= quest.Length)
        {
            MenuManager.Instance.OpenMenu("UIPanel");
            PlayerController.Instance.LockPlayerMovement(false);
            return;
        }
        MenuManager.Instance.OpenMenu("UIPanel");
        quest[CurrentQuest].isActive = true;
        playerController.quest = quest[CurrentQuest];
        
        PlayerController.Instance.LockPlayerMovement(false);
    }

}
