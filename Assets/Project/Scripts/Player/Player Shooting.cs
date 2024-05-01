using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    internal Weapon _currentWeapon;

    internal List<Weapon> _weapons = new();
    private int _currentWeaponIndex = 0;

    public static PlayerShooting Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _weapons = GetComponentsInChildren<Weapon>().ToList();
    }

    private void Start()
    {
        EquipWeapon(_currentWeapon);
        _weapons.Add(_currentWeapon);
    }

    public void AddWeapon(Weapon weapon)
    {
        if (!_weapons.Contains(weapon))
            _weapons.Add(weapon);
    }

    public void CollectWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
        _weapons.Add(weapon);
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (_currentWeapon != null && _currentWeapon is not Fist)
            _currentWeapon.gameObject.SetActive(false);

        _currentWeapon = weapon;
        _currentWeapon.Equip(weapon);
        _currentWeapon.gameObject.SetActive(true);

        PlayerData.Instance.playerWeapons = weapon;
    }

    public void SwitchToNextWeapon()
    {
        if (Time.timeScale != 0)
        {
            _currentWeaponIndex = (_currentWeaponIndex + 1) % _weapons.Count;
            EquipWeapon(_weapons[_currentWeaponIndex]);
        }
    }

    public void SwitchToPreviousWeapon()
    {
        if (Time.timeScale != 0)
        {
            _currentWeaponIndex--;

            if (_currentWeaponIndex < 0)
                _currentWeaponIndex = _weapons.Count - 1;

            EquipWeapon(_weapons[_currentWeaponIndex]);
        }
    }

    internal void Fire()
    {
        _currentWeapon.Fire();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.shoot);
    }

    internal void Reload() => _currentWeapon.Reload();
}