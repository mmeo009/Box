using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerButtonManager : MonoBehaviour
{
    public List<TowerButton> buttons = new List<TowerButton>();
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
    void Update()
    {
        
    }
}
