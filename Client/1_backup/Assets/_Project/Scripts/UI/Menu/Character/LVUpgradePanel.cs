using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace menu
{
    public class LVUpgradePanel : MonoBehaviour
    {
        PlayerData PData;
        CharacterInformation Cinfo;

        public GameObject invisibleBG;
        public Button BackButton;

        [Header("현재 능력치")]
        public TextMeshProUGUI currentLV;
        public TextMeshProUGUI currentHP;
        public TextMeshProUGUI currentATK;

        [Header("강화 후 능력치")]
        public TextMeshProUGUI AfterUpgLV;
        public TextMeshProUGUI AfterUpgHP;
        public TextMeshProUGUI AfterUpgATK;

        [Header("강화 수치")]
        public TextMeshProUGUI UpgHP;
        public TextMeshProUGUI UpgATK;

        [Header("경험치")]
        public TextMeshProUGUI AddExp;
        public TextMeshProUGUI Exp;

        public Button UpgButton;
        public Slider expSlider;
        public GameObject Arrow;
        public GameObject NextLevelText;

        public int UpgradeExp;
        int upgradeLv;
        int calculateexp;
        int calculatemaxexp;
        int calculatemaxexpBackup;
        int exptextint;
        int maxEXP;
        float ExpsliderValue;
        int ExpsliderLVAmount;
        float ExpsliderTimeweight=5f;
        int ADDEXP;
        CharacterUpgradeUI upgui;
        //List<int> ExpsliderMaxValueList;

        public ExpItemPanel[] ExpItems = new ExpItemPanel[3];

        public void ActiveLVPanel(CharacterUpgradeUI upgui)
        {
            this.gameObject.SetActive(true);
            invisibleBG.SetActive(true);
                this.upgui = upgui;
        }
        public void BackButtonInput()
        {
            this.gameObject.SetActive(false);
            invisibleBG.SetActive(false);
        }
        private void OnDisable()
        {
            calculatemaxexpBackup = 0;
            UpgradeExp = 0;
            upgradeLv = 0;
        }
        private void FixedUpdate()
        {
           
            if(ExpsliderLVAmount+PData.LV<PData.LV+upgradeLv)
            {
                ExpsliderValue += Time.deltaTime * (Mathf.Abs(ADDEXP)) * ExpsliderTimeweight;
                if (ExpsliderValue >= expSlider.maxValue)
                {
                    ExpsliderValue = 0;
                    ExpsliderLVAmount++;
                    expSlider.maxValue = PData.getmaxexp(ExpsliderLVAmount + PData.LV);
                }
            }else if (ExpsliderLVAmount + PData.LV == PData.LV + upgradeLv )
            {
                if (ExpsliderValue < exptextint)
                {
                    ExpsliderValue += Time.deltaTime *(Mathf.Abs(ADDEXP))* ExpsliderTimeweight;
                    if(ExpsliderValue > exptextint)
                        ExpsliderValue = exptextint;
                }
                else if (ExpsliderValue > exptextint)
                {
                    ExpsliderValue -= Time.deltaTime * (Mathf.Abs(ADDEXP)) * ExpsliderTimeweight;
                    if (ExpsliderValue < exptextint)
                        ExpsliderValue = exptextint;
                }
            }
            else if(ExpsliderLVAmount + PData.LV > PData.LV + upgradeLv)
            {
                ExpsliderValue -= Time.deltaTime * (Mathf.Abs(ADDEXP)) * ExpsliderTimeweight;
                if (ExpsliderValue <= 0)
                {
                    
                    ExpsliderLVAmount--;
                    expSlider.maxValue = PData.getmaxexp(ExpsliderLVAmount + PData.LV);
                    ExpsliderValue = (int)expSlider.maxValue;
                }
            }
            expSlider.value = ExpsliderValue;
        }
        public void UpgradeButtonInput()
        {
          
            StartCoroutine(GameManager.instance.saveAccountcorutine(() =>
            {
                PData.getEXP(UpgradeExp);
                foreach (ExpItemPanel ui in ExpItems)
                {
                    ui.upgradebuttoninput();
                }
                UpdateCurrentData();
                UpdateNextLevelData();

                upgui.updateprofil();
            }));
           
        }
        public void SetData(PlayerData d, CharacterInformation info)
        {
            PData = d;
            Cinfo = info;

            UpdateCurrentData();
            UpdateNextLevelData();
        }
        void UpdateCurrentData()
        {
            currentLV.text = PData.LV.ToString();
            currentATK.text = (Cinfo.BaseATK + Cinfo.ATK_Grownth * PData.LV*PData.Grade).ToString();
            currentHP.text = (Cinfo.BaseHP + Cinfo.HP_Grownth * PData.LV * PData.Grade).ToString();
            
            maxEXP = PData.getmaxexp(PData.LV);
           
            calculatemaxexpBackup = 0;
            calculateexp = PData.EXP;
            calculatemaxexp = maxEXP;
       
            UpgradeExp = 0;
            upgradeLv = 0;
            ExpsliderLVAmount = 0;
            expSlider.maxValue = maxEXP;
            expSlider.value = ExpsliderValue = exptextint = PData.EXP;
            UpgButton.interactable = false;
            AddExp.gameObject.SetActive(false);
            Exp.text = PData.EXP + "/" + maxEXP;
        }
        
        void UpdateNextLevelData()
        {
            if (upgradeLv > 0)
            {
                if (!AfterUpgLV.gameObject.activeSelf)
                {
                    AfterUpgLV.gameObject.SetActive(true);
                    AfterUpgHP.gameObject.SetActive(true);
                    AfterUpgATK.gameObject.SetActive(true);
                    UpgHP.gameObject.SetActive(true);
                    UpgATK.gameObject.SetActive(true);
                    Arrow.gameObject.SetActive(true);
                    NextLevelText.SetActive(true);
                }
                AfterUpgLV.text = (PData.LV + upgradeLv).ToString();
                AfterUpgATK.text = (Cinfo.BaseATK + Cinfo.ATK_Grownth * (PData.LV + upgradeLv) * PData.Grade).ToString();
                AfterUpgHP.text = (Cinfo.BaseHP + Cinfo.HP_Grownth * (PData.LV + upgradeLv) * PData.Grade).ToString();
                UpgATK.text = (Cinfo.ATK_Grownth * PData.Grade * upgradeLv).ToString();
                UpgHP.text = (Cinfo.HP_Grownth * PData.Grade * upgradeLv).ToString();

            }
            else
            {
                if (AfterUpgLV.gameObject.activeSelf)
                {
                    AfterUpgLV.gameObject.SetActive(false);
                    AfterUpgHP.gameObject.SetActive(false);
                    AfterUpgATK.gameObject.SetActive(false);
                    UpgHP.gameObject.SetActive(false);
                    UpgATK.gameObject.SetActive(false);
                  
                    Arrow.gameObject.SetActive(false);
                    NextLevelText.SetActive(false);
                }
            }

        }
        public void GetExp(int exp)
        {
            ADDEXP = exp;
            UpgradeExp += exp;
            exptextint += exp;
            calculateexp = PData.EXP + UpgradeExp;
            if (UpgradeExp > 0)
            {
                if (!AddExp.gameObject.activeSelf)
                    AddExp.gameObject.SetActive(true);
                UpgButton.interactable = true;
                AddExp.text = "+"+UpgradeExp.ToString();

            }
            else
            {
                if (AddExp.gameObject.activeSelf)
                    AddExp.gameObject.SetActive(false);
                UpgButton.interactable = false;
            }
          
            if (calculateexp > calculatemaxexp)
            {
                while (calculateexp > calculatemaxexp)
                {
                    upgradeLv++;
                    exptextint -= maxEXP;
                    calculatemaxexpBackup = calculatemaxexp;
                    maxEXP = PData.getmaxexp(PData.LV + upgradeLv);

                    calculatemaxexp += maxEXP;
                }
                UpdateNextLevelData();
            }
            else if (calculateexp < calculatemaxexpBackup)
            {
                while (calculateexp < calculatemaxexpBackup)
                {
                    calculatemaxexp = calculatemaxexpBackup;
                    upgradeLv--;
                    maxEXP = PData.getmaxexp(PData.LV + upgradeLv);
                    calculatemaxexpBackup = calculatemaxexpBackup - maxEXP;
                    exptextint = maxEXP - (calculatemaxexp - calculateexp);

                }


                UpdateNextLevelData();


            }
            if (UpgradeExp > 0)
            {
                Exp.text = "<color=#00FF00>" + exptextint + "</color>/" + maxEXP;
            }
            else
            {
                Exp.text = PData.EXP + "/" + maxEXP;

            }
            //Exp.text = PData.EXP + "/" + maxEXP;

        }
    
        private void Awake()
        {
            
            BackButton.onClick.AddListener(BackButtonInput);
            UpgButton.onClick.AddListener(UpgradeButtonInput);


            //ExpsliderMaxValueList = new List<int>();


            if (GameManager.instance.Account.get_item("I02") == null)
            {

                ExpItems[0].setItem(new item() { Amount = 0, itemid = "I02", data = LoadManager.instance.getitem("I02") }, 100, this);
            }
            else
            {
                ExpItems[0].setItem(GameManager.instance.Account.get_item("I02"), 100, this);
            }

            if (GameManager.instance.Account.get_item("I03") == null)
            {

                ExpItems[1].setItem(new item() { Amount = 0, itemid = "I03", data = LoadManager.instance.getitem("I03") }, 1000, this);
            }
            else
            {
                ExpItems[1].setItem(GameManager.instance.Account.get_item("I03"), 1000, this);
            }

            if (GameManager.instance.Account.get_item("I04") == null)
            {

                ExpItems[2].setItem(new item() { Amount = 0, itemid = "I04", data = LoadManager.instance.getitem("I04") }, 10000, this);
            }
            else
            {
                ExpItems[2].setItem(GameManager.instance.Account.get_item("I04"), 10000, this);
            }



        }
    }
}