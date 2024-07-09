using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class GameManager : MonoBehaviour
{
    public Enums.GameState gameState;       // 게임의 상태
    public Enums.GameSpeed gameSpeed = Enums.GameSpeed.Default;     // 게임의 속도

    public static GameManager instance;     // 싱글톤
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
