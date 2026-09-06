using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace menu
{
    public class ShopItemPurchaseUI : DoubleButtonSliderHandler
    {

        public Image ItemImage;
        public Image NeedItemImage;

        int Price;
        int Amount;
        public TextMeshProUGUI ItemNameText;
        public TextMeshProUGUI PriceText;
        public TextMeshProUGUI CurrentAmount;
        public TextMeshProUGUI ItemLeftText;
        public Button PurchaseButton;
        public Button UndoButton;
        int index;
       public ShopUI shop;

        ShopItemIconUI chooseitem;
        ShopItem i;

        public void ActivePurchaseUI(ShopItemIconUI item,int n)
        {
            chooseitem = item;
            this.i = chooseitem.item;
            index=n;
            ItemNameText.text = i.data.Name;
            Price = i.price;
            Amount = GameManager.instance.Account.get_item(i.Needitemid).Amount;
            count = 1;
            MaxCount = Amount / Price;
            if (MaxCount > i.Amount)
            {
                MaxCount = i.Amount;
            }

            setUI();
        }
        void UndoButtonInput()
        {
            this.gameObject.SetActive(false);
        }
        void PurchaseButtonInput()
        {
            //i.Amount -= count;
            Debug.Log("구매 시작");

           GameManager.instance. CurrentShopList[index].Amount -= count;
            StartCoroutine(GameManager.instance.SaveShopList(GameManager.instance.CurrentShopList,
               Amount, () =>
                {
                   

                        GameManager.instance.Account.set_item(new item() { itemid = i.Needitemid, Amount = -(Price * count) });

                        GameManager.instance.Account.set_item(new item() { itemid = i.itemid, data = LoadManager.instance.getitem(i.itemid), Amount = count });


               
                }, () =>
                {
                    chooseitem.UpdateItemDatas(i);
                    shop.notifyObserver(GameManager.instance.Account.get_item(i.Needitemid));
                    //리스트는 갱신 및 저장 완료 이제 상점ui의 리스트를 업데이트 해야함
                    //ui리스트의 

                    this.gameObject.SetActive(false);

                    GameManager.instance.DeactiveInvisibleBG();
                    Debug.Log("구매 왼료");
                }));
                
           

            //StartCoroutine(GameManager.instance.saveAccountcorutine(() =>
            //{



            //    GameManager.instance.Account.set_item(new item() { itemid = i.Needitemid, Amount = -(Price * count) });

            //    GameManager.instance.Account.set_item(new item() { itemid = i.itemid, data = LoadManager.instance.getitem(i.itemid), Amount = count });

             

            //    shop.notifyObserver(GameManager.instance.Account.get_item(i.Needitemid));
            //   //리스트 갱신
            //    this.gameObject.SetActive(false);
            //    GameManager.instance.DeactiveInvisibleBG();

            //}));
        }
        void setUI()
        {
            ItemImage.sprite = i.data.itemSprite;
            NeedItemImage.sprite = i.NeedItem.itemSprite;

            int n = 0;
            int tmp = Amount;
            do
            {
                tmp = tmp / 10;
                n++;
            }
            while (tmp != 0);

            NeedItemImage.GetComponent<RectTransform>().position =
                NeedItemImage.GetComponent<RectTransform>().position +
               new Vector3(-15 * (n - 1), 0);

            ItemLeftText.text = "X" + i.Amount;

            slider.maxValue = MaxCount;
            slider.minValue = 1;
            if (count == MaxCount)
            {
                slider.gameObject.SetActive(false);
                PlusButton.gameObject.SetActive(false);
                MinusButton.gameObject.SetActive(false);
            }
            else
            {
                slider.gameObject.SetActive(true);
                PlusButton.gameObject.SetActive(true);
                MinusButton.gameObject.SetActive(true);
            }
            MinusButton.interactable = false;





        }
        protected override void Update()
        {
            base.Update();
            PriceText.text = (Amount - (Price * count)).ToString();
            CountText.text = "X" + count;
            CurrentAmount.text = Amount.ToString();


        }
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Awake()
        {
            base.Awake();
            PurchaseButton.onClick.AddListener(PurchaseButtonInput);
            UndoButton.onClick.AddListener(UndoButtonInput);
            
        }

    }
}