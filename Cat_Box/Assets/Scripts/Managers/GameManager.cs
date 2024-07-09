using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class GameManager : MonoBehaviour
{
    public Enums.GameState gameState;       // ������ ����
    public Enums.GameSpeed gameSpeed = Enums.GameSpeed.Default;     // ������ �ӵ�

    public static GameManager instance;     // �̱���
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }


}
