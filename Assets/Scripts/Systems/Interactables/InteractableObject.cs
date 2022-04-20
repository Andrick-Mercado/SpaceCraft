using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI inputPrompt;
    [SerializeField] private Image activationProgress;
    [SerializeField] private CanvasGroup promptGroup;
    [SerializeField] private RectTransform promptUI;
    [SerializeField] private InteractableConfigSO ConfigSo;
    [SerializeField] private float alphaSlewRate;

    [SerializeField] bool AllowInteraction = true;
    [SerializeField] UnityEvent OnInteractionCompleted = new UnityEvent();
    
    private float _targetAlpha = 0f;
    private float InteractionProgress = 0f;

    public bool CanInteract => AllowInteraction;

    
    private void Start()
    {
        objectName.text = ConfigSo.name;
        description.text = ConfigSo.Description;
        inputPrompt.text = "Press [E] to " + ConfigSo.Verb;
        activationProgress.fillAmount = 0;
        activationProgress.gameObject.SetActive(false);
        promptGroup.alpha = 0f;
    }

    private void Update()
    {
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
        // are performing an interaction
        if (performInteraction)
        {
            // no interaction time set?
            if (ConfigSo.InteractionTime <= 0)
            {
                InteractionCompleted();
                return;
            }
        
            // update the progress, check if completed
            InteractionProgress += Time.deltaTime / ConfigSo.InteractionTime;
            if (InteractionProgress >= 1f)
            {
                InteractionCompleted();
                return;                                
            }
        
            // update the progress slider
            activationProgress.fillAmount = InteractionProgress;
            if (!activationProgress.gameObject.activeInHierarchy)
                activationProgress.gameObject.SetActive(true);
        }
        else if (ConfigSo.InteractionTime > 0)
        {
            activationProgress.fillAmount = InteractionProgress = 0f;
            activationProgress.gameObject.SetActive(false);
        }
    }

    private void InteractionCompleted()
    {
        Debug.Log("Collected item: "+ ConfigSo.Name);
        
        AllowInteraction = false;
        promptGroup.alpha = _targetAlpha = 0f;

        OnInteractionCompleted.Invoke();

        Destroy(gameObject);
    }
}
