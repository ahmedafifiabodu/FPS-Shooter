using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Fist Colliders")]
    [SerializeField]
    internal SphereCollider _fistCollider1;

    [SerializeField]
    internal SphereCollider _fistCollider2;

    [Header("Weapon Data")]
    public WeaponStateData weaponData;

    public int StaminaCost;

    public static Weapon Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    protected int _weaponDamage;

    internal abstract void Equip(Weapon weaponToEquip);

    internal abstract void Fire();

    internal abstract void Reload();

    protected void HitEnemy(EnemyAI enemy) => enemy.TakeDamage(_weaponDamage);

    public string GetCurrentWeaponName() => gameObject.name;
}