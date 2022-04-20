using UnityEngine;

public class InteractableScanner : MonoBehaviour
{
    [SerializeField] private float InteractionRange;
    [SerializeField] private LayerMask interactionMask = ~0;

    [SerializeField] private KeyCode interactionKeyCode;

    private InteractableObject _currentInteractable;
    private bool _Internal_PerformInteract;
    private float startTime = 0f;
    private float holdTime = 5.0f;

    private void Update()
    {
        // if (Camera.main == null)
        //     return;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,out var hitResult, InteractionRange,
                interactionMask, QueryTriggerInteraction.Collide))
        {
            var candidateInteractable = hitResult.collider.GetComponent<InteractableObject>();

            // if we can't interact then null
            if (candidateInteractable != null && !candidateInteractable.CanInteract)
                candidateInteractable = null;

            _Internal_PerformInteract = Input.GetKey(interactionKeyCode);
            
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
        //OnInteract();
        _currentInteractable.ContinueLookingAt(_Internal_PerformInteract);
    }
    
    
}
