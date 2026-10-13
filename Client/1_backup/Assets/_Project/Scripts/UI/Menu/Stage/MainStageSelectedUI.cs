using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace menu
{
    public class MainStageSelectedUI : StageSelectedUI
    {
        public int stagenumber;
        int maxstagenumber;
        public Button rightDirectArrow;
        public Button LeftDirectArrow;
        public override void getStageinfo(string s)
        {
            if (GameManager.instance.Account.MaxStageClearData != "")
            {
                maxstagenumber = int.Parse(GameManager.instance.Account.MaxStageClearData.Substring(0,
                    GameManager.instance.Account.MaxStageClearData.IndexOf("-")));
                stagenumber = maxstagenumber;
            }
            else
            {
                maxstagenumber = 0;
                stagenumber = 0;
            }
                //k
                selectstage = s + stagenumber;
            Debug.Log(selectstage);
                dictionary = GameManager.instance.Account.get_StageList(selectstage+"-");

                StageData d;
                bool tmp;
                for (int n = 0; n < dictionary.Count; n++)
                {
                    string key = s + stagenumber + "-" + n;

       

                    if (dictionary.TryGetValue(key, out d))
                    {

                        if (n == 0)
                        {
                            if (d.stagelock)
                            {
                                dictionary[key].stagelock = false;
                            Debug.Log($"{key} lock's is unlock");
                                GameManager.instance.Account.get_Stage(key).stagelock = false;
                            }
                        }
                        else
                        {
                            tmp = false;
                            foreach (StageMission mission in d.stagemission)
                            {
                                if (mission.Clear)
                                {
                                    tmp = true;
                                    break;
                                }
                            }
                            if (!tmp)
                            {
                                continue;
                            }
                            else if (tmp)
                            {
                                if (d.stagelock)
                                {
                                    dictionary[key].stagelock = false;
                                Debug.Log($"{key} lock's is unlock");
                                GameManager.instance.Account.get_Stage(key).stagelock = false;
                                }
                                break;
                            }

                        }

                    }
                    else
                    {
                        Debug.Log(key + "is null");
                    }

                }
                CreateObjectByDictionary();
            }
        
        //bool unlockstage(string selectstage,int stagemininumber)
        //{
        //    StageData d;
        
        //    if (dictionary.TryGetValue(s, out d))
        //    {
        //      foreach(StageMission mission in d.stagemission)
        //        {
        //            if (mission.Clear)
        //            {
        //                tmp = true;
        //                break;
        //            }
        //        }
        //        if (d.stagelock)
        //        {
                   
        //            StartCoroutine(GameManager.instance.saveAccountcorutine(() =>
        //            {
        //                GameManager.instance.Account.get_Stage(d.ID).stagelock = false;
              
        //            }));
        //            return true;
        //        }
        //        else
        //            return false;
        //    }
        //    else
        //        return false;
           
        //}
        public void getStageinfo(string s,int n)
        {
            if (GameManager.instance.Account.MaxStageClearData != "")
            {
                maxstagenumber = int.Parse(GameManager.instance.Account.MaxStageClearData.Substring(0,
                    GameManager.instance.Account.MaxStageClearData.IndexOf("-")));
            }
            else
            {
                maxstagenumber = 0;
            }
            stagenumber = n;
            selectstage = s + stagenumber;
            dictionary = GameManager.instance.Account.get_StageList(selectstage);
            CreateObjectByDictionary();
        }
        //protected override void OnEnable()
        //{
        //    


        //    base.OnEnable();
        //}
        private void OnEnable()
        {
            foreach (KeyValuePair<string,StageData> d in GameManager.instance.Account.get_StageList())
            {
                Debug.Log($"{d.Value.ID} is {d.Value.stagelock}");
            }
        }
        private void Awake()
        {
            rightDirectArrow.onClick.AddListener(() =>
            {
                if (++stagenumber > maxstagenumber&& !LoadManager.instance.onload) stagenumber = maxstagenumber;
                else getStageinfo(selectstage + stagenumber);
                if (stagenumber == maxstagenumber)
                    rightDirectArrow.gameObject.SetActive(false);
                else
                    rightDirectArrow.gameObject.SetActive(true);
                if (stagenumber == 0)
                    LeftDirectArrow.gameObject.SetActive(false);
                else
                    LeftDirectArrow.gameObject.SetActive(true);
            });
            LeftDirectArrow.onClick.AddListener(() =>
            {
                if (--stagenumber < 1&& !LoadManager.instance.onload) stagenumber = 1;
                else getStageinfo(selectstage + stagenumber);
                if (stagenumber == maxstagenumber)
                    rightDirectArrow.gameObject.SetActive(false);
                else
                    rightDirectArrow.gameObject.SetActive(true);
                if (stagenumber == 0)
                    LeftDirectArrow.gameObject.SetActive(false);
                else
                    LeftDirectArrow.gameObject.SetActive(true);
            });
            if (stagenumber == maxstagenumber)
                rightDirectArrow.gameObject.SetActive(false);
            else
                rightDirectArrow.gameObject.SetActive(true);
            if (stagenumber == 0)
                LeftDirectArrow.gameObject.SetActive(false);
            else
                LeftDirectArrow.gameObject.SetActive(true);

        }
      
    }
}
