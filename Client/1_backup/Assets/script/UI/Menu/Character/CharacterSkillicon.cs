using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace menu
{
    public class CharacterSkillicon : MonoBehaviour
    {
        public Character_action skill;
        public Image skillIcon;
        public Sprite AttackSprite;
        public Sprite SupportSprite;
        public Sprite DisturbSprite;

        public TextMeshProUGUI skilliconText;
        public TextMeshProUGUI skillLVtext;
        public GameObject SkillDescription;
        CharacterUpgradeUI upgradeui;
   
        public void seticontext(string s)
        {
            skilliconText.text = s;
        }
        
        public void ActiveSkillDescription()
        {
            SkillDescription.SetActive(true);
        }
        public void DeActiveSkillDescription()
        {
            SkillDescription.SetActive(false);
        }
        public TextMeshProUGUI SkillnameText;
        public TextMeshProUGUI skilldescriptionText;

     public virtual void SkillDescriptionTextUpdate()
        {
            List<float> ActionPower = skill.getActionpowers();
            List<float> Actionsec = skill.getActiondurations();
           
            string s = skill.skilldescription;
            int i = 0;
            int n = 0;
            do//figure
            {
                
                i = s.IndexOf("figure" + n);
                Debug.Log("Test i index:" + i);
                if (i != -1)
                {
                    string replaceValue = (ActionPower != null && n < ActionPower.Count) ? ActionPower[n].ToString() : "0";
                   
                    s = s.Substring(0, i) + replaceValue + s.Substring(i + ("figure" + n).Length);
                    n++;
                    //s = s.Substring(0, i);
                    //s += ActionPower[n];
                    //s+= s.Substring(i + ("figure" + n).Length);
                    //n++;
                }
                
            } while (i != -1);
            i = 0;
            n = 0;
            do//figure
            {

                i = s.IndexOf("duration" + n);
                if (i != -1)
                {
                    // ★ 방어 코드: 리스트에 값이 있으면 그 값을, 없으면 임시로 "0"을 넣는다.
                    string replaceValue = (Actionsec != null && n < Actionsec.Count) ? Actionsec[n].ToString() : "0";

                    s = s.Substring(0, i) + replaceValue + s.Substring(i + ("duration" + n).Length);
                    n++;
                    //s = s.Substring(0, i) + Actionsec[n] + s.Substring(i + ("duration" + n).Length);
                    //n++;
                }
            } while (i != -1);
      
            skilldescriptionText.text = s;
            //foreach (ActionClass ac in skill.ActionDesc)
            //{
            //    if (ac.getfigurepercent(skill.skillLV) != 0)
            //        ActionPower.Add(ac.getfigurepercent(skill.skillLV));
            //    if (ac.getEffectPower(skill.skillLV) != 0)
            //        Actionsec.Add(ac.getEffectPower(skill.skillLV));
            //}
            //if (s != -1)
            //    skilldescriptionText.text =
            //    this.skill.skilldescription.Substring(0, s)
            //    + this.skill.AtkPercent +
            //    this.skill.skilldescription.Substring(s);

        }
        public virtual void set_skill(Character_action skill, CharacterUpgradeUI ui=null)
        {
        
            this.skill = skill;
            if (upgradeui == null)
                upgradeui = ui;
            switch (skill.ActionType)
            {
                case ActionType.Attack:
                    skillIcon.color = Color.red;
                    skillIcon.sprite = AttackSprite;
                    break;
                case ActionType.Support:
                    skillIcon.color = Color.green;
                    skillIcon.sprite = SupportSprite;
                    break;
                case ActionType.disturb:
                    skillIcon.color = Color.blue;
                    skillIcon.sprite = DisturbSprite;
                    break;
            }
            skilliconText.text = skill.action_name;
            skilliconText.fontSize = 50;
           
            SkillnameText.text = this.skill.skillname;
            skillLVtext.text = "LV " +this.skill.skillLV.ToString();
            SkillDescriptionTextUpdate();
            if (upgradeui != null)
            {
                if (this.skill != null)
                {
                    if (this.skill.SkillUpgrradefee < GameManager.instance.Account.GetMoney().Amount)
                    {
                        upgradeui.UpgradeOperator |= true;
                    }
                    else
                    {
                        upgradeui.UpgradeOperator |= false;
                    }
                }
            }
        }

        protected virtual void OnEnable()
        {
            SkillDescription.SetActive(false);

        }

    }
}