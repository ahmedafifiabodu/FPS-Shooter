using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(PlayerStamina))]
public class PlayerMotor : MonoBehaviour
{
    [Category("Character Controller")]
    [SerializeField]
    internal CharacterController _characterController;

    [SerializeField]
    private Transform fpsHands;

    private Vector3 baseHandPosition;
    private Vector2 movementBouncePositionOffset = Vector2.zero;

    [Category("Movement")]
    [SerializeField]
    private float speed = 5f;

    private float normalSpeed;
    private float crippledSpeed;

    private Vector3 _velocity;
    private bool _isGrounded;
    private readonly float _gravity = -9.81f;

    private bool _lerpCrouch = false;
    private float _crouchTimer = 0f;
    private bool _isCrouching = false;

    private bool _isSprinting = false;

    [Category("Jumping")]
    [SerializeField]
    private float _jumpHeight = 3f;

    [SerializeField]
    internal bool isJumping = false;

    [SerializeField]
    private int jumpCount = 0;

    [Category("Stamina")]
    [SerializeField]
    private PlayerStamina playerStamina;

    private Weapon currentWeapon;
    internal string currentState;

    public static PlayerMotor Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        normalSpeed = speed;
        crippledSpeed = speed / 2f;

        baseHandPosition = fpsHands.localPosition;
    }

    public void EquipWeapon(Weapon newWeapon) => currentWeapon = newWeapon;

    private void UpdateChecker()
    {
        _isGrounded = _characterController.isGrounded;

        if (!_characterController.isGrounded)
            _velocity.y += _gravity * Time.deltaTime;

        if (_lerpCrouch)
        {
            _crouchTimer += Time.deltaTime;
            float p = _crouchTimer / 1f;
            //p = p * p * (3f - 2f * p);
            p *= p;
            if (_isCrouching)
                _characterController.height = Mathf.Lerp(_characterController.height, 1f, p);
            else
                _characterController.height = Mathf.Lerp(_characterController.height, 2f, p);

            if (p > 1f)
            {
                _lerpCrouch = false;
                _crouchTimer = 0f;
            }
        }

        if (_isGrounded)
        {
            isJumping = false;
            jumpCount = 0;

            if (_isSprinting)
            {
                playerStamina.UseStamina(1);
                if (playerStamina.stamina <= 0)
                {
                    Sprint(true);
                    speed = crippledSpeed;
                }
            }
            else
            {
                playerStamina.RegenerateStamina();

                if (playerStamina.stamina >= playerStamina.maxStamina)
                    speed = normalSpeed;
            }
        }
    }

    internal void ProcessMove(Vector2 _input)
    {
        Vector3 moveDirection = transform.right * _input.x + transform.forward * _input.y;
        moveDirection.y = 0;

        moveDirection.Normalize();

        Vector3 horizontalVelocity = moveDirection * speed;

        _velocity.y += Physics.gravity.y * Time.deltaTime;

        Vector3 finalVelocity = horizontalVelocity + _velocity;

        _characterController.Move(finalVelocity * Time.deltaTime);

        if (_characterController.isGrounded)
            _velocity.y = 0;
    }

    public float GetMoveVelocityMagnitude()
    {
        if (_characterController.isGrounded == false)
            return 0f;

        Vector3 horizontalVelocity = _characterController.velocity;
        horizontalVelocity.y = 0;
        return horizontalVelocity.magnitude;
    }

    internal void Jump()
    {
        if (jumpCount < 1 && playerStamina.stamina >= 10)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            playerStamina.UseStamina(20);
            isJumping = true;
            jumpCount++;
        }
    }

    internal void Crouch()
    {
        if (playerStamina.stamina >= 10)
        {
            _isCrouching = !_isCrouching;

            if (_isCrouching)
                speed /= 2f;
            else
                speed *= 2f;

            _crouchTimer = 0f;
            _lerpCrouch = true;

            playerStamina.UseStamina(10);
        }
    }

    internal void Sprint(bool isSprinting)
    {
        if (isSprinting && playerStamina.stamina > 0 && GetMoveVelocityMagnitude() > 0)
        {
            if (!_isSprinting)
            {
                _isSprinting = true;
                speed *= 2f;
            }
        }
        else
        {
            if (_isSprinting)
            {
                _isSprinting = false;
                speed = normalSpeed;
            }
        }
    }

    public void ApplyWeaponState(WeaponStateData weaponStateData, WeaponStateData.WeaponPose pose)
    {
        UpdateMovementBounce(weaponStateData, pose, Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(pose.EulerOffset);
        float maxDegreesDelta = pose.TransformSmoothDampTime * Time.deltaTime * 360;
        fpsHands.localRotation = Quaternion.RotateTowards(fpsHands.localRotation, targetRotation, maxDegreesDelta);

        Vector3 currentVelocity = Vector3.zero;
        fpsHands.localPosition = Vector3.SmoothDamp(fpsHands.localPosition, baseHandPosition + pose.positionOffset, ref currentVelocity, pose.TransformSmoothDampTime);
    }

    private void UpdateMovementBounce(WeaponStateData weaponStateData, WeaponStateData.WeaponPose pose, float deltaTime)
    {
        float sine = Mathf.Sin(Time.time * pose.MovementBounceSpeed);
        float cos = Mathf.Cos(Time.time * 0.5f * pose.MovementBounceSpeed);

        float moveVelocityMagnitude = GetMoveVelocityMagnitude();
        moveVelocityMagnitude = Mathf.Min(moveVelocityMagnitude, weaponStateData.MovementBounceVelocityLimit);
        moveVelocityMagnitude /= weaponStateData.MovementBounceVelocityLimit;

        movementBouncePositionOffset += moveVelocityMagnitude * new Vector2(
            deltaTime * ((0.5f - cos) * 2f) * pose.MovementBounceStrength_Horizontal,
            deltaTime * sine * pose.MovementBounceStrength_Vertical);

        Vector2 dampingForce =
            (weaponStateData.MovementBounceSpringStiffness * -movementBouncePositionOffset) -
            (weaponStateData.MovementBounceSpringDamping * deltaTime * movementBouncePositionOffset);

        movementBouncePositionOffset += deltaTime * dampingForce;

        fpsHands.localPosition += (Vector3)movementBouncePositionOffset;
    }

    private void Update() => UpdateChecker();

    private void FixedUpdate()
    {
        if (currentWeapon != null)
        {
            WeaponStateData.WeaponPose pose = currentState switch
            {
                "idle" => currentWeapon.weaponData.idlePose,
                "running" => currentWeapon.weaponData.runPose,
                "aiming" => currentWeapon.weaponData.aimPose,
                "reloading" => currentWeapon.weaponData.reloadPose,
                _ => currentWeapon.weaponData.idlePose,
            };

            ApplyWeaponState(currentWeapon.weaponData, pose);
        }
    }
}