
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
namespace menu
{
    public class Menu : MonoBehaviour
    {
        public CharacterUI CharacterUI;
        public StageUI StageUI;
        public itemUI ItemUI;
        public ShopUI ShopUI;
        public GameObject MainCanvas;
        
        public MainMenuUI MainUI;
        public Button SettingButton;
        public Button BackButton;
        public GameObject SettingUI;
        public Tutorial tutorialUI;
        public static Menu instance;
        public MenuUIHeader header;

        
        GameObject currentUI { get; set; }
      public Stack< GameObject> LastestUI { get; set; }

        public void ActiveUI(GameObject UI)
        {
            //if (tutorialUI.gameObject.activeSelf)
            //{
            //    return;
            //}
        
            if (currentUI != null)
                currentUI.SetActive(false);
            if(UI!=MainUI.gameObject)
            updatestack();
            else
            {
                LastestUI.Clear();
                BackButton.gameObject.SetActive(false);
            }
            currentUI = UI;
  
       
            currentUI.SetActive(true);
    

        }
        public void updatestack()
        {



            if (currentUI != null)
            {
                LastestUI.Push(currentUI);
              
            }

            BackButton.gameObject.SetActive(true);
            MainCanvas.gameObject.SetActive(false);

        }
        public void BackButttonInput()
        {
            
            if (LastestUI.Count > 0)
            {

                currentUI.SetActive(false);
                currentUI = LastestUI.Pop();
                //updatestack(currentUI);
                currentUI.SetActive(true);
               
                if (LastestUI.Count == 0)
                {
                    BackButton.gameObject.SetActive(false);
                    MainCanvas.gameObject.SetActive(true);
                }
                //header.UpdateButtons(currentUI);
            }
            else
            {
                Debug.Log("스택 크기 오류");
            }
        }
        public readonly float backbuttonbuffer = 0.1f;
        public float _backbuttonbuffer { get; set; }


   
        private void Awake()
        {
            instance = this;
            SettingButton.onClick.AddListener(() => SettingUI.gameObject.SetActive(true));
                LastestUI = new Stack<GameObject>();
            BackGroundAudioPlayer.instance.PlaymainBG();
            MainCanvas.gameObject.SetActive(true);
            BackButton.onClick.AddListener(BackButttonInput);
        }
        public void ActiveMainMenu()
        {
            ItemUI.gameObject.SetActive(false);
        
            SettingButton.gameObject.SetActive(true);
            BackButton.gameObject.SetActive(false);
        
            ShopUI.gameObject.SetActive(false);
            SettingUI.gameObject.SetActive(false);
            ActiveUI(MainUI.gameObject);
        }
        private void Update()
        {
            if (_backbuttonbuffer > 0)
            {
                BackButton.interactable = false;
                _backbuttonbuffer -= Time.deltaTime;
            }
            else
            {
                BackButton.interactable = true;
            }

        }

    }
}
