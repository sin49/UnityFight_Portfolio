using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace menu
{
    public class CharacterUpgradeUI : MonoBehaviour
    {
        PlayerData d;
        CharacterInformation info;

        public TextMeshProUGUI LvText;
        public TextMeshProUGUI ExpText;
        public Image ExpSlider;
        public Button ExpButton;
        public TextMeshProUGUI GradeText;
        public Button GradeButton;
        public CharacterGradeUpgradeUI PieceItemIconUI;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI HPText;
        public TextMeshProUGUI ATKText;
        public RectTransform HpSlider;
        public RectTransform ATKSlider;
        public TextMeshProUGUI PositionText;
        public CharacterSkillicon[] characterskilliconlist = new CharacterSkillicon[4];
        public CharacterSkillUpgradeIconUI[] characterskillupgradeiconlist = new CharacterSkillUpgradeIconUI[4];
        public GameObject upgrademodel;
        public CharacterUpgradeModeling upgrademodeling;

        public Image CanUpgradeSkill;
        public bool UpgradeOperator;
        public Button SkillUpgrade;
        public SkillUpgradeUI SkillUpgradeUI;

        public Button LVUpgradeButton;
        public LVUpgradePanel LVUpgradeUI;

        [Header("unlock")]
        public GameObject UnlockPanel;
        public GameObject LVUpgPanel;
        public GameObject DescPanel;
        public GameObject StatusPanel;
        public TextMeshProUGUI unlockGradetext;
        public TextMeshProUGUI DescText;
        //public TextMeshProUGUI PieceText;
        //public Image PieceImage;
        public Button unlockButton;

        public void SkillUpgradeButtonInput()
        {
            SkillUpgradeUI.ActiveSkillUpgPanel(this);
               
        }
        public void LVUpgradeButtonInput()
        {
            LVUpgradeUI.ActiveLVPanel(this);
            LVUpgradeUI.SetData(d, info);
           
        }
        private void Awake()
        {
            UpgradeOperator = false;
            SkillUpgradeUI.gameObject.SetActive(false);
            LVUpgradeUI.gameObject.SetActive(false);
            SkillUpgrade.onClick.AddListener(SkillUpgradeButtonInput);
            LVUpgradeButton.onClick.AddListener(LVUpgradeButtonInput);
            GradeButton.onClick.AddListener(GradeUpgradeButtonInput);
            unlockButton.onClick.AddListener(unlockButtonInput);
        }
        void GradeUpgradeButtonInput()
        {
            PieceItemIconUI.SetData(d, info);
            PieceItemIconUI.ActiveGradeUpgUI(this);
        }
        
        public void setUpgradeModel(object o)
        {
            if (o != null)
                upgrademodeling.CreateModel(o as GameObject);
            else
            {
                Debug.Log("Find Error");
            }
        }
        public void set_data(PlayerData d)
        {
            this.d = d;
       
            LoadManager.instance.PushCharacterloadrequest(d.id, (obj) =>
            {
                setUpgradeModel(obj);
    
            });
            LoadManager.instance.PushCharacterdbLoadRequest(d.id, (i) =>
            {
                info = i;
                info.getSkillLV(this.d);
                updateprofil();
     
                Menu.instance.ActiveUI(this.gameObject);
            });
            LoadManager.instance.DataLoadExecute();
        }
        void unlockButtonInput()
        {
            d.characterlock = false;
            updateprofil();
            StartCoroutine(GameManager.instance.saveAccountcorutine(() => { }));
        }
       public void updateprofil()
        {
            if (d.characterlock == true)
            {
                NameText.text = info.name;
                DescText.text = info.Desc;
                string s = null;
                for (int i = 0; i < d.Grade; i++)
                    s += "¡Ú";
                for (int i = 0; i < 5 - d.Grade; i++)
                    s += "¡Ù";
                unlockGradetext.text = s;
                item piece = GameManager.instance.Account.get_item("I" + info.id);
                //if (piece != null)
                //{
                //    if (piece.data.itemSprite != null)
                //        PieceImage.sprite = piece.data.itemSprite;
                //    PieceText.text = piece.Amount + "/" + PieceNeedToUpgradeGrade.returnNeedPiece(info.BaseGrade).ToString();

                //}
                //else
                //{
                //    if (LoadManager.instance.getitem("I" + info.id).itemSprite!=null)
                //        PieceImage.sprite = LoadManager.instance.getitem("I" + info.id).itemSprite;
                //    PieceText.text = 0 + "/" + PieceNeedToUpgradeGrade.returnNeedPiece(info.BaseGrade).ToString();
                //}
                DescPanel.SetActive( true);
                StatusPanel.SetActive(false);
                LVUpgPanel.SetActive(false);
                UnlockPanel.SetActive(true);
                CanUpgradeSkill.gameObject.SetActive(false);
                SkillUpgrade.interactable = false;
                if (info.LB != null)
                {
                    characterskilliconlist[0].set_skill(info.LB);

                    characterskilliconlist[0].gameObject.SetActive(true);
                }
                else
                {
                    characterskilliconlist[0].gameObject.SetActive(false);
                }
                for (int i = 0; i < info.skills.Length; i++)
                {

                    characterskilliconlist[i + 1].set_skill(info.skills[i]);

                    characterskilliconlist[i + 1].gameObject.SetActive(true);


                }
                for (int i = 2; i > info.skills.Length; i--)
                {
                    characterskilliconlist[i].gameObject.SetActive(false);
                    characterskilliconlist[i].skill = null;
                }
               




            }
            else
            {
                DescPanel.SetActive(false);
                StatusPanel.SetActive(true);
                LVUpgPanel.SetActive(true);
                UnlockPanel.SetActive(false);
                SkillUpgrade.interactable = true;
                LvText.text = d.LV.ToString();
                NameText.text = d.Name;
                string s = null;
                for (int i = 0; i < d.Grade; i++)
                    s += "¡Ú";
                for (int i = 0; i < 5 - d.Grade; i++)
                    s += "¡Ù";
                GradeText.text = s;
                ExpText.text = d.EXP + "/" + d.getmaxexp(d.LV);
                ExpSlider.fillAmount= d.EXP/ d.getmaxexp(d.LV);
                if (info != null)
                {
                    PositionText.text =info.position.ToString();
                    HPText.text =  (info.BaseHP + info.HP_Grownth * d.LV * d.Grade).ToString();
                    ATKText.text = (info.BaseATK + info.ATK_Grownth * d.LV * d.Grade).ToString();
              
                    if (info.LB != null)
                    {
                        characterskilliconlist[0].set_skill(info.LB, this);
                        characterskillupgradeiconlist[0].set_skill(info.LB);
                        characterskilliconlist[0].gameObject.SetActive(true);
                    }
                    else
                    {
                        characterskilliconlist[0].gameObject.SetActive(false);
                    }
                    for (int i = 0; i < info.skills.Length; i++)
                    {

                        characterskilliconlist[i + 1].set_skill(info.skills[i], this);
                        characterskillupgradeiconlist[i + 1].set_skill(info.skills[i]);
                        characterskilliconlist[i + 1].gameObject.SetActive(true);


                    }
                    for (int i = 2; i > info.skills.Length; i--)
                    {
                        characterskilliconlist[i].gameObject.SetActive(false);
                        //characterskillupgradeiconlist[i].gameObject.SetActive(false);
                    }
                }
                if (UpgradeOperator)
                {
                    CanUpgradeSkill.gameObject.SetActive(true);
                }
                else
                {
                    CanUpgradeSkill.gameObject.SetActive(false);
                }
            }

        }
       
    }
}