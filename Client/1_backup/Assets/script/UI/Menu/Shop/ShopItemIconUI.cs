using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace menu
{
    public class ShopItemIconUI : itemListIconUI,ItemObserver
{
       public ShopItem item;
        public TextMeshProUGUI PriceText;
        public Button ItemDescButton;
        public Image NeedItemIcon;
        public int index;

        
        public TextMeshProUGUI NameText;
        public void set_Data(ShopItem item, ShopUI t,int index) {
          this.item  = item;
            item.data = LoadManager.instance.getitem(item.itemid);
            item.NeedItem = LoadManager.instance.getitem(item.Needitemid);
            UpdateItemInfo();
         
            this.index = index;
            d = item as item;
            ItemButton.onClick.AddListener(()=>t.ActivePurchaseUI(this,this.index));
           
            //ItemDescButton.onClick.AddListener(() => {
            //GameManager.instance.ActiveItemDescription(this.item as item);
            //});

        }
      public void UpdateItemDatas(ShopItem item)
        {
            this.item = item;
            d = item as item;
            UpdateItemInfo();

        }
        private void OnDisable()
        {
            ItemButton.onClick.RemoveAllListeners();
        }
        public void UpdateInformation(item n)
        {
            if (n.itemid==item.Needitemid&&item.price > n.Amount)
            {
                PriceText.color = Color.red;
                ItemButton.interactable = false;
            }
            if (item.Amount <= 0)
            {
                amountText.text = "<color=#FF0000>Ç°ÀýµÊ</color>";
                ItemButton.interactable = false;
            }
            else
            {
                amountText.text = "X"+item.Amount;
            }
        }
       
        public override void UpdateItemInfo()
        {
            base.UpdateItemInfo();
            UpdateInformation(item);
            NameText.text = item.data.Name;
            amountText.text = "X" + item.Amount;
            PriceText.text = item.price.ToString();
            if (item.price > GameManager.instance.Account.GetMoney().Amount)
            {
                PriceText.color = Color.red;
                ItemButton.interactable = false;
            }
            itemIcon.sprite = item.data.itemSprite;
            NeedItemIcon.sprite = item.NeedItem.itemSprite;
          
        }
       
    }
}
