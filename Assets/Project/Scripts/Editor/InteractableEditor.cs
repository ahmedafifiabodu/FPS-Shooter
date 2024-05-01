using UnityEditor;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactable _interactable = (Interactable)target;

        if (target.GetType() == typeof(EventOnlyInteractables))
        {
            _interactable._promptMessage = EditorGUILayout.TextField("Prompt Message", _interactable._promptMessage);
            EditorGUILayout.HelpBox("This interactable will only use events.", MessageType.Info);

            if (_interactable.gameObject.GetComponent<InteractableEvents>() == null)
            {
                _interactable._useEvents = true;
                _interactable.gameObject.AddComponent<InteractableEvents>();
            }
        }
        else
        {
            base.OnInspectorGUI();

            if (_interactable._useEvents)
            {
                if (_interactable.gameObject.GetComponent<InteractableEvents>() == null)
                    _interactable.gameObject.AddComponent<InteractableEvents>();
            }
            else
            {
                if (_interactable.gameObject.GetComponent<InteractableEvents>() != null)
                    DestroyImmediate(_interactable.gameObject.GetComponent<InteractableEvents>());
            }
        }
    }
}