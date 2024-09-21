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
        StartCoroutine(IE_ShowText("어서 오시게 꼬맹이"));
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
                switch (Random.Range(0, 3))
                {
                    case 0:
                        StartCoroutine(IE_ShowText("좋은 거래였다네"));
                        break;
                    case 1:
                        StartCoroutine(IE_ShowText("이거 내가 아끼던 친구인데 아쉽게 되었구만"));
                        break;
                    case 2:
                        StartCoroutine(IE_ShowText("이 고양이 만큼은 팔고 싶지 않았는데..."));
                        break;
                }
            }
            // TODO : 구매 사운드
            Invoke("ReFreshStore", 1.0f);
        }
        else
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    StartCoroutine(IE_ShowText("이봐 나도 깎아주고 싶은데 이미 손해보는 장사라고"));
                    break;
                case 1:
                    StartCoroutine(IE_ShowText("다른 고양이도 많은데 왜 이걸 사려는겐가?"));
                    break;
                case 2:
                    StartCoroutine(IE_ShowText("내가 팔고싶지 않아서 안파는게 아니라고"));
                    break;
                case 3:
                    StartCoroutine(IE_ShowText("자네 물건을 살 생각은 있는거겠지?"));
                    break;
                case 4:
                    StartCoroutine(IE_ShowText("설마 고양이를 설치하는 가격이랑 착각하는건 아니겠지?"));
                    break;
            }

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

            StartCoroutine(IE_ShowText("더이상은 고양이가 없네 이거 참 멸종이라고!"));
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
