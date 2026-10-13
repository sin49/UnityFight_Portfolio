using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace menu
{
    public class MainMenuUI : MonoBehaviour
    {




        public Transform Character_Standing;
        public float CharacterRotateSpeed = 0;
        public void MakeStarCharacterModeling(GameObject Character)
        {

            if (Character == null)
                return;
            StarCharacterStanding s = Character_Standing.GetComponent<StarCharacterStanding>();
            s.createstarcharacter(Character, CharacterRotateSpeed);

        }
        //public void MakeStarCharacterModeling(object o)
        //{
        //    GameObject a = o as GameObject;

        //    StarCharacterStanding s = Character_Standing.GetComponent<StarCharacterStanding>();
        //    s.createstarcharacter(a, CharacterRotateSpeed);

        //}




        public TextMeshProUGUI AccountNameText;
        public TextMeshProUGUI PlvText;
        public TextMeshProUGUI Stamina;
        public TextMeshProUGUI UID;
        public Image StaminaSlider;
        //public Slider StaminaSlider;

        public Image ExpSlider;
        public TextMeshProUGUI Exp;

      
        private void Awake()
        {



            
            if (GameManager.instance.Account.StarCharacterModel != null)
                MakeStarCharacterModeling(GameManager.instance.Account.StarCharacterModel) ;
          

        }
        private void Start()
        {
            GameManager.instance.Account.SetMoney(9999999);
        }
        private void FixedUpdate()
        {
            StaminaSlider.fillAmount = GameManager.instance.Account.Stamina / GameManager.instance.Account.MaxStamina;


            ExpSlider.fillAmount = GameManager.instance.Account.Exp / GameManager.instance.Account.Max_Exp;

            AccountNameText.text = GameManager.instance.Account.username;
            PlvText.text = GameManager.instance.Account.accountlv.ToString();
            Stamina.text = GameManager.instance.Account.Stamina.ToString() + "/" + GameManager.instance.Account.MaxStamina.ToString();

            UID.text ="UID:" +GameManager.instance.Account.uid;
            //Exp.text = GameManager.instance.Account.Exp.ToString() + "/" + GameManager.instance.Account.Max_Exp.ToString();


        }

    }
}
