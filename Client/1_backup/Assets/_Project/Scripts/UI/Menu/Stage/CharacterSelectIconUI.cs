using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace menu
{
    public class CharacterSelectIconUI : CharacterListIconUI
    {
      CharactersSelectUI characterSelectUI;
        public  GameObject selectedshadow;

       bool selected;
      
       
        public override void setData(PlayerData d, Transform t)
        {
            data = d;
            Updatedata();
            characterSelectUI = t.GetComponent<CharactersSelectUI>();

            selectedFalse(); // 초기화
            for (int i = 0; i < 5; i++)
            {
                if (characterSelectUI.SelectedPlayerUI[i].Data == d)
                {
                    characterSelectUI.selectedplayerbuttons[i] = this; // 참조 동기화
                    selectedshadow.SetActive(true); // 선택됨 그림자 표시
                    selected = true;
                    break;
                }
            }
        }
      
        protected override void OnEnable()
        {
            base.OnEnable();
            selectedshadow.SetActive(false);
        }
        
       public void selectedFalse()
        {
            selectedshadow.SetActive(false);
            selected = false;
        }
        protected override void buttoninput()
        {

            if (!selected)
            {

                characterSelectUI.selectPlayer(data, this);
                selectedshadow.SetActive(true);
                selected = true;
            }
            else
            {
  
                characterSelectUI.removePlayer(data);
                //selectedFalse();
            }
            
        }
    }
}
