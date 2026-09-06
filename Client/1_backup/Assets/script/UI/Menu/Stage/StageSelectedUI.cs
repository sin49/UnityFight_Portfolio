using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DataExecute;
using UnityEngine.Events;

namespace menu
{
    public class StageSelectedUI : MonoBehaviour
    {
        protected Dictionary<string, StageData> dictionary;
        public string selectstage;
        public GameObject StageSelectContent;
        public GameObject StageSelectContentUI;

        public TextMeshProUGUI StageNameText;
        public TextMeshProUGUI ProgressText;

        List<StageSelectButton> createButtonList;
        StageSelectButton pullingButton()
        {
            if (createButtonList == null)
                createButtonList = new List<StageSelectButton>();
            foreach (StageSelectButton s in createButtonList)
            {
                if (!s.gameObject.activeSelf)
                {
                    s.gameObject.SetActive(true);
                    return s;
                }
            }
            var obj = Instantiate(StageSelectContentUI, StageSelectContent.transform);
            createButtonList.Add(obj.GetComponent<StageSelectButton>());
            return obj.GetComponent<StageSelectButton>();

        }
        public virtual void getStageinfo(string s)
        {
            selectstage = s;
            Debug.Log(s);
            dictionary = GameManager.instance.Account.get_StageList(s);
            CreateObjectByDictionary();
        }
        protected void CreateObjectByDictionary()
        {
            if (createButtonList != null && createButtonList.Count > 0)
            {
                foreach (StageSelectButton s in createButtonList)
                {
                    s.gameObject.SetActive(false);
                }
            }
            foreach (KeyValuePair<string, StageData> d in dictionary)
            {
                pullingButton().setData(d.Value,this.transform.parent.GetComponent<StageUI>());
            }
            if (ProgressText != null)
                getProgressText();
            StageNameText.text = selectstage;
            StageSelectContent.GetComponent<RectTransform>().sizeDelta = new Vector2(StageSelectContent.GetComponent<RectTransform>().sizeDelta.x, 1600 + (300 * (StageSelectContent.transform.childCount - 5))
                );
        }
      
    
        public void getProgressText()
        {
            int sum = 0;
            foreach (KeyValuePair<string, StageData> d in dictionary)
            {
                foreach(StageMission item in d.Value.stagemission)
                {
                    if (item.Clear)
                        sum++;
                }
            }
            ProgressText.text = $"ÁøÇàµµ: {sum / (dictionary.Count * 3) * 100}% ({sum}/{dictionary.Count * 3})";
        }
    }
}