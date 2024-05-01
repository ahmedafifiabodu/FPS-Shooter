using UnityEngine;

public class EnemyCheckForTrigger : MonoBehaviour
{
    [SerializeField] private BasicEnemyAI enemyAI;
    [SerializeField] private EnemyNavMesh enemyNavMeshAI;

    private void OnTriggerEnter(Collider other)
    {
        if (enemyAI != null)
            if (other.gameObject.CompareTag("Player"))
                PlayerHealth.Instance.TakeDamage(enemyAI.damage);

        if (enemyNavMeshAI != null)
            if (other.gameObject.CompareTag("Player"))
                PlayerHealth.Instance.TakeDamage(enemyNavMeshAI.damage);
    }
}