using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerStamina : MonoBehaviour
{
    [SerializeField]
    private Image staminaBar;

    [SerializeField]
    internal float maxStamina = 100f;

    [SerializeField]
    internal float stamina = 100f;

    private const float staminaDecreaseRate = 20f;
    private const float staminaRegenRate = 10f;
    public PlayerMotor playerMotor;

    public static PlayerStamina Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void UseStamina(float amount)
    {
        stamina -= amount * staminaDecreaseRate * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        UpdateStaminaBar();
    }

    public void RegenerateStamina()
    {
        if (!playerMotor.isJumping)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            UpdateStaminaBar();
        }
    }

    private void UpdateStaminaBar() => staminaBar.fillAmount = stamina / maxStamina;
}