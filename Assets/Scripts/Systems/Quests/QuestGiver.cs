using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public static QuestGiver Instance;
    
    [Header("Dependencies")]
    [SerializeField]
    private TextMeshProUGUI questName;
    [SerializeField]
    private TextMeshProUGUI questDescription;
    [SerializeField]
    private TextMeshProUGUI questExperience;
    [SerializeField]
    private TextMeshProUGUI questDisplay;
    
    [Header("Quest Info")]
    [SerializeField]
    private Quest[] quest;
    
    
    [HideInInspector] public int CurrentQuest;
    
    public delegate void LockPlayerMovementEvent();
    public event LockPlayerMovementEvent OnLockPlayerMovementEvent;
    
    public delegate void UnlockPlayerMovementEvent();
    public event UnlockPlayerMovementEvent OnUnlockPlayerMovementEvent;

    public delegate void GiveQuestEvent(Quest quest);
    public event GiveQuestEvent OnGiveQuestEvent;


    private PhotonView _view;

    private void Awake()
    {
        Instance = this;
        _view = GetComponent<PhotonView>();
    }

    public void OpenQuestWindow()
    {
        if (!PhotonNetwork.IsMasterClient && !_view.IsMine) return;
        
        if (CurrentQuest >= quest.Length)
        {
            MenuManager.Instance.OpenMenu("questMenu");
            questName.text = "You Done";
            questDescription.text = "You Finished all the available quests";
            questExperience.text = "";
            //PlayerController.Instance.LockPlayerMovement(true);
            OnLockPlayerMovementEvent?.Invoke();
            return;
        }

        MenuManager.Instance.OpenMenu("questMenu");
        OnLockPlayerMovementEvent?.Invoke();
        questName.text = quest[CurrentQuest].title;
        questDescription.text = quest[CurrentQuest].description;
        questExperience.text = $"EXP: {quest[CurrentQuest].experienceReward}";
        //PlayerController.Instance.LockPlayerMovement(true);
        //OnLockPlayerMovementEvent?.Invoke();

    }

    public void AcceptQuest()
    {
        if (CurrentQuest >= quest.Length)
        {
            MenuManager.Instance.OpenMenu("UIPanel");
            
            OnUnlockPlayerMovementEvent?.Invoke();
            questDisplay.text = "+ Finished All Available Quests";
            return;
        }
        MenuManager.Instance.OpenMenu("UIPanel");
        
        
        _view.RPC(nameof(GiveQuestPlayers), RpcTarget.AllBuffered);

        OnUnlockPlayerMovementEvent?.Invoke();
    }

    [PunRPC]
    private void GiveQuestPlayers()
    {
        UncrossTextQuest();
        quest[CurrentQuest].isActive = true;
        
        if (quest[CurrentQuest].questGoal[0].goalType == GoalType.Deliver)
        {
            questDisplay.text = "+ Quest Deliver items to spaceship!";
        }
        else if (quest[CurrentQuest].questGoal[0].goalType == GoalType.Kill)
        {
            questDisplay.text = $"+ Quest Kill {quest[CurrentQuest].questGoal[0].requiredAmount} Birds";
        }
        else
        {
            questDisplay.text = $"+ Quest Gather {quest[CurrentQuest].questGoal[0].requiredAmount} " 
                                + quest[CurrentQuest].questGoal[0].goalType.ToString().Remove(0,6);
        }
        
        OnGiveQuestEvent?.Invoke(quest[CurrentQuest]);
    }

    public Quest GetCurrentQuest()
    {
        if (CurrentQuest >= quest.Length)
            return null;
            
        return quest[CurrentQuest];
    }

    public void CrossTextQuest()
    {
        questDisplay.fontStyle = FontStyles.Bold | FontStyles.Strikethrough | FontStyles.Italic;
        //questDisplay.text = "<s color=#ff800080>"+questDisplay.text + "</s>";
    }

    public void UncrossTextQuest()
    {
        questDisplay.fontStyle = FontStyles.Bold |  FontStyles.Italic;
    }
    

}
