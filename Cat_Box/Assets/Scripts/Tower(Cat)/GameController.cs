using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;

public class GameController : MonoBehaviour
{
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
    public void CreateTower(TowerObject towerObject, Transform transform)
    {
        var tower = PoolManager.Instance.SpawnFromPool("Tower", transform.position, transform.rotation);
        tower.GetComponent<TowerController>().OnCreated(towerObject);
        GameManager.instance.playerData.inGameMoney -= towerObject.costInGame;
    }
}
