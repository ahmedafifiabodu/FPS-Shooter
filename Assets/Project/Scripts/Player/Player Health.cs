using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField]
    private int _maxHealth = 5;

    [SerializeField]
    private int _currentHealth = 5;

    [SerializeField]
    private GameObject _healthSprite;

    [SerializeField]
    private GameObject _maxHealthSprite;

    [SerializeField]
    private Transform _UICanvas;

    private readonly List<GameObject> _healthSprites = new();

    [Header("Damage Overlay")]
    [SerializeField]
    private Image _damageOverlay;

    [SerializeField]
    private float _damageOverlayDuration = 5f;

    [SerializeField]
    private float _damageOverlayFadeSpeed = 5f;

    private float _damageOverlayDurationTimer;
    private Vector3 respawnPoint;

    public static PlayerHealth Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        _currentHealth = _maxHealth;
    }

    private void Start()
    {
        _damageOverlay.color = new Color(_damageOverlay.color.r, _damageOverlay.color.g, _damageOverlay.color.b, 0);
        respawnPoint = transform.position;
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint) => respawnPoint = newRespawnPoint;

    public void AddHealthSprite()
    {
        GameObject newHealthSprite = Instantiate(_healthSprite, _UICanvas);
        _healthSprites.Add(newHealthSprite);
    }

    public void RemoveHealthSprite()
    {
        if (_healthSprites.Count > 0)
        {
            GameObject lastHealthSprite = _healthSprites[^1];
            _healthSprites.RemoveAt(_healthSprites.Count - 1);
            Destroy(lastHealthSprite);
        }
    }

    public void ReplaceLastHealthSpriteWithMax()
    {
        if (_healthSprites.Count > 0)
        {
            GameObject lastHealthSprite = _healthSprites[^1];
            _healthSprites.RemoveAt(_healthSprites.Count - 1);
            Destroy(lastHealthSprite);

            GameObject newMaxHealthSprite = Instantiate(_maxHealthSprite, _UICanvas);
            _healthSprites.Add(newMaxHealthSprite);
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        _damageOverlayDurationTimer = 0;
        _damageOverlay.color = new Color(_damageOverlay.color.r, _damageOverlay.color.g, _damageOverlay.color.b, 1);

        for (int i = 0; i < damage; i++)
            RemoveHealthSprite();

        if (_currentHealth <= 0)
        {
            PlayerMotor.Instance._characterController.enabled = false;
            PlayerMotor.Instance._characterController.transform.position = respawnPoint;
            PlayerMotor.Instance._characterController.enabled = true;

            Heal(_maxHealth);
        }
    }

    public void Heal(int healAmount)
    {
        int oldHealth = _currentHealth;
        _currentHealth += healAmount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

        int actualHealing = _currentHealth - oldHealth;

        for (int i = 0; i < actualHealing; i++)
            AddHealthSprite();

        if (_currentHealth == _maxHealth)
            ReplaceLastHealthSpriteWithMax();
    }

    private void Update()
    {
        if (_damageOverlay.color.a > 0)
        {
            if (_currentHealth <= _maxHealth / 3)
                return;

            _damageOverlayDurationTimer += Time.deltaTime;
            if (_damageOverlayDurationTimer > _damageOverlayDuration)
            {
                float alpha = _damageOverlay.color.a;
                alpha -= _damageOverlayFadeSpeed * Time.deltaTime;
                _damageOverlay.color = new Color(_damageOverlay.color.r, _damageOverlay.color.g, _damageOverlay.color.b, alpha);
            }
        }
    }

    internal int GetCurrentHealth() => _currentHealth;

    internal Vector3 GetRespawnPoint() => respawnPoint;

    internal void SetHealth(int health)
    {
        _currentHealth = health;

        foreach (var healthSprite in _healthSprites)
            Destroy(healthSprite);

        _healthSprites.Clear();

        for (int i = 0; i < _currentHealth; i++)
            AddHealthSprite();

        if (_currentHealth == _maxHealth)
            ReplaceLastHealthSpriteWithMax();
    }
}