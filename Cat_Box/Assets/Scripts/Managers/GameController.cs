using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CatBoxUtils;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public int maxHp = 10;
    public int hp = 10;

    public Timer inGameMonyTimer = new Timer(0.5f);
    public List<TowerButton> inGameTowerButton = new List<TowerButton>();
    private TowerController lastTower;

    private void Start()
    {
        var buttons = FindObjectsOfType<TowerButton>();
        foreach(var button in buttons)
        {
            button.ChangeButtonData(null);
            inGameTowerButton.Add(button);
        }

        inGameTowerButton = inGameTowerButton.OrderBy(button => button.name.Last()).ToList();

        for(int i = 0; i < GameManager.instance.playerData.myTowersIUse.Count; i ++)
        {
            if(GameManager.instance.playerData.myTowersIUse[i] != null)
            {
                inGameTowerButton[i].ChangeButtonData(GameManager.instance.playerData.myTowersIUse[i]);
            }
        }
    }
    private void Update()
    {
        if (GameManager.instance.gameState == Enums.GameState.GAMEPLAY)
        {
            float deltaTime = Time.deltaTime;
            inGameMonyTimer.Update(deltaTime, GameManager.instance.gameSpeed);

            if (!inGameMonyTimer.IsRunning())
            {
                GameManager.instance.playerData.inGameMoney++;
                inGameMonyTimer.Start();
            }
        }
    }
    public void GetDamage(int damage)
    {
        hp -= damage;
    }
    public void CreateTower(TowerObject towerObject, Vector3 position)
    {
        if(GameManager.instance.playerData.inGameMoney < towerObject.costInGame)
        {
            return;
        }

        if(lastTower != null)
        {
            if(lastTower.isActiveAndEnabled)
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
        GameManager.instance.playerData.inGameMoney -= towerObject.costInGame;
    }
}
