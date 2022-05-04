using UnityEngine;
using UnityEngine.Serialization;
using Photon.Pun;

public class InteractableScanner : MonoBehaviour
{
    [FormerlySerializedAs("InteractionRange")]
    [Header("Settings for Interactables")]
    [SerializeField] private float interactionRange;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private KeyCode interactionKeyCode;

    private InteractableObject _currentInteractable;
    private bool _internalPerformInteract;
    private void Update()
    {
        if (Camera.main == null) return;
        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,out var hitResult, interactionRange,
                interactionMask, QueryTriggerInteraction.Collide))
        {
                
            var candidateInteractable = hitResult.collider.GetComponent<InteractableObject>();

            // if we can't interact then null
            if (candidateInteractable != null && !candidateInteractable.CanInteract)
                candidateInteractable = null;

            _internalPerformInteract = Input.GetKey(interactionKeyCode);
            
            if (candidateInteractable != null && _currentInteractable == null)
            {
                OnStartLookingAt(candidateInteractable);
            }
            else if (candidateInteractable != null & candidateInteractable == _currentInteractable)
            {
                OnContinueLookingAt();
            }
            else if (candidateInteractable != null && _currentInteractable !=null)
            {
                OnStopLookingAt();
                OnStartLookingAt(candidateInteractable);
            }
            else if(candidateInteractable == null && _currentInteractable !=null)
            {
                OnStopLookingAt();
            }
        }
        else if (_currentInteractable != null)
        {
            OnStopLookingAt();
        }
    }

    private void OnStartLookingAt(InteractableObject interactable)
    {
        _currentInteractable = interactable;

        _currentInteractable.StartLookingAt();
    }

    private void OnStopLookingAt()
    {
        _currentInteractable.StopLookingAt();

        _currentInteractable = null;
    }
    
    private void OnContinueLookingAt()
    {
        _currentInteractable.ContinueLookingAt(_internalPerformInteract);
    }
    
    
}
