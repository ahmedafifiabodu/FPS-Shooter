using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private int _health;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] internal int damage = 10;
    [SerializeField] private Transform[] points;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float delayBetweenPoints = 0.5f;

    private float timer;
    private int destPoint = 0;
    private bool _isDead = false;

    private void Start()
    {
        _health = 100;
        agent.autoBraking = false;
        timer = delayBetweenPoints;
        GotoNextPoint();
    }

    private void GotoNextPoint()
    {
        if (points.Length == 0)
            return;

        if (timer < delayBetweenPoints)
        {
            timer += Time.deltaTime;
        }
        else
        {
            agent.destination = points[destPoint].position;
            destPoint = (destPoint + 1) % points.Length;
            timer = 0;
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

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep the enemy's rotation on the y-axis unchanged
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }

    private void Update()
    {
        healthText.text = "Enemy Health: " + _health;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isAttacking", true);
                LookAtPlayer();
            }
            else
            {
                agent.isStopped = false;
                agent.destination = player.position;
                _animator.SetBool("isWalking", true);
                _animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            agent.isStopped = false;
            if (!agent.pathPending && agent.remainingDistance < 0.1f)
                GotoNextPoint();

            if (agent.velocity != Vector3.zero)
                _animator.SetBool("isWalking", true);
            else
                _animator.SetBool("isWalking", false);

            _animator.SetBool("isAttacking", false);
        }
    }
}