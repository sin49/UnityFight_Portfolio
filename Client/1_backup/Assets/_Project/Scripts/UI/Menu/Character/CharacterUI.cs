using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
namespace menu
{
    public class CharacterSortListUIHandler : ScrollListHandlerUI
    {
        public CharacterListSort listcreater;
        public virtual void CreateList(List<PlayerData>list, List<CharacterInformation> infolist = null)
        {
            base.CreateList();
        }
        public virtual void sortNCreateCharacterList()
        {
            listcreater.UpdatesortButtonText();
            var dict= GameManager.instance.Account.get_CharacterList();
            List<PlayerData> list = new List<PlayerData>();
            foreach(KeyValuePair<string,PlayerData> kvp in dict)
            {
                list.Add(kvp.Value);
            }
            List<string> list2 = new List<string>();
            List<CharacterInformation> list3 = null;
            if (GameManager.instance.GamePlayCache.CharacterSortOption2 == (int)charactersortoption2.Desc)
            {

                switch (GameManager.instance.GamePlayCache.CharacterSortOption1)
                {
                    case (int)charactersortoption1.LV:

                        list = list.OrderByDescending(p => p.LV).ToList();
                        CreateList(list);
                        break;
                    case (int)charactersortoption1.name:
                        foreach (var item in list)
                        {
                            list2.Add(item.id);
                        }
                        LoadManager.instance.PushCharacterdbsLoadRequest(list2, (l) =>
                        {

                            var sortinfo = l.OrderByDescending(p => p.name).ToList();

                            for (int n = 0; n < sortinfo.Count; n++)
                            {
                                for (int i = n; i < list.Count; i++)
                                {

                                    if (sortinfo[n].id == list[i].id)
                                    {
                                        if (n == i)
                                            break;
                                        var a = list[n];
                                        list[n] = list[i];
                                        list[i] = a;
                                    }
                                }
                            }
                            list3 = sortinfo;


                            CreateList(list, list3);

                        });
                        LoadManager.instance.DataLoadExecute();
                        break;
                    case (int)charactersortoption1.position:
                        foreach (var item in list)
                        {
                            list2.Add(item.id);
                        }
                        LoadManager.instance.PushCharacterdbsLoadRequest(list2, (l) =>
                        {

                            var sortinfo = l.OrderByDescending(p => p.position).ToList();

                            for (int n = 0; n < sortinfo.Count; n++)
                            {
                                for (int i = n; i < list.Count; i++)
                                {
                                   
                                    if (sortinfo[n].id == list[i].id)
                                    {
                                        if (n == i)
                                            break;
                                        var a = list[n];
                                        list[n] = list[i];
                                        list[i] = a;
                                    }
                                }
                            }
                            list3 = sortinfo;


                      CreateList(list, list3);

                        });
                        LoadManager.instance.DataLoadExecute();
                        break;

                }


            }
            else
            {

                switch (GameManager.instance.GamePlayCache.CharacterSortOption1)
                {
                    case (int)charactersortoption1.LV:

                        list = list.OrderBy(p => p.LV).ToList();
                        CreateList(list);
                        break;
                    case (int)charactersortoption1.name:
                        foreach (var item in list)
                        {
                            list2.Add(item.id);
                        }
                        LoadManager.instance.PushCharacterdbsLoadRequest(list2, (l) =>
                        {

                            var sortinfo = l.OrderBy(p => p.name).ToList();

                            for (int n = 0; n < sortinfo.Count; n++)
                            {
                                for (int i = n; i < list.Count; i++)
                                {

                                    if (sortinfo[n].id == list[i].id)
                                    {
                                        if (n == i)
                                            break;
                                        var a = list[n];
                                        list[n] = list[i];
                                        list[i] = a;
                                    }
                                }
                            }
                            list3 = sortinfo;


                            CreateList(list, list3);

                        });
                        LoadManager.instance.DataLoadExecute();
                        break;
                    case (int)charactersortoption1.position:
                        foreach (var item in list)
                        {
                            list2.Add(item.id);
                        }
                        LoadManager.instance.PushCharacterdbsLoadRequest(list2, (l) =>
                        {

                            var sortinfo = l.OrderBy(p => p.position).ToList();

                            for (int n = 0; n < sortinfo.Count; n++)
                            {
                                for (int i = n; i < list.Count; i++)
                                {
                                   
                                    if (sortinfo[n].id == list[i].id)
                                    {
                                        if (n == i)
                                            break;
                                        var a = list[n];
                                        list[n] = list[i];
                                        list[i] = a;
                                    }
                                }
                            }
                            list3 = sortinfo;





                            CreateList(list, list3);
                           
                        });
                        LoadManager.instance.DataLoadExecute();


                        break;

                }

            }

        }

    }
    public class CharacterUI : CharacterSortListUIHandler
    {

       
        //public GameObject characterList;
        public GameObject characterListContent;
        public Transform CharacterUnlockList;
        //public Transform CharacterlockList;
        
  

