using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using menu;
//public int[] PieceNeedToUpgradeGrade = new int[5] { 20, 50, 100, 180, 250 };

public static class PieceNeedToUpgradeGrade
{
   public static int returnNeedPiece(int n)
    {
        switch (n)
        {
            case 1:
                return 20;

            case 2:
                return 50;

            case 3:
                return 100;

            case 4:
                return 180;

            case 5:
                return 250;

            default:
                return 0;

        }
    }
}
public class CharacterGradeUpgradeUI : MonoBehaviour
{

    public GameObject invisibleBG;
    PlayerData PData;
    CharacterInformation Cinfo;
    CharacterUpgradeUI upgui;
    public Button BackButton;

    [Header("현재 능력치")]
    public TextMeshProUGUI currentGrade;
    public TextMeshProUGUI currentHP;
    public TextMeshProUGUI currentATK;

    [Header("강화 후 능력치")]
    public TextMeshProUGUI AfterUpgGrade;
    public TextMeshProUGUI AfterUpgHP;
    public TextMeshProUGUI AfterUpgATK;

    [Header("강화 수치")]
    public TextMeshProUGUI UpgHP;
    public TextMeshProUGUI UpgATK;

    [Header("피스")]
    public TextMeshProUGUI PieceStatus;
    public PieceItemIconUI iconUI;
    item piece;

    int pieceUpgradeNumber;

    public Button UpgButton;


    public void ActiveGradeUpgUI(CharacterUpgradeUI upgui)
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
    public void SetData(PlayerData d, CharacterInformation info)
    {
        PData = d;
        Cinfo = info;
        iconUI.setItem("I05");
           

        UpdateCurrentData();
    }
    void UpdateCurrentData()
    {
        string s = "";
        for (int i = 0; i < PData.Grade; i++)
            s += "★";
        for (int i = 0; i < 5 - PData.Grade; i++)
            s += "☆";
        currentGrade.text =s;
        currentATK.text = (Cinfo.BaseATK + Cinfo.ATK_Grownth * PData.LV* PData.Grade).ToString();
        currentHP.text = (Cinfo.BaseHP + Cinfo.HP_Grownth * PData.LV* PData.Grade).ToString();
        s = "";
        for (int i = 0; i < PData.Grade+1; i++)
            s += "★";
        for (int i = 0; i < 5 -( PData.Grade+1); i++)
            s += "☆";
        AfterUpgGrade.text =s;
        AfterUpgATK.text = (Cinfo.BaseATK + Cinfo.ATK_Grownth * PData.LV * (PData.Grade+1)).ToString();
        AfterUpgHP.text = (Cinfo.BaseHP + Cinfo.HP_Grownth * PData.LV * (PData.Grade + 1)).ToString();
        UpgHP.text= (int.Parse(AfterUpgHP.text) - int.Parse(currentHP.text)).ToString();
        UpgATK.text = (int.Parse(AfterUpgATK.text) - int.Parse(currentATK.text)).ToString();
        pieceUpgradeNumber = PieceNeedToUpgradeGrade.returnNeedPiece(PData.Grade);
        piece = GameManager.instance.Account.get_item("I05");
        if(piece!=null)
        PieceStatus.text = piece.Amount + "/" + pieceUpgradeNumber;
        else
            PieceStatus.text =   "0/" + pieceUpgradeNumber;
        if (piece==null||piece.Amount < pieceUpgradeNumber)
            UpgButton.interactable = false;
        else
            UpgButton.interactable = true;

    }
    private void Awake()
    {
        BackButton.onClick.AddListener(BackButtonInput);
        UpgButton.onClick.AddListener(UpgradeButtonInput);
    }
    void UpgradeButtonInput()
    {
        StartCoroutine(GameManager.instance.saveAccountcorutine(() =>
        {
            piece.Amount -= pieceUpgradeNumber;
            PData.Grade++;
            UpdateCurrentData();
        }));
        upgui.updateprofil();
    }
}
