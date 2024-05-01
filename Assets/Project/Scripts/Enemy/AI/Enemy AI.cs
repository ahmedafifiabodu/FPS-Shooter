using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _bulletTranform;
    [SerializeField] private float _bulletForce = 10;

    private Transform[] _points;
    private int _destPoint = 0;
    private float _speed;
    private float _fov;
    private bool _waiting = false;

    private LayerMask _playerLayer;
    private Transform _playerTransform;

    private int _health;
    private bool _isDead = false;

    private GameObject _bulletPrefab;
    private int _bulletDamage;
    private readonly float _fireRate = 1f;
    private float _nextFire = 0f;
    private bool _isAttacking = false;

    public TextMeshProUGUI healthText;

    private void Awake()
    {
        if (EnemyManager.EnemyAIInstances.Contains(this))
            Destroy(gameObject);
        else
            EnemyManager.EnemyAIInstances.Add(this);

        _playerTransform = PlayerMotor.Instance.transform;
    }

    private void Start() => StartCoroutine(GotoNextPoint());

    private IEnumerator GotoNextPoint()
    {
        if (_points.Length == 0)
            yield break;

        yield return new WaitForSeconds(2);
        _destPoint = (_destPoint + 1) % _points.Length;

        _waiting = false;
    }

    private void Update()
    {
        healthText.text = "Enemy Health: " + _health;

        if (_points.Length == 0 || _waiting)
            return;

        if (!_isAttacking)
        {
            Vector3 target = _points[_destPoint].position;
            Vector3 directionToTarget = (new Vector3(target.x, transform.position.y, target.z) - transform.position).normalized;

            if (directionToTarget != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
                float rotationSpeed = 180;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, target) < 0.5f)
            {
                _waiting = true;
                StartCoroutine(GotoNextPoint());
                _animator.SetBool("isWalking", false);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);
                _animator.SetBool("isWalking", true);
            }
        }

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), transform.forward, out _, _fov, _playerLayer))
        {
            if (Time.time > _nextFire)
            {
                _nextFire = Time.time + _fireRate;
                Shoot();

                _isAttacking = true;

                _animator.SetBool("isShooting", true);
                _animator.SetBool("isWalking", false);
            }
        }
        else
        {
            _isAttacking = false;
            _animator.SetBool("isShooting", false);
        }
    }

    private void Shoot()
    {
        GameObject bulletInstance = Instantiate(_bulletPrefab, _bulletTranform.position, Quaternion.identity);
        EnemyBullet.Instance.damage = _bulletDamage;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.shoot);

        Vector3 directionToPlayer = (_playerTransform.position - _bulletTranform.position).normalized;
        bulletInstance.GetComponent<Rigidbody>().velocity = directionToPlayer * _bulletForce;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead)
            return;

        _health -= damage;

        if (_health <= 0)
        {
            _health = 0;
            _isDead = true;

            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDie);

            _animator.SetTrigger("isDead");
            Destroy(gameObject, _animator.GetCurrentAnimatorStateInfo(0).length);
            PlayerScore.Instance.IncreaseScore(1);
        }
    }

    public void SetPatrolPointsAndParameters(Transform[] patrolPoints, float speed, float fov, GameObject bulletPrefab, int bulletDamage, int health, LayerMask playerLayer)
    {
        _points = patrolPoints;
        _speed = speed;
        _fov = fov;
        _bulletPrefab = bulletPrefab;
        _bulletDamage = bulletDamage;
        _health = health;
        _playerLayer = playerLayer;
    }
}