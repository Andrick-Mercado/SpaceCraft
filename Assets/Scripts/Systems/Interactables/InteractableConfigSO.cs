using UnityEngine;

[CreateAssetMenu(menuName = "Interactables/Interactable Config", fileName = "Interactable Config")]
public class InteractableConfigSO : ScriptableObject
{
    public string Name;
    public string Description;
    public string Verb;
    public float InteractionTime;
}
