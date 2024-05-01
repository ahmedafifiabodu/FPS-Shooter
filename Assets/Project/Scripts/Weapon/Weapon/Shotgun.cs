using UnityEngine;

public class Shotgun : Weapon
{
    [Header("Animator")]
    [SerializeField] private Animator _playerAnimator;

    [SerializeField] private Animator _shotgunAnimator;

    [Header("Bullet")]
    [SerializeField] private Camera _camera;

    [SerializeField] protected GameObject bulletPrefab;

    [SerializeField] protected float shootingForce;

    [SerializeField] private Transform bulletStartLocation;

    [SerializeField] private int _shotgunDamage = 10;

    [Header("Particle Effect")]
    [SerializeField] private ParticleSystem _muzzleFlash;

    internal override void Equip(Weapon weaponToEquip)
    {
        _playerAnimator.SetTrigger("isShotgun");

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

            _shotgunAnimator.SetTrigger("isShotgun");
            _shotgunAnimator.SetTrigger("Shoot");

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

                Bullet.Instance.damage = _shotgunDamage;
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
        _shotgunAnimator.SetTrigger("Reload");
    }
}