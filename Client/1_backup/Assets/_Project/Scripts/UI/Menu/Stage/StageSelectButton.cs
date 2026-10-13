using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DataExecute;
using UnityEngine.Events;

namespace menu
{
    public class StageSelectButton : MonoBehaviour
    {
        public Button Button;
        StageData d;

        public TextMeshProUGUI StageName;
        public TextMeshProUGUI StageClearLevel;
        StageUI stageui;
        void buttoninput()
        {
            if(!LoadManager.instance.onload)
                stageui.StageSelectButtoninput(d);

        }
        private void Awake()
        {
            Button.onClick.AddListener(buttoninput);
        }

        public void setData(StageData d, StageUI ui)
        {
            stageui = ui;
            this.d = d;
            if (d.stagelock == true)
                this.gameObject.SetActive(false);
          
            StageName.text =d.name;
            
            if (StageClearLevel != null)
            {
                string s = null;
                for (int i = 0; i < d.stagemission.Count; i++)
                {
                    if(d.stagemission[i].Clear)
                    s += "¡Ú";
                    else
                        s += "¡Ù";
                }
              
                StageClearLevel.text = s;
            }

        }

    }
}