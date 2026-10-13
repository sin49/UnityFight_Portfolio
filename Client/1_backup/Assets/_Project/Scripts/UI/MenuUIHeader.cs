using menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIHeader : MonoBehaviour
{
    //public Button MainButton;

    public Button StageButton;

    public Button CharacterButton;

    public Button ItemButton;

    public Button ShopButton;
    /// <summary>/// 
    ///현제 페이지에 따라 버튼이 눌려졌는지를 판단하는 함수
    /// </summary>
    public void UpdateButtons(GameObject UI)
    {
   
        //MainButton.interactable = true;
        StageButton.interactable =
            true;
        CharacterButton.interactable =
            true;
        ItemButton.interactable =
            true;
        ShopButton.interactable =
            true;
        //if (UI== Menu.instance.MainUI.gameObject)
        //    MainButton.interactable = false;
      /*  else */if (UI.transform.parent == Menu.instance.StageUI.transform.parent)
            StageButton.interactable = false;
        else if (UI.transform.parent == Menu.instance.CharacterUI.transform.parent)
            CharacterButton.interactable = false;
        else if (UI == Menu.instance.ItemUI.gameObject)
            ItemButton.interactable = false;
        else if (UI == Menu.instance.ShopUI.gameObject)
            ShopButton.interactable = false;
    }
    private void Awake()
    {
        
        //MainButton.onClick.AddListener(() =>
        //{

        //    Menu.instance.ActiveUI(Menu.instance.MainUI.gameObject);


        //});
        StageButton.onClick.AddListener(() =>
        {
            Menu.instance.StageUI.transform.parent.gameObject.SetActive(true);
            Menu.instance.ActiveUI(Menu.instance.StageUI.gameObject);

        });
        CharacterButton.onClick.AddListener(() =>
        {
            Menu.instance.CharacterUI.transform.parent.gameObject.SetActive(true);
            Menu.instance.ActiveUI(Menu.instance.CharacterUI.gameObject);

        });
        ItemButton.onClick.AddListener(() =>
        {


            Menu.instance.ActiveUI(Menu.instance.ItemUI.gameObject);

        });
        ShopButton.onClick.AddListener(() =>
        {

            StartCoroutine(GameManager.instance.RequestShopList((int)Shopid.Daily,
                   () =>
                   {
                       Menu.instance.ActiveUI(Menu.instance.ShopUI.gameObject);
                       Menu.instance.ShopUI.setBasicShopItems(Shopid.Daily);
                   }));


        });
    }
}
