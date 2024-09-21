using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStageManager : MonoBehaviour
{
    public List<TowerButton> buttons = new List<TowerButton>();
    public TMP_Text moneyText, healthText;
    public GameObject gameClear, gameOver;
    public Button clearButton, resetButton;
    private GameManager gameManager;
    private void OnEnable()
    {
        GameManager.OnInGameMoneyChanged += UpdateInGameMoney;
        GameManager.OnHealthChanged += UpdateHealth;
        GameManager.OnGameOver += GameOver;
        GameManager.OnGameClear += GameClear;
    }
    private void OnDisable()
    {
        GameManager.OnInGameMoneyChanged -= UpdateInGameMoney;
        GameManager.OnHealthChanged -= UpdateHealth;
        GameManager.OnGameOver -= GameOver;
        GameManager.OnGameClear -= GameClear;
    }
    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.maxHp = 10;
        gameManager.hp = 10;
        gameManager.playerData.inGameMoney = 80;
        gameManager.inGameMonyTimer.Reset();
        gameManager.gameSpeed = CatBoxUtils.Enums.GameSpeed.Default;

        clearButton.onClick.AddListener(() => SceneManager.LoadScene("StageSelectScene"));
        resetButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        for (int i = 0; i < buttons.Count; i++)
        {
            if (gameManager.playerData.myTowersIUse.Count < i)
            {
                buttons[i].gameObject.SetActive(false);
            }
            else
            {
                buttons[i].ChangeButtonData(gameManager.playerData.myTowersIUse[i]);
            }
        }
        UpdateInGameMoney();
        UpdateHealth();
    }
    private void UpdateInGameMoney()
    {
        moneyText.text = string.Format("{0:#,###}", gameManager.playerData.inGameMoney);
    }
    private void UpdateHealth()
    {
        healthText.text = string.Format("{0}/{1}", gameManager.hp, gameManager.maxHp);
    }
    private void GameOver()
    {
        gameManager.gameSpeed = CatBoxUtils.Enums.GameSpeed.Pause;
        gameOver.SetActive(true);
    }
    private void GameClear()
    {
        gameManager.gameSpeed = CatBoxUtils.Enums.GameSpeed.Pause;
        gameClear.SetActive(true);
    }
}
