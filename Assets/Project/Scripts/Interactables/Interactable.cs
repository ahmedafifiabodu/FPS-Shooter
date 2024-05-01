using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool _useEvents;
    public string _promptMessage;

    protected virtual string OnLook() => _promptMessage;

    internal void BaseInteract()
    {
        if (_useEvents)
        {
            if (gameObject.TryGetComponent<InteractableEvents>(out var _events))
                _events.onInteract.Invoke();
        }
        else
            Interact();
    }

    protected virtual void Interact() => Debug.Log($"(Virtual) Interacting with {gameObject.name}");
}