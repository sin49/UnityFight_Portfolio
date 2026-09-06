using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace menu
{
    public class stageinfoUI : MonoBehaviour
    {
   StageUI stageui;
        StageInformation info;
        StageData data;
        public TextMeshProUGUI StageNameText;
        public TextMeshProUGUI EnoughLVText;
        public TextMeshProUGUI needstaminaText;
        public TextMeshProUGUI[] MissionList=new TextMeshProUGUI[3];
        public spawncharactericonUI spawncharactrUI;
        List<spawncharactericonUI> spawncharactrUIpulling;
        spawncharactericonUI PullingCharacterIcon()
        {
            if (spawncharactrUIpulling == null)
                spawncharactrUIpulling = new List<spawncharactericonUI>();
            foreach(spawncharactericonUI item in spawncharactrUIpulling)
            {
                if(!item.gameObject.activeSelf)
                {
                    item.gameObject.SetActive(true);
                    return item;
                }
            }
            var a = Instantiate(spawncharactrUI.gameObject).GetComponent<spawncharactericonUI>();
            spawncharactrUIpulling.Add(a);
            return a;
        }
        public Transform Enemylist;
        public ItemIconUI spawnItemIconUI;
        List<ItemIconUI> spawnItemIconUIPulling;
        ItemIconUI PullingitemIcon()
        {
            if (spawnItemIconUIPulling == null)
                spawnItemIconUIPulling = new List<ItemIconUI>();
            foreach (ItemIconUI item in spawnItemIconUIPulling)
            {
                if (!item.gameObject.activeSelf)
                {
                    item.gameObject.SetActive(true);
                    return item;
                }
            }
            var a = Instantiate(spawnItemIconUI.gameObject).GetComponent<ItemIconUI>();
            spawnItemIconUIPulling.Add(a);
            return a;
        }
        private void OnDisable()
        {
            if (spawncharactrUIpulling != null)
            {
                foreach (spawncharactericonUI item in spawncharactrUIpulling)
                {
                    item.gameObject.SetActive(false);
                }
            }
            if (spawnItemIconUIPulling != null)
            {
                foreach (ItemIconUI item in spawnItemIconUIPulling)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
        public Transform Itemlist;
        public Button BackButton;
        public Button StartButton;

        public void setData(StageData d,StageUI ui)
        {
            stageui=ui;
            if (!LoadManager.instance.onload)
            {
                data = d;
                LoadManager.instance.PushStagedbLoadRequest(data.ID, (i) =>
                {
                    info = i;
                    UpdateInfo();
                    this.gameObject.SetActive(true);


                });
                LoadManager.instance.DataLoadExecute();
            }
        }
        void UpdateInfo()
        {
            StageNameText.text = info.name;
            needstaminaText.text = $"필요 스테미나 :{info.StaminaNeed}";
            int n = 0;
            foreach (CharacterData d in info.SpawnCharacterList)
            {
                n += d.LV;
                var a = PullingCharacterIcon();
                a.transform.SetParent(Enemylist);
                a.SetData(d);
            }
            foreach(StageReward s in info.rewardlist)
            {
                var a = PullingitemIcon();
                a.setItem(s.item);
                a.transform.SetParent(Itemlist);
            }
            for(int i = 0; i < data.stagemission.Count; i++)
            {
                if (data.stagemission[i].Clear)
                    MissionList[i].text = "★" + MissionList[i].text.Substring(1);
                else
                    MissionList[i].text = "☆" + MissionList[i].text.Substring(1);
            }
           
            EnoughLVText.text = $"적정 레벨 {n / info.SpawnCharacterList.Count}";
        }
        private void Awake()
        {
            BackButton.onClick.AddListener(backbuttoninput);
            StartButton.onClick.AddListener(startbuttoninput);
        }
        void backbuttoninput()
        {
            this.gameObject.SetActive(false);
        }
        void startbuttoninput()
        {
            Menu.instance.ActiveUI(stageui.CharacterSelectUI);
            stageui.CharacterSelectUI.GetComponent<CharactersSelectUI>().ActiveSelectUI(data,info);
            this.gameObject.SetActive(false);
        }
    }
}