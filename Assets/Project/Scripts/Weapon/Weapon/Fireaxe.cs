using UnityEngine;

public class Fireaxe : Weapon
{
    [Header("Animator")]
    [SerializeField] private Animator _animator;

    [SerializeField] private int _FireaxeDamage = 10;
    [SerializeField] private float _fireDelay = 0.4f;

    private float _lastFireTime;
    private bool _isFirstAttack = true;
    private bool isAttacking = false;

    private void Awake() => _weaponDamage = _FireaxeDamage;

    internal override void Equip(Weapon weaponToEquip)
    {
        _animator.SetTrigger("isFireaxe");

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
                temp.TakeDamage(_FireaxeDamage);
            else if (other.gameObject.TryGetComponent(out BasicEnemyAI temp2))
                temp2.TakeDamage(_FireaxeDamage);
            else if (other.gameObject.TryGetComponent(out FlyingEnemy temp3))
                temp3.TakeDamage(_FireaxeDamage);
            else if (other.gameObject.TryGetComponent(out EnemyNavMesh temp4))
                temp4.TakeDamage(_FireaxeDamage);

            isAttacking = false;
        }
    }

    internal override void Reload()
    { }
}