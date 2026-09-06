using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace menu
{
    public class CharacterListIconUI : CharacterIconUI
    {
       
        public GameObject NamePanel;
        public TextMeshProUGUI LVText;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI GradeText;
        CharacterUI chrUI;
        public GameObject Shadow;
        public virtual void setData(PlayerData d, Transform t)
        {
            data = d;
            Updatedata();
            chrUI = t.GetComponent<CharacterUI>();
        }
        public virtual void setData(PlayerData d, Transform t,charactersortoption1 sortoption,CharacterInformation info=null)
        {
            setData(d, t);
            updatenametextbysort(sortoption,info);
        }
      protected  void updatenametextbysort(charactersortoption1 sortoption,CharacterInformation info=null)
        {
            Debug.Log("test1");
            if (info == null)
                return;
            Debug.Log("test2");
            switch (sortoption)
            {
                case charactersortoption1.position:
                    switch (info.position)
                    {
                        case Position.front:
                            NameText.text = "front";
                            break;
                            case Position.back:
                            NameText.text = "back";
                            break;
                        case Position.mid:
                            NameText.text = "mid";
                            break;
                    }
                    break;
                    default: break;
            }
       
        }
        protected override void Updatedata()
        {
            base.Updatedata();
            

                NamePanel.SetActive(true);
            if (data != null)
            {
                if (Shadow != null && data.characterlock)
                {
                    Shadow.SetActive(true);
                    LVText.gameObject.SetActive(false);
                }
                else
                {
                    if (Shadow != null)
                        Shadow.SetActive(false);

                    LVText.gameObject.SetActive(true);
                    LVText.text = "LV" + data.LV.ToString();
                }
                if (NameText != null)
                    NameText.text = data.Name;
                if (GradeText != null)
                {
                    GradeText.text = "";
                    for (int n = 0; n < data.Grade; n++)
                    {
                        GradeText.text += "¡Ú";
                    }
                }
            }
            
        }
        protected virtual void OnEnable()
        {

           
            if(data==null)
            {
                LVText.gameObject.SetActive(false);
                NamePanel.SetActive(false);
            }
        }
        //private void OnDisable()
        //{
        //    data = null;
        //}
        protected  override void buttoninput()
        {
            if (!LoadManager.instance.onload)
            {
                chrUI.selecteCharacter(data);
            }
        }

    }

}