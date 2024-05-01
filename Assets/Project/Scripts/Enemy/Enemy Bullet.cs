using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    internal int damage = 1;

    public static EnemyBullet Instance { get; private set; }

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
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth.Instance.TakeDamage(damage);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHit);
            Destroy(gameObject);
        }
    }
}