        public List<GameObject> AwakeDeActiveUIs = new List<GameObject>();

        public GameObject CharacterUpgrade;
       
        public void selecteCharacter(PlayerData d)
        {

            var b = CharacterUpgrade.GetComponent<CharacterUpgradeUI>();
            b.set_data(d);
           
        }
      

        private void Awake()
        {
            //characterList.SetActive(false);
            CharacterUpgrade.SetActive(false);
            foreach(GameObject item in AwakeDeActiveUIs)
            {
                item.SetActive(false);
            }

            listcreater.ScrollListHandlerUI = this;
        }
        protected override void InitializeList()
        {
            base.InitializeList();
            //SecondLayOut = null;
        }
        private void OnEnable()
        {

            sortNCreateCharacterList();
   
        }
     
        public override void CreateList(List<PlayerData> list, List<CharacterInformation> infolist = null)
        {
           base.CreateList(list, infolist);
            int unlockreated = 0;

            for (int n = 0; n < list.Count; n++)
            {
                if (!list[n].characterlock)
                {
                    if (infolist != null)
                    {
                        CreateListIcon(CharacterUnlockList).GetComponent<CharacterListIconUI>().setData(list[n], this.transform,
      (charactersortoption1)GameManager.instance.GamePlayCache.CharacterSortOption1, infolist[n]);
                        //CreateListIcon(CharacterUnlockList).GetComponent<CharacterListIconUI>().setData(list[n], this.transform,
                        //    (charactersortoption1)GameManager.instance.GamePlayCache.CharacterSortOption1, infolist[n]);

                    }
                    else
                    {
                     
                        CreateListIcon(CharacterUnlockList).GetComponent<CharacterListIconUI>().setData(list[n], this.transform
                           );
                    }

                }
                //else
                //{
                //    if (infolist != null)
                //    {
                //        CreateSecondListIcon(CharacterlockList).GetComponent<CharacterListIconUI>().setData(list[n], this.transform,
                //            (charactersortoption1)GameManager.instance.GamePlayCache.CharacterSortOption1, infolist[n]);

                //    }
                //    else
                //    {
                //        CreateSecondListIcon(CharacterlockList).GetComponent<CharacterListIconUI>().setData(list[n], this.transform
                //           );
                //    }

                //}
            }

            ScrollViewContent.GetComponent<VerticalLayoutGroup>().spacing = 500 + 250 * (unlockreated - 1);
        }
   
     
       
        //int secondListCount;
        //Transform SecondLayOut;
        //protected virtual GameObject CreateSecondListIcon(Transform LayOutTransform)
        //{

        //    if (SecondLayOut == null || secondListCount == LayOutAcceptCount)
        //    {
        //        secondListCount = 0;
        //        LayOutCount++;
        //        ScrollViewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, ContentSizeUpScale * LayOutCount);
        //        SecondLayOut = pullingLayout(LayOutTransform).transform;
        //        Debug.Log("2번째 레이아웃");
        //    }

        //    secondListCount++;
        //    return pullingIcon(SecondLayOut);
        //}

    }
  
    
}
