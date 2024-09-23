using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using CatBoxUtils;

[System.Serializable]
public class PlayerData
{
    public int Money;                   // �������� Ÿ���� ��� ��
    public int inGameMoney;             // �����߿� �־����� ��ȭ �� �������� ���� �ʱ�ȭ ��
    public List<TowerObject> myTowers = new List<TowerObject>();        // ���� ������ �ִ� Ÿ����
    public List<TowerObject> myTowersIUse = new List<TowerObject>();    // ���� ����ϴ� Ÿ����
    public int Reroll;                  // ���� �ʱ�ȭ ��ȸ
}
[System.Serializable]
public class LevelData
{
    public int level;
    public int index;
    public string sceneName;
    public int tryCount;
    public int clearCount;
}
public class GameManager : MonoBehaviour
{
    public Enums.GameState gameState = Enums.GameState.MAIN;       // ������ ����
    public Enums.GameSpeed gameSpeed = Enums.GameSpeed.Default;     // ������ �ӵ�

    public PlayerData playerData = new PlayerData { Money = 0, inGameMoney = 0, Reroll = 1 };   // �÷��̾�

    private string key = "����̴»��ڸ�������";  // ��ȣȭ Ű
    public string saveFilePath = null;          // ���̺� ���

    // ���ӿ� �����ϴ� Ÿ����
    public List<TowerObject> towers = new List<TowerObject>();

    public static GameManager instance;     // �̱���

    public static event Action OnInGameMoneyChanged;
    public static event Action OnMoneyChanged;
    public static event Action OnHealthChanged;
    public static event Action OnGameClear;
    public static event Action OnGameOver;

    public int maxHp = 10;
    public int hp = 10;

    public Timer inGameMonyTimer = new Timer(0.5f);
    public List<TowerButton> inGameTowerButton = new List<TowerButton>();
    private TowerController lastTower;

    private void Awake()
    {
        if (instance == null)
        {
            transform.parent = null;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }
    }
    private void Start()
    {
        if(gameState == Enums.GameState.MAIN)
        {

        }
    }

    private void Update()
    {
        if(gameState == Enums.GameState.GAMEPLAY)
        {
            float deltaTime = Time.deltaTime;
            inGameMonyTimer.Update(deltaTime, gameSpeed);

            if (!inGameMonyTimer.IsRunning())
            {
                ChangeMoney(Enums.MoneyType.INGAME, 1);
                inGameMonyTimer.Start();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                gameSpeed = Enums.GameSpeed.Pause;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                gameSpeed = Enums.GameSpeed.Slow;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                gameSpeed = Enums.GameSpeed.Default;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                gameSpeed = Enums.GameSpeed.Fast;
            }
        }
    }
    public void ChangeMoney(Enums.MoneyType moneyType, int amount)
    {
        if(moneyType == Enums.MoneyType.INGAME)
        {
            playerData.inGameMoney += amount;

            if (playerData.inGameMoney <= 0)
            {
                playerData.inGameMoney = 0;
            }
            OnInGameMoneyChanged?.Invoke();
        }
        else if(moneyType == Enums.MoneyType.INSTORE)
        {
            playerData.Money += amount;

            if (playerData.Money <= 0)
            {
                playerData.Money = 0;
            }
            OnMoneyChanged?.Invoke();
        }
        else
        {
            return;
        }
    }
    public void StageClear()
    {
        if(maxHp - hp > 5)
        {
            playerData.Money += EnemyManager.Instance.enemyDatas.Count + maxHp - hp;
        }
        else
        {
            playerData.Money += EnemyManager.Instance.enemyDatas.Count;
        }
        OnGameClear?.Invoke();
    }

    public void GetDamage(int damage)
    {
        if (gameState == Enums.GameState.GAMEPLAY)
        {
            hp -= damage;
            OnHealthChanged?.Invoke();
            if(hp <= 0)
            {
                OnGameOver?.Invoke();
            }
        }
    }

