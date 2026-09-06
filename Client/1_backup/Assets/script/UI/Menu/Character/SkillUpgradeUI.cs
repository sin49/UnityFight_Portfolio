using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



namespace menu
{
    public class SkillUpgradeUI : MonoBehaviour
    {
        public GameObject invisibleBG;
        public GameObject UpgraeObj;
        CharacterSkillUpgradeIconUI selecticon;
        public TextMeshProUGUI Skillname;
        public TextMeshProUGUI Skilldescription;
        public int SkillUpgradefee;
        public Image MoneyImage;
        public TextMeshProUGUI SkillUpgradefeeText;
        public Button UpgradeButton;
        public Button BackButton;

        CharacterUpgradeUI upgui;
        void UpgradeButtonInput()
        {
            if (selecticon.skill.SkillUpgrradefee < GameManager.instance.Account.GetMoney().Amount && selecticon.skill.skillLV != 10)
            {
                
                StartCoroutine(GameManager.instance.saveAccountcorutine(() =>
                {
                    GameManager.instance.Account.SetMoney(selecticon.skill.SkillUpgrradefee * -1);
                    selecticon.skill.UpgradeSkill();
                    selecticon.set_skill(selecticon.skill);
                    updateText();
                }));
                upgui.updateprofil();
            }
        }
        public void getSkillIcon(CharacterSkillUpgradeIconUI icon)
        {
            if(selecticon!=null)
                selecticon.GetComponent<Image>().color = new Color(1, 1, 0.4f);
            if (selecticon != icon)
            {
                
                selecticon = icon;
                icon.GetComponent<Image>().color = new Color(1, 1, 0.4f);
            }
            else
            {
                icon.GetComponent<Image>().color = new Color(0.7294118f, 0.7294118f, 0.7294118f);
                selecticon = null;
            }

            if (selecticon == null)
            {
                UpgraeObj.SetActive(false);
            }
            else
            {
                updateText();
                UpgraeObj.SetActive(true);
             
            }
        }

        public virtual void SkillDescriptionTextUpdate()
        {

            List<float> ActionPower = selecticon.skill.getActionpowers();
            List<float> Actionsec = selecticon.skill.getActiondurations();

            string s = selecticon.skill.skilldescription;
            int i = 0;
            int n = 0;
            do//figure
            {
                i = s.IndexOf("figure" + n);
                if (i != -1)
                {
                    string replaceValue = (ActionPower != null && n < ActionPower.Count) ? ActionPower[n].ToString() : "0";
                    s = s.Substring(0, i) + "<color=#FFFF64>" + replaceValue + "</color>" + s.Substring(i + ("figure" + n).Length);
                    n++;
                //    i = s.IndexOf("figure" + n);
                //if (i != -1)
                //{
                //    s = s.Substring(0, i) + "<color=#FFFF64>" + ActionPower[n]+ "</color>" + s.Substring(i + ("figure" + n).Length);
                //    n++;
                }

            } while (i != -1);
            i = 0;
            n = 0;
            do//figure
            {

                i = s.IndexOf("duration" + n);
                if (i != -1)
                {
                    string replaceValue = (Actionsec != null && n < Actionsec.Count) ? Actionsec[n].ToString() : "0";
                    s = s.Substring(0, i) + "<color=#FFFF64>" + replaceValue + "</color>" + s.Substring(i + ("duration" + n).Length);
                    n++;
                }
            } while (i != -1);

           Skilldescription.text = s;
           

        }
        void updateText()
        {
            Skillname.text = selecticon.skill.skillname;
            SkillDescriptionTextUpdate();
            SkillUpgradefeeText.text = selecticon.skill.SkillUpgrradefee.ToString();
            if (selecticon.skill.SkillUpgrradefee > GameManager.instance.Account.GetMoney().Amount) 
                UpgradeButton.interactable = false;
        }
        private void Awake()
        {
            BackButton.onClick.AddListener(BackButtonInput);
            UpgradeButton.onClick.AddListener(UpgradeButtonInput);
            MoneyImage.sprite = LoadManager.instance.getitem("I01").itemSprite;
            UpgraeObj.SetActive(false);
        }
        public void ActiveSkillUpgPanel(CharacterUpgradeUI ui)
        {
            this.gameObject.SetActive(true);
            invisibleBG.SetActive(true);
            upgui = ui;
        }
        public void BackButtonInput()
        {
            this.gameObject.SetActive(false);
            invisibleBG.SetActive(false);
        }
       
    }
}