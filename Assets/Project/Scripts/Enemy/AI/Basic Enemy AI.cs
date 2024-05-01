using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicEnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [SerializeField] private CharacterController playerRb;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Settings")]
    [SerializeField] private List<Transform> waypoints;

    [SerializeField] private int _health;
    [SerializeField] internal int damage = 10;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float stoppingDistanceToAttack = 2.0f;
    [SerializeField] private float stoppingDistanceToMove = 0.1f;
    [SerializeField] private float predictionTime = 1.0f;

    [SerializeField]
    private EnemyState currentState;

    private Transform currentWaypoint;

    private bool _isDead = false;

    private void Start()
    {
        _health = 100;
        currentState = EnemyState.Wandering;
    }

    private void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
    }

    private void Wondering(List<Transform> waypoints)
    {
        if (waypoints.Count == 0) return;

        if (currentWaypoint == null || Vector3.Distance(transform.position, currentWaypoint.position) < stoppingDistanceToMove)
            currentWaypoint = waypoints[Random.Range(0, waypoints.Count)];

        Seeking(currentWaypoint);
    }

    private void Seeking(Transform target)
    {
        //Vector3 direction = (target.position - transform.position).normalized;
        //direction.y = 0;
        //Quaternion lookRotation = Quaternion.LookRotation(direction);

        //transform.SetPositionAndRotation(
        //    Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime),
        //    Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed));

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        RotateTowardsTarget(target);

        _animator.SetBool("isWalking", true);
        _animator.SetBool("isAttacking", false);
    }

    private void Pursuing(Transform target, Vector3 targetVelocity, float stoppingDistance)
    {
        _animator.SetBool("isAttacking", false);

        if (stoppingDistance > stoppingDistanceToAttack)
        {
            Vector3 futurePosition = target.position + targetVelocity * predictionTime;

            //Vector3 direction = (futurePosition - transform.position).normalized;
            //direction.y = 0;
            //Quaternion lookRotation = Quaternion.LookRotation(direction);

            //transform.SetPositionAndRotation(
            //    Vector3.MoveTowards(transform.position, futurePosition, speed * Time.deltaTime),
            //    Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed));

            Vector3 direction = (futurePosition - transform.position).normalized;
            direction.y = 0;

            transform.position = Vector3.MoveTowards(transform.position, futurePosition, speed * Time.deltaTime);

            RotateTowardsTarget(target);

            _animator.SetBool("isWalking", true);
        }
        else
        {
            currentState = EnemyState.Attacking;
            _animator.SetBool("isWalking", false);
        }
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

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (currentState == EnemyState.Attacking)
            {
                RotateTowardsTarget(player);
                _animator.SetBool("isAttacking", true);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void CheckCurrentState(float distanceToPlayer)
    {
        if (currentState != EnemyState.Attacking)
            transform.localScale = Vector3.one;

        if (currentState == EnemyState.Attacking && distanceToPlayer > 3.0f)
        {
            currentState = EnemyState.Pursuing;
            StopCoroutine(AttackRoutine());
        }
        else if (currentState != EnemyState.Attacking)
        {
            if (distanceToPlayer <= 6.0f)
            {
                currentState = EnemyState.Pursuing;
                StopCoroutine(AttackRoutine());
            }
            else
            {
                currentState = EnemyState.Wandering;
                StopCoroutine(AttackRoutine());
            }
        }
        else if (currentState == EnemyState.Attacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void FixedUpdate()
    {
        healthText.text = "Enemy Health: " + _health;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Wandering:
                Wondering(waypoints);
                break;

            case EnemyState.Pursuing:
                Pursuing(player, playerRb.velocity, distanceToPlayer);
                break;

            case EnemyState.Attacking:
                break;

            default:
                break;
        }

        CheckCurrentState(distanceToPlayer);
    }
}