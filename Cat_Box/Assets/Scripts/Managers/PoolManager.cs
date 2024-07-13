using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Pool
{
    public string tag;              // pool�� ������ tag
    public Transform poolObject;   // ��Ȱ��ȭ�� ������Ʈ���� ��ƵѰ�
    public GameObject prefab;       // ������ ������Ʈ
    public int size;                // pool�� �ִ� ������
    public Queue<GameObject> gameObjects = new Queue<GameObject>();
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] private List<Pool> pools = new List<Pool>();                      // ������ pool���� ������ ���� ��� ����� List

    public Dictionary<string, Pool> poolDictionary;    // pool�� ã�� ���ϰ� Dictionary

    void Start()
    {
        poolDictionary = new Dictionary<string, Pool>();   // ��ųʸ� ����

        foreach (Pool pool in pools)                                    // List�� �ִ� Ǯ���� ������
        {
            CreatePool(pool);
        }
    }

    public void CreatePool(Pool pool)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();         // FIFO (���� ���¾ָ� ���� ����) ���� �Ẹ�� �; ����غ�

        GameObject parentObject = new GameObject($"@{pool.tag}_Pool");
        pool.poolObject = parentObject.transform;

        for (int i = 0; i < pool.size; i++)                             // ���� �Է��ص� ������ ��ŭ
        {
            GameObject obj = Instantiate(pool.prefab);                  // ������Ʈ�� ����
            obj.name = pool.tag;                                        // ������Ʈ�� �̸� ����
            obj.SetActive(false);                                       // ��Ȱ��ȭ ��Ŵ
            obj.transform.parent = pool.poolObject;                     // ��������� �־��
            objectPool.Enqueue(obj);                                    // Queue�� �߰���
        }

        pool.gameObjects = objectPool;                                  // pool Ŭ������ Queue�� �ٲ���

        poolDictionary.Add(pool.tag, pool);                             // Dictionary�� �߰���
    }

    public void AddNewPool(string tag, GameObject prefab, int size = 10)            // ���� �� Ǯ�� ���� ��� (�⺻ ������� 10)
    {
        if(poolDictionary.ContainsKey(tag))                                         // ���� �ش� �ױ��� Ǯ�� ������ ���
        {
            Debug.LogWarning($"{tag}�� ���� pool�� �̹� ���� �մϴ�.");               // ���� �޼��� ���
            return; 
        }

        Pool newPool = new Pool { tag = tag, prefab = prefab, size = size };
        pools.Add(newPool);                                                         // ����Ʈ���� �߰�
        CreatePool(newPool);                                                        // Ǯ�� ����
    }

    public void ReSizePool(string tag, int additionalSize)                          // Ǯ�� ����� �ø� ���
    {
        Pool pool = poolDictionary[tag];                            // tag�� ���� Ǯ�� ������
        pool.size += additionalSize;                                // ����� �ø�

        Queue<GameObject> objectPool = new Queue<GameObject>();     // Queue�� ���� ����

        for (int i = 0; i < additionalSize; i++)                    // �ø� ������ ��ŭ ������Ʈ�� �����ؼ� Queue�� �߰���
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.name = pool.tag;
            obj.SetActive(false);
            obj.transform.parent = pool.poolObject;                 
            objectPool.Enqueue(obj);
        }

        foreach(var obj in pool.gameObjects)                        // ���� ���� Queue�� �ִ� ������Ʈ���� �߰���
        {
            objectPool.Enqueue(obj);
        }

        pool.gameObjects = objectPool;                              // ���� ���� Queue�� �ٲ�
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))                       // ���� tag�� ���� pool�� �������� ���� ���
        {
            Debug.LogWarning($"{tag}�� Ǯ�� �����ϴ�!");             // ��� �޼��� ���
            return null;
        }

        Pool pool = poolDictionary[tag];                            // tag�� ���� Ǯ�� ������

        GameObject objectToSpawn = pool.gameObjects.Dequeue();      // �ش� �ױ׸� ���� Queue���� ������Ʈ�� ������
        
        if(objectToSpawn.activeSelf == true)                        // ���� ã�ƿ� ������Ʈ�� Ȱ��ȭ�� ������ ���
        {
            ReSizePool(tag, pool.size * 2);                         // Ǯ ������ �ι�� �ø�
            objectToSpawn = pool.gameObjects.Dequeue();             // Queue���� ������Ʈ�� �ٽ� ������
        }

        objectToSpawn.SetActive(true);                              // ������Ʈ�� Ȱ��ȭ ��Ŵ
        objectToSpawn.transform.parent = null;                      // ���� �������� ��
        objectToSpawn.transform.position = position;                // ��ġ�� �̵�
        objectToSpawn.transform.rotation = rotation;                // ��ü�� ȸ��

        pool.gameObjects.Enqueue(objectToSpawn);                    // Queue ���������� �̵� ��Ŵ

        return objectToSpawn;                                       // ������ ������Ʈ�� ��ȯ��
    }

    public void ReturnToPool(GameObject obj)                        // ������Ʈ�� �� Ȱ��ȭ �ϴ� �Լ�
    {
        obj.transform.parent = poolDictionary[obj.name].poolObject;
        obj.SetActive(false);
    }

    public bool HasThisPool(string tag)                             // �ش� tag�� pool�� �����ϴ��� ��ȯ�ϴ� �Լ�(�־ ��� �׸��� �Լ��� �� ���������?)
    {
        return poolDictionary.ContainsKey(tag);
    }
}