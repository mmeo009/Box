using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    public TMP_Text ownerText;
    public TMP_Text moneyText;
    public List<TowerButton> buttons = new List<TowerButton>();
    public DialogueObject greet, buyTower, cantBuy, empty;

    [SerializeField] private List<TowerObject> selectedObjects = new List<TowerObject>();
    private GameManager gameManager;
    private void OnEnable()
    {
        GameManager.OnMoneyChanged += UpdateMoney;
    }
    private void OnDisable()
    {
        GameManager.OnMoneyChanged -= UpdateMoney;
    }
    void Start()
    {
        gameManager = GameManager.instance;
        ReFreshStore();
        int randomIndex = Random.Range(0, greet.dialogues.Count);
        StartCoroutine(IE_ShowText(greet.dialogues[randomIndex]));
    }
    public void BuyTower(int index)
    {
        if(gameManager.playerData.Money >= selectedObjects[index].costInStore)
        {
            gameManager.ChangeMoney(CatBoxUtils.Enums.MoneyType.INSTORE, selectedObjects[index].costInStore * -1);
            gameManager.playerData.myTowers.Add(selectedObjects[index]);

            if(gameManager.playerData.Money == 0)
            {
                StartCoroutine(IE_ShowText("하하 이제 거지구만 친구"));
            }
            else if(selectedObjects[index].towerName == "총냥이")
            {
                StartCoroutine(IE_ShowText("진정한 고양이를 구매하였구만 역시 현명하군"));
            }
            else
            {
                int randomIndex = Random.Range(0, buyTower.dialogues.Count);
                StartCoroutine(IE_ShowText(buyTower.dialogues[randomIndex]));
            }
            // TODO : 구매 사운드
            Invoke("ReFreshStore", 1.0f);
        }
        else
        {
            int randomIndex = Random.Range(0, cantBuy.dialogues.Count);
            StartCoroutine(IE_ShowText(cantBuy.dialogues[randomIndex]));
            // TODO : 구매 불가 사운드
        }

    }

    private void ReFreshStore()
    {
        foreach(var button in buttons)
        {
            button.button.onClick.RemoveAllListeners();
        }

        selectedObjects = SelectTowers();
        
        if(selectedObjects.Count > 0)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i;

                if (index >= selectedObjects.Count)
                {
                    buttons[index].ChangeButtonData(selectedObjects[0]);
                    buttons[index].button.onClick.AddListener(() => BuyTower(0));
                }
                else
                {
                    buttons[index].ChangeButtonData(selectedObjects[index]);
                    buttons[index].button.onClick.AddListener(() => BuyTower(index));
                }
            }
        }
        else
        {
            foreach(var button in buttons)
            {
                button.gameObject.SetActive(false);
            }

            int randomIndex = Random.Range(0, empty.dialogues.Count);
            StartCoroutine(IE_ShowText(empty.dialogues[randomIndex]));
        }

    }

    private List<TowerObject> SelectTowers()
    {
        List<TowerObject> towerObjects = new List<TowerObject>();

        foreach(var tower in gameManager.towers)
        {
            if(!gameManager.playerData.myTowers.Contains(tower))
            {
                towerObjects.Add(tower);
            }
        }

        List<TowerObject> temp = new List<TowerObject>();

        if(towerObjects.Count > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                int index = Random.Range(0, towerObjects.Count);
                temp.Add(towerObjects[index]);
            }
        }
        else
        {
            temp.Clear();
        }

        return temp;
    }
    private void UpdateMoney()
    {
        moneyText.text = string.Format("{0:#,###}", gameManager.playerData.Money);
    }

    private IEnumerator IE_ShowText(string text)
    {
        ownerText.text = "<color=#00FFFB>사장님</color>\n";

        for (int i = 0; i < text.Length; i++)
        {
            ownerText.text += text[i];
            yield return new WaitForSeconds(0.015f);
        }
        yield break;
    }
}
