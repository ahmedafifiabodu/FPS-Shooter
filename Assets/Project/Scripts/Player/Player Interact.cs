using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerUI))]
public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _maxDistance = 5f;

    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private PlayerUI _playerUI;

    [SerializeField]
    private InputManager _inputManager;

    private void InteractableUpdate()
    {
        _playerUI.UpdateText(string.Empty);
        Ray ray = new(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, _maxDistance, _layerMask))
        {
            if (hitInfo.collider.TryGetComponent<Interactable>(out var _interactable))
            {
                _playerUI.UpdateText(_interactable._promptMessage);

                if (_inputManager._onFoot.Interact.triggered)
                {
                    _interactable.BaseInteract();
                }
            }
        }
    }

    private void Update() => InteractableUpdate();
}