using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public static QuestGiver Instance;
    
    public Quest[] quest;
    //public PlayerController playerController;

    public TextMeshProUGUI questName;
    public TextMeshProUGUI questDescription;
    public TextMeshProUGUI questExperience;

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
            //PlayerController.Instance.LockPlayerMovement(false);
            OnUnlockPlayerMovementEvent?.Invoke();
            return;
        }
        MenuManager.Instance.OpenMenu("UIPanel");
        quest[CurrentQuest].isActive = true;
        //PlayerController.Instance.quest = quest[CurrentQuest];
        _view.RPC(nameof(GiveQuestPlayers), RpcTarget.AllBuffered);
        
        //PlayerController.Instance.LockPlayerMovement(false);
        OnUnlockPlayerMovementEvent?.Invoke();
    }

    [PunRPC]
    private void GiveQuestPlayers()
    {
        OnGiveQuestEvent?.Invoke(quest[CurrentQuest]);
    }

    public Quest GetCurrentQuest()
    {
        if (CurrentQuest >= quest.Length)
            return null;
            
        return quest[CurrentQuest];
    }
    

}
