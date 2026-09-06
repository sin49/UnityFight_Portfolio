using menu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace menu
{


    public class CharacterListSortUI : MonoBehaviour
    {
        [Header("정렬 기준")]
        public Toggle namesorttoggle;
        public Toggle positionsorttoggle;
        public Toggle LVsorttoggle;
        [Header("오름차순 내림차순")]
        public Toggle Asctoggle;
        public Toggle Desctoggle;

        public Button OkButton;
        public Button CancelButton;


        charactersortoption1 sortoption1;
        charactersortoption2 sortoption2;
        CharacterSortListUIHandler parentsui;














       

        public void ActiveCharacterListSortUI(CharacterSortListUIHandler ui)
        {
            if (parentsui == null)
                parentsui = ui;
            LoadGamePlayCache();
            updatetoggle();
            this.gameObject.SetActive(true);
        }
        public void OKButtonInput()
        {
            saveGamePlayCache();
            parentsui.sortNCreateCharacterList();
            this.gameObject.SetActive(false);
        }
        public void BackButtonInput()
        {
            this.gameObject.SetActive(false);
        }
        void updatetoggle()
        {
            switch (sortoption1)
            {
                case charactersortoption1.LV:
                    LVsorttoggle.isOn = true;
                    break;
                case charactersortoption1.name:
                    namesorttoggle.isOn = true;
                    break;
                case charactersortoption1.position:
                    positionsorttoggle.isOn = true;
                    break;

            }
            switch (sortoption2)
            {
                case charactersortoption2.Desc:
                    Desctoggle.isOn = true;
                    break;
                case charactersortoption2.Asc:
                    Asctoggle.isOn = true;
                    break;
            }
        }
        void saveGamePlayCache()
        {
            GameManager.instance.GamePlayCache.CharacterSortOption1 = (int)sortoption1;
            GameManager.instance.GamePlayCache.CharacterSortOption2 = (int)sortoption2;
            FIleIOManager.MakeGamePlayCache(GameManager.instance.GamePlayCache);
        }
        void LoadGamePlayCache()
        {
            sortoption1 = (charactersortoption1)
                   GameManager.instance.GamePlayCache.CharacterSortOption1;
            sortoption2 = (charactersortoption2)
                GameManager.instance.GamePlayCache.CharacterSortOption2;

        }

        private void Awake()
        {
            if (GameManager.instance.GamePlayCache == null)
            {
                sortoption1 = charactersortoption1.LV;
                sortoption2 = charactersortoption2.Desc;
                FIleIOManager.MakeGamePlayCache(new GamePlayCache()
                {
                    CharacterSortOption1 = (int)sortoption1,
                    CharacterSortOption2 = (int)sortoption2
                });

            }
            else
            {
                LoadGamePlayCache();

            }
            updatetoggle();
            namesorttoggle.onValueChanged.AddListener((a) =>
            {
                if (a)
                {
                    sortoption1 = charactersortoption1.name;
                }
            });
            LVsorttoggle.onValueChanged.AddListener((a) =>
            {
                if (a)
                {
                    sortoption1 = charactersortoption1.LV;
                }
            });
            positionsorttoggle.onValueChanged.AddListener((a) =>
            {
                if (a)
                {
                    sortoption1 = charactersortoption1.position;
                }
            });
            Asctoggle.onValueChanged.AddListener((a) =>
            {
                if (a)
                {
                    sortoption2 = charactersortoption2.Asc;
                }
            });
            Desctoggle.onValueChanged.AddListener((a) =>
            {
                if (a)
                {
                    sortoption2 = charactersortoption2.Desc;
                }
            });
            CancelButton.onClick.AddListener(BackButtonInput);
            OkButton.onClick.AddListener(OKButtonInput);
        }
    }
}
