using UnityEngine;

public class MiniMapIcons : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private float fixedY;

    private void Start() => fixedY = transform.position.y;

    private void Update()
    {
        Vector3 position = playerTransform.position;
        position.y = fixedY;
        transform.position = position;
    }
}