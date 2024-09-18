using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    public TMP_Text ownerText;
    public List<TowerButton> buttons = new List<TowerButton>();
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameManager.instance;
    }

    void Update()
    {
        
    }

    private List<TowerObject> SelectTowers()
    {
        int index = Random.Range(0, gameManager.towers.Count);
        return null;
    }
    private IEnumerator IE_ShowText(string text)
    {
        ownerText.text = "<b>ªÁ¿Â¥‘</b> <br/>";
        for (int i = 0; i < text.Length; i++)
        {
            ownerText.text += text[i];
            yield return new WaitForSeconds(0.015f);
        }
        yield break;
    }
}
