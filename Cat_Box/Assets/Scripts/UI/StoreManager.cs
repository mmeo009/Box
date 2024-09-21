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
        StartCoroutine(IE_ShowText("� ���ð� ������"));
    }
    public void BuyTower(int index)
    {
        if(gameManager.playerData.Money >= selectedObjects[index].costInStore)
        {
            gameManager.ChangeMoney(CatBoxUtils.Enums.MoneyType.INSTORE, selectedObjects[index].costInStore * -1);
            gameManager.playerData.myTowers.Add(selectedObjects[index]);

            if(gameManager.playerData.Money == 0)
            {
                StartCoroutine(IE_ShowText("���� ���� �������� ģ��"));
            }
            else if(selectedObjects[index].towerName == "�ѳ���")
            {
                StartCoroutine(IE_ShowText("������ ����̸� �����Ͽ����� ���� �����ϱ�"));
            }
            else
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        StartCoroutine(IE_ShowText("���� �ŷ����ٳ�"));
                        break;
                    case 1:
                        StartCoroutine(IE_ShowText("�̰� ���� �Ƴ��� ģ���ε� �ƽ��� �Ǿ�����"));
                        break;
                    case 2:
                        StartCoroutine(IE_ShowText("�� ����� ��ŭ�� �Ȱ� ���� �ʾҴµ�..."));
                        break;
                }
            }
            // TODO : ���� ����
            Invoke("ReFreshStore", 1.0f);
        }
        else
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    StartCoroutine(IE_ShowText("�̺� ���� ����ְ� ������ �̹� ���غ��� �����"));
                    break;
                case 1:
                    StartCoroutine(IE_ShowText("�ٸ� ����̵� ������ �� �̰� ����°հ�?"));
                    break;
                case 2:
                    StartCoroutine(IE_ShowText("���� �Ȱ���� �ʾƼ� ���Ĵ°� �ƴ϶��"));
                    break;
                case 3:
                    StartCoroutine(IE_ShowText("�ڳ� ������ �� ������ �ִ°Ű���?"));
                    break;
                case 4:
                    StartCoroutine(IE_ShowText("���� ����̸� ��ġ�ϴ� �����̶� �����ϴ°� �ƴϰ���?"));
                    break;
            }

            // TODO : ���� �Ұ� ����
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

            StartCoroutine(IE_ShowText("���̻��� ����̰� ���� �̰� �� �����̶��!"));
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
        ownerText.text = "<color=#00FFFB>�����</color>\n";

        for (int i = 0; i < text.Length; i++)
        {
            ownerText.text += text[i];
            yield return new WaitForSeconds(0.015f);
        }
        yield break;
    }
}
