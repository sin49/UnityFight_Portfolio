using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DataExecute;
namespace menu {
    public class ShopUI : ScrollListHandlerUI, ItemBroadcaster
    {

        public LeftItemUI MoneyUI;
      public  int shopid;
        public ShopItemPurchaseUI ItemPurchaseUI;
        public void ActivePurchaseUI(ShopItemIconUI i,int n)
        {
            ItemPurchaseUI.ActivePurchaseUI(i,n);
            ItemPurchaseUI.gameObject.SetActive(true);
            
        }
        public void UpdateShopItemList()
        {
            
        }


        private void OnDisable()
        {
            GameManager.instance.CurrentShopList.Clear();
        }
        protected override void InitializeList()
        {
            base.InitializeList();
            Observer.Clear();
        }
        public  void setBasicShopItems(Shopid id)
        {
            
            InitializeList();
            switch (id) {
                case Shopid.Daily:
                    shopid = (int)id;
            
            notifyObserver(GameManager.instance.Account.GetMoney());
                    break;
        }
            CreateShopItemIconList(GameManager.instance.CurrentShopList);
        }
      
        void CreateShopItemIconList(List<ShopItem> items,List<ItemObserver> observer=null)
        {
            for(int n = 0; n < items.Count; n++)
            {
                ShopItemIconUI UI = CreateListIcon(ScrollViewContent.transform).GetComponent<ShopItemIconUI>();

                if (observer != null)
                    AddObserver(UI);

                UI.set_Data(items[n], this,n);
            }
            //foreach (ShopItem i in items)
            //{
            //    ShopItemIconUI UI = CreateListIcon(ScrollViewContent.transform).GetComponent<ShopItemIconUI>();

            //    if (observer != null)
            //        AddObserver(UI);

            //    UI.set_Data(i, this);
            //}
        }
        
        private void Awake()
        {
            

          
            Observer = new List<ItemObserver>();

       
            ItemPurchaseUI.shop = this;

        }
        List<ItemObserver> Observer;
    
        public void AddObserver(ItemObserver s)
        {
            Observer.Add(s);

        }

        public void DeleteObserver(ItemObserver s)
        {
            Observer.Remove(s);
        }

        public void notifyObserver(item n)
        {foreach(ItemObserver obj in Observer)
            {
                obj.UpdateInformation(n);
            }
        }
    }
}


