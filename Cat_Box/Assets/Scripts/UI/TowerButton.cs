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
    public TMP_Text[] texts;
    public Image towerImage;
    private Dictionary<string, TMP_Text> textDictionary = new Dictionary<string, TMP_Text>();
    private void Awake()
    {
        if (!TryGetComponent<Button>(out button))
        {
            button = gameObject.AddComponent<Button>();
        }

        foreach(var text in texts)
        {
            textDictionary.Add(text.gameObject.name, text);
        }

        if(towerButtonType == Enums.TowerButtonType.InGame)
        {
            button.onClick.AddListener(() => GameManager.instance.CreateTower(towerObject, Vector3.zero));
        }
    }
    public void ChangeButtonData(TowerObject newTowerObject)
    {
        towerObject = newTowerObject;

        if (newTowerObject == null)
        {
            textDictionary["Name_Text"].text = "NULL";
            return;
        }

        if(towerButtonType == Enums.TowerButtonType.InGame)
        {
            button.onClick.AddListener(() => GameManager.instance.CreateTower(towerObject, Vector3.zero));
            textDictionary["Cost_Text"].text = towerObject.costInGame.ToString();
        }
        else
        {
            textDictionary["Cost_Text"].text = towerObject.costInGame.ToString() + "/" + towerObject.costInStore.ToString();
        }

        towerImage.sprite = towerObject.towerImage;
        textDictionary["AttackDamage_Text"].text = StstsByString(towerObject, Enums.TowerStatType.DAMAGE);
        textDictionary["AttackSpeed_Text"].text = StstsByString(towerObject, Enums.TowerStatType.ATTACKRATE);
        textDictionary["Range_Text"].text = StstsByString(towerObject, Enums.TowerStatType.RANGE);
        textDictionary["Name_Text"].text = towerObject.towerName;
    }

    private string StstsByString(TowerObject towerObject, Enums.TowerStatType statType)
    {
        string result = "";

        foreach(var stat in towerObject.tower)
        {
            switch(statType)
            {
                case Enums.TowerStatType.DAMAGE:
                    result += stat.baseDamage.ToString() + "/ ";
                    break;
                case Enums.TowerStatType.ATTACKRATE:
                    result += stat.baseAttackRate.ToString() + "/ ";
                    break;
                case Enums.TowerStatType.RANGE:
                    result += stat.baseRange.ToString() + "/ ";
                    break;
                default:
                    result += "00/ ";
                    break;
            }
        }
        return result.TrimEnd(' ').TrimEnd('/');
    }
}
