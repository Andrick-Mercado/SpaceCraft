using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class InteractableObject : MonoBehaviour
{
    [Header("Dependencies")] 
    [SerializeField] private TextMeshProUGUI objectName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI inputPrompt;
    [SerializeField] private Image activationProgress;
    [SerializeField] private CanvasGroup promptGroup;
    [SerializeField] private RectTransform promptUI;

    [FormerlySerializedAs("ConfigSo")] [SerializeField]
    private InteractableConfigSO configSO;

    [Header("Settings")] [SerializeField] private float alphaSlewRate;
    [SerializeField] bool AllowInteraction = true;
    [SerializeField] private UnityEvent OnInteractionCompleted;

    public bool CanInteract => AllowInteraction;
    private float _targetAlpha = 0f;
    private float _interactionProgress = 0f;
    private PhotonView _view;
    private void Awake()
    {
        _view = GetComponent<PhotonView>();
    }

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
                _view.RPC(nameof(InteractionCompleted), RpcTarget.AllBuffered);
                return;
            }
            
            _interactionProgress += Time.deltaTime / configSO.InteractionTime;
            if (_interactionProgress >= 1f )
            {
                _view.RPC(nameof(InteractionCompleted), RpcTarget.AllBuffered);
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

    [PunRPC]
    private void InteractionCompleted()
    {
        Debug.Log("Interacted with: "+ configSO.Name);
        
        //only added to our inventory if its an inventory item and delete it afterwards
        if (TryGetComponent<ItemObject>(out ItemObject itemObject))
        {
            itemObject.OnHandlePickupItem();
            AllowInteraction = false;
            promptGroup.alpha = _targetAlpha = 0f;
            OnInteractionCompleted.Invoke();
            Destroy(gameObject);
        }
        
        //here we can add other interaction systems
    }
}
