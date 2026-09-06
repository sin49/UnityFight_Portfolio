using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace menu
{
    public class SelectedCharacterIconUI : CharacterListIconUI
    {
       CharactersSelectUI characterSelectUI;
        [HideInInspector]
        public PlayerData Data { get { return data; } }
        public   bool ondata;
        protected override void Updatedata()
        {
            base.Updatedata();
            NamePanel.SetActive(false);
        }
        public void resetData()
        {
            data = null;
           
            this.gameObject.SetActive(false);
        }
        protected override void Awake()
        {
            base.Awake();

            this.gameObject.SetActive(false);
        }

        protected override void buttoninput()
        {

            characterSelectUI.removePlayer(data);
        }
       
        public override void setData(PlayerData d, Transform t=null)
        {
            data = d;
            if(characterSelectUI==null&& t!=null)
                characterSelectUI = t.GetComponent<CharactersSelectUI>();
            if (data != null)
            {
                Updatedata();
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
