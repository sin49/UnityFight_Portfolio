using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugButton : MonoBehaviour
{
    public string item;
    public void ButtonInput()
    {
      
        StartCoroutine(GameManager.instance.saveAccountcorutine(() =>
        {
            item i = new item();
            switch (item)
            {
                case "돈":
                    i.itemid = "I01";
                    i.Amount = 10000;
                    break;
                case "경험치1":
                    i.itemid = "I02";
                    i.Amount = 1000;
                    break;
                case "경험치2":
                    i.itemid = "I03";
                    i.Amount = 100;
                    break;
                case "경험치3":
                    i.itemid = "I04";
                    i.Amount = 10;
                    break;
                case "등급 강화":
                    i.itemid = "I05";
                    i.Amount = 10;
                    break;
                default:
                    Debug.Log("알수없는 아이템");
                    break;
            }
          
            GameManager.instance.Account.set_item(i);
        }));
    }
    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(ButtonInput);
    }
}
