using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace menu
{
    public class StageUI : MonoBehaviour
    {
        public StageBannerUI MainStageUI;

        //public GameObject stageSelectUI;
        public GameObject MainStageSelectedUI;
        public GameObject CharacterSelectUI;
        public stageinfoUI infoui;

      

 
        public void StageSelectButtoninput(StageData d)
        {

            infoui.setData(d,this);

        }
        private void OnEnable()
        {
            LoadManager.instance.PushStageDownLoadRequest();
            LoadManager.instance.DataLoadExecute();
         
        }

        private void Awake()
        {
            infoui.gameObject.SetActive(false);
            MainStageSelectedUI.SetActive(false);
            CharacterSelectUI.SetActive(false);
            MainStageUI.StageButton.onClick.AddListener(() =>
            {
                if (!LoadManager.instance.onload)
                {
                    StageButtonInput(MainStageSelectedUI);
                    MainStageSelectedUI.GetComponent<StageSelectedUI>().getStageinfo("Main");
                }
            });
        }
        void StageButtonInput(GameObject UI)
        {
            Menu.instance.ActiveUI(UI);

           

        }
        

      
    }
}