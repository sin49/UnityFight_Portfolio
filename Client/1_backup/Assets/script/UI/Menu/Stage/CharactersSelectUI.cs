using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DataExecute;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using NUnit.Framework;

namespace menu
{
    public class CharactersSelectUI : CharacterSortListUIHandler
    {
        

        public CharacterSelectIconUI characterListIcon;

        StageData selectStage;
        StageInformation info;


        public void ActiveSelectUI(StageData d,StageInformation info)
        {
            selectStage = d;
            this.info =info;
            staginfoText.text = selectStage.name;
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }
            sortNCreateCharacterList();
        }
        public TextMeshProUGUI staginfoText;

        public SelectedCharacterIconUI[] SelectedPlayerUI = new SelectedCharacterIconUI[5];
  


      public  CharacterSelectIconUI[] selectedplayerbuttons = new CharacterSelectIconUI[5];
        public void selectPlayer(PlayerData d, CharacterSelectIconUI ui)
        {

            if (SelectedPlayerUI[4].Data!=null)
                return;

           
            CharacterSelectIconUI buttontrasform= ui;
            for (int i = 0; i < 5; i++)
            {

                if (SelectedPlayerUI[i].Data== null)
                {
  
                    SelectedPlayerUI[i].setData(d,this.transform);
                    selectedplayerbuttons[i]= buttontrasform;

                    break;
                }
                
            }
            if(!StartButton.interactable)
            {
                StartButton.interactable = true;
            }
    
        }
        public void removePlayer(PlayerData d)
        {
            int index=0;
            for(int i = 0; i < 5; i++)
            {
                if (SelectedPlayerUI[i].Data==null)
                    break;
                else if ( SelectedPlayerUI[i].Data==d)
                {
                    SelectedPlayerUI[i].resetData();
                    index = i;
                    selectedplayerbuttons[i].selectedFalse();
                    selectedplayerbuttons[i] = null;
                    break;
                }
            }
           for(int n = index; n < 4; n++)
            {
                SelectedPlayerUI[n].setData(SelectedPlayerUI[n + 1].Data) ;
                SelectedPlayerUI[n + 1].resetData();
                selectedplayerbuttons[n] = selectedplayerbuttons[n + 1];
                selectedplayerbuttons[n + 1] = null;
            }

        }

        
        private void Awake()
        {
  
            StartButton.onClick.AddListener(StartButtonInput);
            listcreater.ScrollListHandlerUI = this;
        }
        private void Update()
        {
            if (SelectedPlayerUI[0].Data==null)
            {
                StartButton.interactable = false;
            }
            else
            {
                StartButton.interactable = true;
            }
        }

        public override void  CreateList(List<PlayerData> list,List<CharacterInformation> infolist = null)
        {
            base.CreateList();
            for (int n = 0; n < list.Count; n++)
            {
                if (!list[n].characterlock)
                {
                    if (infolist != null)
                    {
                        CreateListIcon(ScrollViewContent.transform).GetComponent<CharacterSelectIconUI>().setData(list[n], this.transform,
                            (charactersortoption1)GameManager.instance.GamePlayCache.CharacterSortOption1, infolist[n]);

                    }
                    else
                    {
                        CreateListIcon(ScrollViewContent.transform).GetComponent<CharacterSelectIconUI>().setData(list[n], this.transform
                           );
                    }

                }
            }
        }
     
      
       public Button StartButton;
        
        void StartButtonInput()
        {

            List<PlayerData> selectedPlayerlist = new List<PlayerData>();
            for (int n = 0; n < 5; n++)
            {
                if (SelectedPlayerUI[n].Data != null)
                    selectedPlayerlist.Add (SelectedPlayerUI[n].Data);
                else
                    break;
            }
            PlayerData[] list=selectedPlayerlist.ToArray();
            LoadManager.instance.loadgame(selectStage,info, (PlayerData[])list.Clone());
            
        }
     
       

        
    }
}