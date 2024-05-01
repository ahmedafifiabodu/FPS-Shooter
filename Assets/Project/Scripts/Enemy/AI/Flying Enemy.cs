using TMPro;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Enemy Settings")]
    [SerializeField] private Transform[] moveSpots;

    [SerializeField] private int _health;
    [SerializeField] private float speed;
    [SerializeField] private float startWaitTime;
    [SerializeField] private float laserForce;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float playerDetectionRange;
    [SerializeField] private float evadeDistance;
    [SerializeField] private float attackRange;

    [Header("Enemy State (Only For Debugging)")]
    [SerializeField] private float timeSinceLastShot = 0f;

    [SerializeField] private int randomSpot;
    [SerializeField] private float waitTime;
    [SerializeField] private EnemyState state;
    [SerializeField] private bool _isDead = false;

    private void Start()
    {
        waitTime = startWaitTime;
        randomSpot = Random.Range(0, moveSpots.Length);
        state = EnemyState.Wandering;
    }

    private void Update()
    {
        healthText.text = "Health: " + _health;

        CheckPlayerDistance();

        switch (state)
        {
            case EnemyState.Wandering:
                Wandering();
                break;

            case EnemyState.Pursuing:
                Pursuing();
                break;

            case EnemyState.Attacking:
                Attacking();
                break;

            case EnemyState.Evading:
                Evading();
                break;
        }
    }

    private void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > playerDetectionRange)
        {
            state = EnemyState.Wandering;
        }
        else if (distanceToPlayer > attackRange)
        {
            state = EnemyState.Pursuing;
        }
        //else if (distanceToPlayer <= evadeDistance)
        //{
        //    state = EnemyState.Evading;
        //}
        else
        {
            state = EnemyState.Attacking;
        }
    }

    private void Wandering()
    {
        Vector3 direction = (moveSpots[randomSpot].position - transform.position).normalized;
        direction = -direction;

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, toRotation) < 0.1f)
                transform.position = Vector3.MoveTowards(transform.position, moveSpots[randomSpot].position, speed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, moveSpots[randomSpot].position) < 0.2f)
        {
            if (waitTime <= 0)
            {
                randomSpot = (randomSpot + 1) % moveSpots.Length;
                waitTime = startWaitTime;
            }
            else
                waitTime -= Time.deltaTime;
        }
    }

    private void Pursuing()
    {
        Vector3 targetPosition = new(player.position.x, player.position.y + 1, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer = -directionToPlayer;

        Quaternion lookAtPlayer = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtPlayer, Time.deltaTime * speed);
    }

    private void Attacking()
    {
        Debug.Log("Attacking");
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer = -directionToPlayer;

        Quaternion lookAtPlayer = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtPlayer, Time.deltaTime * speed);

        if (Vector3.Dot(transform.forward, directionToPlayer) > 0.9f)
        {
            timeSinceLastShot += Time.deltaTime;
            if (timeSinceLastShot >= attackCooldown)
            {
                GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.LookRotation(-directionToPlayer));

                if (laser.TryGetComponent<Rigidbody>(out var laserRigidbody))
                {
                    laserRigidbody.AddForce(-directionToPlayer * laserForce, ForceMode.Impulse);
                    timeSinceLastShot = 0f;
                }
            }
        }
    }

    private void Evading()
    {
        Vector3 directionFromPlayer = (transform.position - player.position).normalized;
        Vector3 evadePosition = transform.position + directionFromPlayer * evadeDistance;
        transform.position = Vector3.MoveTowards(transform.position, evadePosition, speed * Time.deltaTime);
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

            Destroy(gameObject, 1);
            PlayerScore.Instance.IncreaseScore(1);
        }
    }
}