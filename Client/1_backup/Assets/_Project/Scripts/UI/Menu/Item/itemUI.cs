using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace menu
{
    public class itemUI : ScrollListHandlerUI
    {


        Dictionary<string, itemListIconUI> createIconList;




        public item SelectItem;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI AmountText;
        public Image ItemIcon;
        public TextMeshProUGUI DescriptionText;
        public GameObject Desc;
        private void OnDisable()
        {
            SelectItem=null;
          
            
        }
        void Deleteitemicon(item d)
        {
            itemListIconUI data;
            if ((createIconList.TryGetValue(d.itemid, out data)))
            {
                data.gameObject.SetActive(false);
                GameObject parent = data.transform.parent.gameObject;
                int n = LayoutList.IndexOf(parent);
                int number = 1;
                while (LayoutList.Count > n + number)
                {
                    LayoutList[n + number].transform.GetChild(0).SetParent(LayoutList[n + number - 1].transform);

                    number++;
                }
            }
        }
        public override void CreateList()
        {
            base.CreateList();
            createIconList = new Dictionary<string, itemListIconUI>();
            var list = GameManager.instance.Account.get_itemList();

            foreach (KeyValuePair<string, item> Kvp in list)
            {
                if (Kvp.Value.Amount > 0)
                {
                    var a = CreateListIcon(ScrollViewContent.transform).GetComponent<itemListIconUI>();
                    a.set_Data(Kvp.Value, this.transform);
                    createIconList.Add(Kvp.Key, a);
                }
            }
        }
        //public void CreateItemlist()
        //{


         

        //    if (CreateIcon != null && CreateIcon.Count > 0)
        //    {
        //        foreach (GameObject s in CreateIcon)
        //        {
        //            s.SetActive(false);
        //        }
        //    }
        //    if (LayoutList != null && LayoutList.Count > 0)
        //    {
        //        foreach (GameObject s in LayoutList)
        //        {
        //            s.SetActive(false);
        //        }
        //    }
           
                   
               
            

        //}

        void Awake()
        {
            //Desc.SetActive(false);
            SelectItem = null;
        }
        private void OnEnable()
        {
            CreateList();
        }
        // Update is called once per frame
        void Update()
        {
            if (SelectItem!=null)
            {
          
                ItemIcon.gameObject.SetActive(true);
                DescriptionText.text = SelectItem.data.desc;
                NameText.text = SelectItem.data.Name;
                ItemIcon.sprite = SelectItem.data.itemSprite;
                AmountText.text = SelectItem.Amount+"개 보유 중";
            }
            else
            {
                ItemIcon.gameObject.SetActive(false);
                DescriptionText.text ="";
                NameText.text = "";
                AmountText.text = "";
                //Desc.SetActive(false);

            }
        }
    }
}