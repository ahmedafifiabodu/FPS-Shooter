using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField, Min(0f)] private float _cameraHeight = 1.7f;

    [SerializeField] private float _cameraMouseSensitivity = 1f;
    [SerializeField] private float _cameraMinPitch = 0f;
    [SerializeField] private float _cameraMaxPitch = 0f;
    [SerializeField] private Transform _cameraTransform = null;
    private float _cameraPitch;
    private float _cameraYaw;

    [Header("Zoom Settings")]
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _zoomedFOV = 30f;

    [SerializeField]
    private float _zoomSpeed = 2f;

    private float _normalFOV;
    private float _targetFOV;

    private void Awake()
    {
        _normalFOV = _camera.fieldOfView;
        _targetFOV = _normalFOV;

        if (_cameraTransform != null)
        {
            Vector3 cameraEuler = _cameraTransform.rotation.eulerAngles;
            _cameraPitch = cameraEuler.x;
            _cameraYaw = cameraEuler.y;
        }
    }

    public void ZoomIn() => _targetFOV = _zoomedFOV;

    public void ZoomOut() => _targetFOV = _normalFOV;

    private void UpdateCameraRotation(Vector2 _inputMouseDelta)
    {
        _cameraYaw += _inputMouseDelta.x * _cameraMouseSensitivity;
        _cameraPitch += -_inputMouseDelta.y * _cameraMouseSensitivity;
        _cameraPitch = Mathf.Clamp(_cameraPitch, _cameraMinPitch, _cameraMaxPitch);

        if (_cameraYaw < -180) _cameraYaw += 360f;
        if (_cameraYaw > 180f) _cameraYaw -= 360f;

        if (_cameraPitch < -180f) _cameraPitch += 360f;
        if (_cameraPitch > 180f) _cameraPitch -= 360f;

        _cameraTransform.rotation = Quaternion.Euler(_cameraPitch, _cameraYaw, 0f);
        transform.rotation = Quaternion.Euler(_cameraPitch, _cameraYaw, 0f);

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition() => _cameraTransform.position = transform.position + _cameraHeight * transform.up;

    internal void ProcesLook(Vector2 _input)
    {
        if (Time.timeScale != 0)
            UpdateCameraRotation(_input);
    }

    private void Update() => _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetFOV, _zoomSpeed * Time.deltaTime);
}