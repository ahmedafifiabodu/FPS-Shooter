using UnityEngine;

public class Pistol : Weapon
{
    [Header("Animator")]
    [SerializeField] private Animator _playerAnimator;

    [SerializeField] private Animator _gunAnimator;

    [Header("Bullet")]
    [SerializeField] private Camera _camera;

    [SerializeField] protected GameObject bulletPrefab;

    [SerializeField] protected float shootingForce;

    [SerializeField] private Transform bulletStartLocation;

    [SerializeField] private int _pistolDamage = 10;

    [Header("Particle Effect")]
    [SerializeField] private ParticleSystem _muzzleFlash;

    private void Awake() => _weaponDamage = _pistolDamage;

    internal override void Equip(Weapon weaponToEquip)
    {
        _playerAnimator.SetTrigger("isPistol");

        _fistCollider1.enabled = false;
        _fistCollider2.enabled = false;

        PlayerMotor.Instance.EquipWeapon(weaponToEquip);
    }

    internal override void Fire()
    {
        if (PlayerStamina.Instance.stamina > 25)
        {
            PlayerStamina.Instance.UseStamina(StaminaCost);

            _playerAnimator.SetTrigger("Attack 1");

            _gunAnimator.SetTrigger("isGun");
            _gunAnimator.SetTrigger("Shoot");

            _playerAnimator.SetTrigger("Attack 1");

            _gunAnimator.SetTrigger("isGun");
            _gunAnimator.SetTrigger("Shoot");

            if (ShootingAnimation.isFinished == true)
            {
                Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                Vector3 targetPoint;

                if (Physics.Raycast(ray, out RaycastHit hit))

                    targetPoint = hit.point;
                else
                    targetPoint = ray.GetPoint(1000);

                Vector3 direction = (targetPoint - bulletStartLocation.position).normalized;
                GameObject bullet = Instantiate(bulletPrefab, bulletStartLocation.position, Quaternion.LookRotation(direction));
                bullet.GetComponent<Rigidbody>().AddForce(direction * shootingForce);

                ParticleSystem muzzleFlash = Instantiate(_muzzleFlash, bulletStartLocation.position, Quaternion.identity, bulletStartLocation);
                muzzleFlash.Play();

                Bullet.Instance.damage = _pistolDamage;
                ShootingAnimation.isFinished = false;
            }
        }
        else if (PlayerStamina.Instance.stamina == 0)
        {
            return;
        }
    }

    internal override void Reload()
    {
        _playerAnimator.SetTrigger("Reload");
        _gunAnimator.SetTrigger("Reload");
    }
}