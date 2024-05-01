using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public void CheckpointReached() => PlayerHealth.Instance.SetRespawnPoint(transform.position);
}