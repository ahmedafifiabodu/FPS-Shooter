using UnityEngine;

[CreateAssetMenu(fileName = "Weapon State Data", menuName = "Scriptable Object/Weapon/Weapon Data", order = 1)]
public class WeaponStateData : ScriptableObject
{
    [System.Serializable]
    public class WeaponPose
    {
        [Header("Transform Settings")]
        public Vector3 positionOffset;

        public Vector3 EulerOffset;
        public float TransformSmoothDampTime = 0.1f;

        [Header("Pose-Specific Movement Bounce Settings")]
        public float MovementBounceStrength_Horizontal = 1.0f;

        public float MovementBounceStrength_Vertical = 1.0f;
        [Min(0f)] public float MovementBounceSpeed = 12f;
    }

    [Header("General Movement Bounce Settings")]
    [Min(0f)] public float MovementBounceVelocityLimit = 3.0f;

    [Min(0f)] public float MovementBounceSpringStiffness = 250.0f;
    [Min(0f)] public float MovementBounceSpringDamping = 30.0f;

    public WeaponPose idlePose = new();
    public WeaponPose runPose = new();
    public WeaponPose aimPose = new();
    public WeaponPose reloadPose = new();
}