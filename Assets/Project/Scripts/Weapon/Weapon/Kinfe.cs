using UnityEngine;

public class Kinfe : Weapon
{
    [Header("Animator")]
    [SerializeField] private Animator _animator;

    [SerializeField] private float _fireDelay = 0.4f;
    [SerializeField] private int _knifeDamage = 10;

    private float _lastFireTime;
    private bool _isFirstAttack = true;
    private bool isAttacking = false;

    private void Awake() => _weaponDamage = _knifeDamage;

    internal override void Equip(Weapon weaponToEquip)
    {
        _animator.SetTrigger("isKnife");

        _fistCollider1.enabled = false;
        _fistCollider2.enabled = false;

        PlayerMotor.Instance.EquipWeapon(weaponToEquip);
    }

    internal override void Fire()
    {
        if (PlayerStamina.Instance.stamina > 25)
        {
            PlayerStamina.Instance.UseStamina(StaminaCost);

            if (Time.time - _lastFireTime < _fireDelay)
            {
                if (_isFirstAttack)
                {
                    _animator.SetTrigger("Attack 1");
                    _isFirstAttack = false;
                }
                else
                {
                    _animator.SetTrigger("Attack 2");
                    _isFirstAttack = true;
                }
            }
            else
            {
                _animator.SetTrigger("Attack 1");
                _isFirstAttack = false;
            }

            isAttacking = true;
            _lastFireTime = Time.time;
        }
        else if (PlayerStamina.Instance.stamina == 0)
        {
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.TryGetComponent(out EnemyAI temp))
                temp.TakeDamage(_knifeDamage);
            else if (other.gameObject.TryGetComponent(out BasicEnemyAI temp2))
                temp2.TakeDamage(_knifeDamage);
            else if (other.gameObject.TryGetComponent(out FlyingEnemy temp3))
                temp3.TakeDamage(_knifeDamage);
            else if (other.gameObject.TryGetComponent(out EnemyNavMesh temp4))
                temp4.TakeDamage(_knifeDamage);

            isAttacking = false;
        }
    }

    internal override void Reload()
    { }
}