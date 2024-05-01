using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerLook))]
[RequireComponent(typeof(PlayerShooting))]
public class InputManager : MonoBehaviour
{
    private PlayerMovement _playerInput;
    internal PlayerMovement.OnFootActions _onFoot;

    [SerializeField]
    private PlayerMotor _playerMotor;

    [SerializeField]
    private PlayerLook _playerLook;

    [SerializeField]
    private PlayerShooting _playerShooting;

    private void Awake()
    {
        _playerInput = new PlayerMovement();

        _onFoot = _playerInput.OnFoot;

        _onFoot.Jump.performed += _ => _playerMotor.Jump();
        _onFoot.Look.performed += _ => _playerLook.ProcesLook(_.ReadValue<Vector2>());

        _onFoot.Crouch.performed += _ => _playerMotor.Crouch();

        _onFoot.Sprint.performed += _ => _playerMotor.Sprint(true);
        _onFoot.Sprint.performed += _ => _playerMotor.currentState = "running";

        _onFoot.Sprint.canceled += _ => _playerMotor.Sprint(false);
        _onFoot.Sprint.canceled += _ => _playerMotor.currentState = "idle";

        _onFoot.Fire.performed += _ => _playerShooting.Fire();
        _onFoot.Reload.performed += _ => _playerShooting.Reload();
        _onFoot.Reload.performed += _ => _playerMotor.currentState = "reloading";

        _onFoot.Zoom.performed += _ => _playerLook.ZoomIn();
        _onFoot.Zoom.performed += _ => _playerMotor.currentState = "aiming";
        _onFoot.Zoom.canceled += _ => _playerLook.ZoomOut();

        _onFoot.Switch.performed += ctx =>
        {
            var scroll = ctx.ReadValue<float>();

            if (scroll > 0)
                _playerShooting.SwitchToNextWeapon();
            else if (scroll < 0)
                _playerShooting.SwitchToPreviousWeapon();
        };
    }

    private void FixedUpdate() => _playerMotor.ProcessMove(_onFoot.Movement.ReadValue<Vector2>());

    private void LateUpdate() => _playerLook.ProcesLook(_onFoot.Look.ReadValue<Vector2>());

    private void OnEnable() => _onFoot.Enable();

    private void OnDisable() => _onFoot.Disable();
}