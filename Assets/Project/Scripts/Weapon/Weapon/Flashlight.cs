using UnityEngine;

public class Flashlight : Weapon
{
    [Header("Animator")]
    [SerializeField]
    private Animator _animator;

    private void Awake() => _weaponDamage = 0;

    internal override void Equip(Weapon weaponToEquip)
    {
        _animator.SetTrigger("isFlashlight");

        _fistCollider1.enabled = false;
        _fistCollider2.enabled = false;

        PlayerMotor.Instance.EquipWeapon(weaponToEquip);
    }

    internal override void Fire()
    { }

    internal override void Reload()
    { }
}