    public void CreateTower(TowerObject towerObject, Vector3 position)
    {
        if (GameManager.instance.playerData.inGameMoney < towerObject.costInGame)
        {
            return;
        }

        if (lastTower != null)
        {
            if (lastTower.isActiveAndEnabled)
            {
                if (lastTower.merge.isFirst)
                {
                    return;
                }
            }
        }

        var tower = PoolManager.Instance.SpawnFromPool("Tower", new Vector3(position.x, position.y + 0.7f, position.z), Quaternion.identity);
        lastTower = tower.GetComponent<TowerController>();
        lastTower.OnCreated(towerObject);

        ChangeMoney(Enums.MoneyType.INGAME, towerObject.costInGame * -1);
    }

    // ���� ������ ����
    private void SaveGmaeData(PlayerData data)
    {
        saveFilePath = Application.persistentDataPath + "/CatBoxSave.json";
        // JSON ����ȭ
        string jsonData = JsonConvert.SerializeObject(data);

        // �����͸� ����Ʈ �迭�� ��ȯ
        byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(jsonData);

        // ��ȣȭ
        byte[] encryptedBytes = Encrypt(bytesToEncrypt);

        // ��ȣȭ�� �����͸� Base64 ���ڿ��� ��ȯ
        string encryptedData = Convert.ToBase64String(encryptedBytes);

        // ���� ����
        File.WriteAllText(saveFilePath, encryptedData);
    }

    // ���� ������ �ҷ�����
    private PlayerData LoadData()
    {
        saveFilePath = Application.persistentDataPath + "/CatBoxSave.json";

        if (File.Exists(saveFilePath))
        {
            // ���Ͽ��� ��ȣȭ�� ������ �б�
            string encryptedData = File.ReadAllText(saveFilePath);

            // Base64���ڿ��� ����Ʈ �迭�� ��ȯ
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            // ��ȣȭ
            byte[] decryptedBytes = Decrypt(encryptedBytes);

            // byte �迭�� ���ڿ��� ��ȯ
            string jsonData = Encoding.UTF8.GetString(decryptedBytes);

            // JSON ���� �� ����ȭ
            PlayerData data = JsonConvert.DeserializeObject<PlayerData>(jsonData);
            return data;
        }
        else
        {
            return null;
        }
    }

    // ������ ��ȣȭ
    private byte[] Encrypt(byte[] plainBytes)
    {
        // AES ��ȣȭ �˰��� ����
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = AdjustKeySize(key, 256); // 256 ��Ʈ (32 ����Ʈ)�� Ű ũ�� ����
            aesAlg.IV = new byte[16];   // �ʱ�ȭ ����

            // ��ȣȭ ��ȯ�� ����
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // �޸� ��Ʈ�� ����
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // ��ȣȭ ��Ʈ�� ����
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    // ������ ����
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                    csEncrypt.FlushFinalBlock();

                    // ��ȣȭ�� �����͸� ����Ʈ �迭�� ��ȯ
                    return msEncrypt.ToArray();
                }
            }
        }
    }

    // ������ ��ȣȭ
    private byte[] Decrypt(byte[] encryptedBytes)
    {
        // AES ��ȣȭ �˰��� ����
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = AdjustKeySize(key, 256); // 256 ��Ʈ (32 ����Ʈ)�� Ű ũ�� ����
            aesAlg.IV = new byte[16];   // �ʱ�ȭ ����

            // ��ȣȭ ��ȯ�� ����
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // �޸� ��Ʈ�� ����
            using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
            {
                // ��ȣȭ ��Ʈ�� ����
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    // ��ȣȭ�� �����͸� ���� ����Ʈ �迭 ����
                    byte[] decryptedBytes = new byte[encryptedBytes.Length];

                    // ������ �б�
                    int decryptedByteCount = csDecrypt.Read(decryptedBytes, 0, decryptedBytes.Length);

                    // ������ ���� ũ�� ��ŭ�� ����Ʈ �迭�� ��ȯ
                    return decryptedBytes.Take(decryptedByteCount).ToArray();
                }
            }
        }
    }

    // ������ ������ ����
    private byte[] AdjustKeySize(string key, int keySize)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);          // Ű�� ����Ʈ �迭�� ��ȯ
        Array.Resize(ref keyBytes, keySize / 8);                // ���ϴ� Ű ũ�⿡ �°� �迭 ũ�� ����
        return keyBytes;                                        // ������ ����Ʈ�� ��ȯ
    }
}
