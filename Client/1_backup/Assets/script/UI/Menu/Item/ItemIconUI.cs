using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemIconUI : MonoBehaviour
{
    public Image itemIcon;
    public Button ItemButton;
    public TextMeshProUGUI amountText;
    public item d;
    bool onclick;
    float ClickTImer = 0;
    public virtual void ImageButtonClickDown()
    {
        onclick = true;
    }
    public virtual void ImageButtonClickUp()
    {
        onclick = false;
        if (ClickTImer < 0.5f)
        {
            ItemButtonInput();
        }
        ClickTImer = 0;
    }
    public void ItemButtonInput()
    {

            //GameManager.instance.ActiveDescPopUp(d.data.Name, d.data.desc, this.transform);

    }
    public virtual void setItem(item i)
    {
        d = i;
        d.data = LoadManager.instance.getitem(d.itemid);
        if(amountText != null )
        amountText.text = "X" + d.Amount.ToString();
        UpdateItemInfo();
    }
    protected virtual void Update()
    {
 
        if (onclick)
        {
            ClickTImer += Time.deltaTime;
            if (ClickTImer > 0.5f)
            {
                ClickTImer = 0;
                GameManager.instance.ActiveItemDescription(d);
            }
        }
       
     
    }
    public virtual void UpdateItemInfo()
    {
        if (d.data != null)
        {

            itemIcon.sprite = d.data.itemSprite;
        }

    }
}
