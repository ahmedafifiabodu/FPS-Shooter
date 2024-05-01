using UnityEngine;

public class Bullet : MonoBehaviour
{
    internal int damage;

    public static Bullet Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start() => Destroy(gameObject, 5f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.TryGetComponent(out EnemyAI temp))
                temp.TakeDamage(damage);
            else if (other.gameObject.TryGetComponent(out BasicEnemyAI temp2))
                temp2.TakeDamage(damage);
            else if (other.gameObject.TryGetComponent(out FlyingEnemy temp3))
                temp3.TakeDamage(damage);
            else if (other.gameObject.TryGetComponent(out EnemyNavMesh temp4))
                temp4.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}