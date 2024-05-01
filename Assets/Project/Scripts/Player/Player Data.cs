using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    internal Weapon playerWeapons;
    private string dataPath;
    public static PlayerData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            dataPath = Path.Combine(Application.dataPath, "playerData.json");
        }
    }

    [System.Serializable]
    private class SerializablePlayerData
    {
        public int health;
        public int score;
        public Vector3 position;
        public Vector3 respawnPoint;
        public string activeWeaponName;
        public List<string> collectedWeapons;
    }

    public void SavePlayerData()
    {
        string weaponName = playerWeapons.GetCurrentWeaponName();

        SerializablePlayerData data = new()
        {
            health = PlayerHealth.Instance.GetCurrentHealth(),
            score = PlayerScore.Instance.GetScore(),
            position = PlayerMotor.Instance.transform.position,
            respawnPoint = PlayerHealth.Instance.GetRespawnPoint(),
            activeWeaponName = weaponName,
            collectedWeapons = PlayerShooting.Instance._weapons.Select(weapon => weapon.GetCurrentWeaponName()).ToList()
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(dataPath, json);
    }

    public void LoadPlayerData()
    {
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            SerializablePlayerData data = JsonUtility.FromJson<SerializablePlayerData>(json);

            PlayerHealth.Instance.SetHealth(data.health);
            PlayerScore.Instance.SetScore(data.score);
            PlayerHealth.Instance.SetRespawnPoint(data.respawnPoint);

            PlayerMotor.Instance._characterController.enabled = false;
            PlayerMotor.Instance._characterController.transform.position = data.position;
            PlayerMotor.Instance._characterController.enabled = true;

            Weapon[] allWeapons = FindObjectsOfType<Weapon>(true);
            foreach (Weapon weapon in allWeapons)
            {
                string weaponName = weapon.GetCurrentWeaponName();

                if (data.collectedWeapons != null)
                    if (data.collectedWeapons.Contains(weaponName))
                        PlayerShooting.Instance.AddWeapon(weapon);
            }

            Weapon weaponToLoad = PlayerShooting.Instance._weapons.FirstOrDefault(weapon => weapon.GetCurrentWeaponName() == data.activeWeaponName);

            if (weaponToLoad != null)
                PlayerShooting.Instance.EquipWeapon(weaponToLoad);
        }
    }
}