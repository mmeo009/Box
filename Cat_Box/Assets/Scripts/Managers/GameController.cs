using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class GameController : MonoBehaviour
{
    public int maxHp = 10;
    public int hp = 10;

    public Timer inGameMonyTimer = new Timer(0.5f);
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
        var tower = PoolManager.Instance.SpawnFromPool("Tower", position, Quaternion.identity);
        tower.GetComponent<TowerController>().OnCreated(towerObject);
        GameManager.instance.playerData.inGameMoney -= towerObject.costInGame;
    }
}
