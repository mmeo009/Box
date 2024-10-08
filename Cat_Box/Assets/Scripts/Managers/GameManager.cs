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
    public int Money;                   // 상점에서 타워를 사는 돈
    public int inGameMoney;             // 게임중에 주어지는 재화 매 스테이지 마다 초기화 됨
    public List<TowerObject> myTowers = new List<TowerObject>();        // 내가 가지고 있는 타워들
    public List<TowerObject> myTowersIUse = new List<TowerObject>();    // 내가 사용하는 타워들
    public int Reroll;                  // 상점 초기화 기회
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
    public Enums.GameState gameState = Enums.GameState.MAIN;       // 게임의 상태
    public Enums.GameSpeed gameSpeed = Enums.GameSpeed.Default;     // 게임의 속도

    public PlayerData playerData = new PlayerData { Money = 0, inGameMoney = 0, Reroll = 1 };   // 플레이어

    private string key = "고양이는상자를조아해";  // 암호화 키
    public string saveFilePath = null;          // 세이브 경로

    // 게임에 등장하는 타워들
    public List<TowerObject> towers = new List<TowerObject>();

    public static GameManager instance;     // 싱글톤

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

    // 게임 데이터 저장
    private void SaveGmaeData(PlayerData data)
    {
        saveFilePath = Application.persistentDataPath + "/CatBoxSave.json";
        // JSON 직렬화
        string jsonData = JsonConvert.SerializeObject(data);

        // 데이터를 바이트 배열로 변환
        byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(jsonData);

        // 암호화
        byte[] encryptedBytes = Encrypt(bytesToEncrypt);

        // 암호화된 데이터를 Base64 문자열로 변환
        string encryptedData = Convert.ToBase64String(encryptedBytes);

        // 파일 저장
        File.WriteAllText(saveFilePath, encryptedData);
    }

    // 게임 데이터 불러오기
    private PlayerData LoadData()
    {
        saveFilePath = Application.persistentDataPath + "/CatBoxSave.json";

        if (File.Exists(saveFilePath))
        {
            // 파일에서 암호화된 데이터 읽기
            string encryptedData = File.ReadAllText(saveFilePath);

            // Base64문자열을 바이트 배열로 변환
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            // 복호화
            byte[] decryptedBytes = Decrypt(encryptedBytes);

            // byte 배열을 문자열로 변환
            string jsonData = Encoding.UTF8.GetString(decryptedBytes);

            // JSON 파일 역 직렬화
            PlayerData data = JsonConvert.DeserializeObject<PlayerData>(jsonData);
            return data;
        }
        else
        {
            return null;
        }
    }

    // 데이터 암호화
    private byte[] Encrypt(byte[] plainBytes)
    {
        // AES 암호화 알고리즘 생성
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = AdjustKeySize(key, 256); // 256 비트 (32 바이트)로 키 크기 조정
            aesAlg.IV = new byte[16];   // 초기화 벡터

            // 암호화 변환기 생성
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // 메모리 스트림 생성
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // 암호화 스트림 생성
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    // 데이터 쓰기
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                    csEncrypt.FlushFinalBlock();

                    // 암호화된 데이터를 바이트 배열로 반환
                    return msEncrypt.ToArray();
                }
            }
        }
    }

    // 데이터 복호화
    private byte[] Decrypt(byte[] encryptedBytes)
    {
        // AES 복호화 알고리즘 생성
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = AdjustKeySize(key, 256); // 256 비트 (32 바이트)로 키 크기 조정
            aesAlg.IV = new byte[16];   // 초기화 벡터

            // 복호화 변환기 생성
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // 메모리 스트림 생성
            using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
            {
                // 복호화 스트림 생성
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    // 복호화된 데이터를 담을 바이트 배열 생성
                    byte[] decryptedBytes = new byte[encryptedBytes.Length];

                    // 데이터 읽기
                    int decryptedByteCount = csDecrypt.Read(decryptedBytes, 0, decryptedBytes.Length);

                    // 실제로 읽힌 크기 만큼의 바이트 배열을 반환
                    return decryptedBytes.Take(decryptedByteCount).ToArray();
                }
            }
        }
    }

    // 데이터 사이즈 조절
    private byte[] AdjustKeySize(string key, int keySize)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);          // 키를 바이트 배열로 변환
        Array.Resize(ref keyBytes, keySize / 8);                // 원하는 키 크기에 맞게 배열 크기 조정
        return keyBytes;                                        // 조정된 바이트를 반환
    }
}
