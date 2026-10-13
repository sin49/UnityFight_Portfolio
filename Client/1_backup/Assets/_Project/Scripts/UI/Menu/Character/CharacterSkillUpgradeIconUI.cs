using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace menu
{
    public class CharacterSkillUpgradeIconUI : CharacterSkillicon
    {
        
        SkillUpgradeUI skillupgradeui;
        private void Awake()
        {
            skillupgradeui =  this.transform.parent.parent.parent.GetComponent<SkillUpgradeUI>();
            this.GetComponent<Button>().onClick.AddListener(ButtonInput);
        }
        protected override void OnEnable()
        {
            if (SkillDescription != null)
                base.OnEnable();
            if(skill.action_name=="")
                this.gameObject.SetActive(false);
        }
     
        public override void set_skill(Character_action skill,CharacterUpgradeUI ui=null)
        {

            this.skill = skill;

            switch (skill.ActionType)
            {
                case ActionType.Attack:
                    skillIcon.color = Color.red;
                    break;
                case ActionType.Support:
                    skillIcon.color = Color.green;
                    break;
                case ActionType.disturb:
                    skillIcon.color = Color.blue;
                    break;
            }
            skilliconText.text = skill.action_name;
            skilliconText.fontSize = 50;
            SkillnameText.text = this.skill.skillname;
            skillLVtext.text = "LV " + this.skill.skillLV.ToString();

        }
        void ButtonInput()
        {
            skillupgradeui.getSkillIcon(this);
        }
    }
}