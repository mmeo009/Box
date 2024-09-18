using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public List<TowerButton> buttons = new List<TowerButton>();
    public TMP_Text moneyText;
    public TMP_Text healthText;
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameManager.instance;
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
    }
    private void Update()
    {
        moneyText.text = string.Format("{0:#,###}", gameManager.playerData.inGameMoney);
        healthText.text = string.Format("{0}/{1}", gameManager.gameController.hp, gameManager.gameController.maxHp);
    }
}
