using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatBoxUtils;
using TMPro;
using UnityEngine.UI;
public class TowerButton : MonoBehaviour
{
    public Enums.TowerButtonType towerButtonType;
    public TowerObject towerObject;
    public Button button;
    private void Start()
    {
        TryGetComponent<Button>(out button);
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(() => GameManager.instance.gameController.CreateTower(towerObject, Vector3.zero));
    }
    public void ChangeButtonData(TowerObject newTowerObject)
    {
        towerObject = newTowerObject;

        if (newTowerObject == null)
        {
            button.GetComponentInChildren<TMP_Text>().text = "NULL";
            return;
        }

        button.onClick.AddListener(() => GameManager.instance.gameController.CreateTower(towerObject, Vector3.zero));
        button.GetComponentInChildren<TMP_Text>().text = towerObject.towerName + "\n" + ((towerButtonType == Enums.TowerButtonType.InGame) ? towerObject.costInGame : towerObject.costInStore);
    }
}
