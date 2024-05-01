using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyPatrolData[] enemyPatrolData;
    internal static List<EnemyAI> EnemyAIInstances { get; set; }

    public static EnemyManager Instance { get; private set; }

    private int counter;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        counter = 0;

        EnemyAIInstances = new List<EnemyAI>();
    }

    private void Start()
    {
        for (int i = 0; i < enemyPatrolData.Length; i++)
        {
            var data = enemyPatrolData[i];
            string key = data.enemyPrefab.name + "_" + i;

            data.LoadParameters(key);

            Instantiate(data.enemyPrefab, data.patrolPoints[0].position, Quaternion.identity, data._enemyTransform);

            EnemyAIInstances[counter].
                SetPatrolPointsAndParameters(data.patrolPoints, data.speed, data.fov, data.bulletPrefab,
                data.bulletDamage, data.health, data.playerLayer);

            counter++;

            //data.SaveParameters(key);
        }
    }
}

[System.Serializable]
public class EnemyPatrolData
{
    [Header("Enemy Patrol Data")]
    public GameObject enemyPrefab;

    public Transform _enemyTransform;
    public Transform[] patrolPoints;

    [Header("Enemy AI Parameters (These Parameters are being loaded)")]
    public int level;

    public int health;

    public float speed;

    public float fov;

    [Header("Enemy Bullet Parameters")]
    public GameObject bulletPrefab;

    public EnemyBullet bullet;
    public int bulletDamage;

    [Header("Enemy Bullet Collision")]
    public LayerMask playerLayer;

    public void SaveParameters(string key)
    {
        PlayerPrefs.SetInt(key + "_level", level);
        PlayerPrefs.SetInt(key + "_health", health);
        PlayerPrefs.SetFloat(key + "_speed", speed);
        PlayerPrefs.SetFloat(key + "_fov", fov);
    }

    public void LoadParameters(string key)
    {
        level = PlayerPrefs.GetInt(key + "_level", level);
        health = PlayerPrefs.GetInt(key + "_health", health);
        speed = PlayerPrefs.GetFloat(key + "_speed", speed);
        fov = PlayerPrefs.GetFloat(key + "_fov", fov);
    }
}