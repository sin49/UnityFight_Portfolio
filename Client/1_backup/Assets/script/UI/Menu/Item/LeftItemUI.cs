using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LeftItemUI : MonoBehaviour
{
    public Image ItemImage;
    public TextMeshProUGUI MoneyText;
    public string itemcode = "I01";
   
    item item;

    public void setitem(string s)
    {

        item = GameManager.instance.Account.get_item(s);
        if (item == null)
        {
            GameManager.instance.Account.set_item(new item()
            {
                itemid = s,
                Amount = 0,
                data = LoadManager.instance.getitem(s)
            }
                );
            item = GameManager.instance.Account.get_item(s);
        }
        ItemImage.sprite = LoadManager.instance.getitem(s).itemSprite;
        var button = ItemImage.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            item i = new item()
            {
                itemid = itemcode, Amount = 10000, data = LoadManager.instance.getitem(itemcode)
            };
            GameManager.instance.Account.set_item(i);
            item= GameManager.instance.Account.get_item(itemcode);
        });
    }
    private void OnEnable()
    {
        setitem(itemcode);
       
    }
    private void Update()
    {
       
            MoneyText.text = item.Amount.ToString();
    }

}
