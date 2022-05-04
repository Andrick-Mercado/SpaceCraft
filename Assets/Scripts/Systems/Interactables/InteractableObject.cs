using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class InteractableObject : MonoBehaviour
{
    [Header("Dependencies")] [SerializeField]
    private TextMeshProUGUI objectName;

    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI inputPrompt;
    [SerializeField] private Image activationProgress;
    [SerializeField] private CanvasGroup promptGroup;
    [SerializeField] private RectTransform promptUI;

    [FormerlySerializedAs("ConfigSo")] [SerializeField]
    private InteractableConfigSO configSO;

    [Header("Settings")] [SerializeField] private float alphaSlewRate;
    [SerializeField] private bool AllowInteraction = true;
    [SerializeField] private UnityEvent OnInteractionCompleted;
    [SerializeField] private bool isKinematic;

    public bool CanInteract => AllowInteraction;
    private float _targetAlpha = 0f;
    private float _interactionProgress = 0f;

    private void Start()
    {
        objectName.text = configSO.name;
        description.text = configSO.Description;
        inputPrompt.text = "Press [E] to " + configSO.Verb;
        activationProgress.fillAmount = 0;
        activationProgress.gameObject.SetActive(false);
        promptGroup.alpha = 0f;
    }

    private void Update()
    {
        

        if (Camera.main == null) return;

        if (promptGroup.alpha != _targetAlpha)
        {
            promptGroup.alpha = Mathf.MoveTowards(promptGroup.alpha, _targetAlpha, alphaSlewRate * Time.deltaTime);
        }

        if (isKinematic)
        {
            
            return;
        }
        
        if (promptGroup.alpha > 0)
        {
            Transform camT = Camera.main.transform;
            promptUI.LookAt(camT.position, camT.up);
        }
    }

    public void StartLookingAt()
    {
        _targetAlpha = 1f;
    }

    public void StopLookingAt()
    {
        _targetAlpha = 0f;
    }

    public void ContinueLookingAt(bool performInteraction)
    {
        if (performInteraction )
        {
            if (configSO.InteractionTime <= 0 )
            {
                InteractionCompleted();
                return;
            }
            
            _interactionProgress += Time.deltaTime / configSO.InteractionTime;
            if (_interactionProgress >= 1f )
            {
                InteractionCompleted();
                return;                                
            }
            
            activationProgress.fillAmount = _interactionProgress;
            if (!activationProgress.gameObject.activeInHierarchy)
                activationProgress.gameObject.SetActive(true);
        }
        else if (configSO.InteractionTime > 0 )
        {
            activationProgress.fillAmount = _interactionProgress = 0f;
            activationProgress.gameObject.SetActive(false);
        }
    }

    private void InteractionCompleted()
    {
        if (configSO.Name == "Quest Getter")
        {
            Debug.Log("Open TH QuestBoard");
            //AllowInteraction = false;
            promptGroup.alpha = _targetAlpha = 0f;
            OnInteractionCompleted.Invoke();
            QuestGiver.Instance.OpenQuestWindow();
        }
        else if (configSO.Name == "Spaceship")
        {
            if (PlayerController.Instance.quest.title == "Deliver Package")
            {
                Debug.Log("Supplies turned in");
                AllowInteraction = false;
                promptGroup.alpha = _targetAlpha = 0f;
                OnInteractionCompleted.Invoke();
                tempScript.Instance.removeItemAmount("Birds", 1);
                tempScript.Instance.removeItemAmount("Flint", 3);
                PlayerController.Instance.quest.questGoal[0].DeliverPackage();
                if (PlayerController.Instance.quest.questGoal[0].IsReached())
                {
                    //do something else when Completed task
                    PlayerController.Instance.quest.Complete();
                    QuestGiver.Instance.CurrentQuest++;
                }
            }
            

        }
        else
        {
            Debug.Log("Collected item: " + configSO.Name);
            tempScript.Instance.UpdateInventory(configSO.Name);
            if (PlayerController.Instance.quest.isActive && PlayerController.Instance.quest.questGoal[0].goalType == GoalType.Gathering)
            {
                if (PlayerController.Instance.quest.title == "Collect Flint")
                {
                    PlayerController.Instance.quest.questGoal[0].ItemCollected();
                    if (PlayerController.Instance.quest.questGoal[0].IsReached())
                    {
                        //do something else when Completed task
                        PlayerController.Instance.quest.Complete();
                        QuestGiver.Instance.CurrentQuest++;
                    }
                }
            }
            else if (PlayerController.Instance.quest.isActive && PlayerController.Instance.quest.questGoal[0].goalType == GoalType.Kill)
            {
                if (PlayerController.Instance.quest.title == "Kill Birds")
                {
                    PlayerController.Instance.quest.questGoal[0].EnemyKilled();
                    if (PlayerController.Instance.quest.questGoal[0].IsReached())
                    {
                        //do something else when Completed task
                        PlayerController.Instance.quest.Complete();
                        QuestGiver.Instance.CurrentQuest++;
                    }
                }
            }
            else if (PlayerController.Instance.quest.isActive && PlayerController.Instance.quest.questGoal[0].goalType == GoalType.Deliver)
            {
                if (PlayerController.Instance.quest.title == "Deliver Package")
                {
                    PlayerController.Instance.quest.questGoal[0].DeliverPackage();
                    if (PlayerController.Instance.quest.questGoal[0].IsReached())
                    {
                        //do something else when Completed task
                        PlayerController.Instance.quest.Complete();
                        QuestGiver.Instance.CurrentQuest++;
                        
                    }
                }
            }
            
            AllowInteraction = false;
            promptGroup.alpha = _targetAlpha = 0f;
            OnInteractionCompleted.Invoke();
            Destroy(gameObject);
        }
    }
}
