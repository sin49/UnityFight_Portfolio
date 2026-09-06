using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace menu
{
    public class ExpItemPanel : DoubleButtonHandlerUI
    {
        item i;
        LVUpgradePanel upgradepanel;



        public Button ItemImageButton;

        //public void ButtonInput()
        //{
        //    GameManager.instance.ActiveItemPopUp(i.data, this.transform);
        //}

        int Expgain;
      

        bool onclick;

        bool oclickArrowPlus;
        bool oclickArrowMinus;
        public void upgradebuttoninput()
        {
            i.Amount -= count;
            count = 0;
            MaxCount = i.Amount;
            CountText.text = count + "/" + MaxCount;
        }

        public void ImageButtonClickDown()
        {
            onclick = true;
        }
        public void ImageButtonClickUp()
        {
            onclick = false;
           
            ClickTImer = 0;
        }
        public void ArrowPlusClickDown()
        {
            oclickArrowPlus = true;
        }
        public void ArrowPlusClickUp()
        {
            oclickArrowPlus = false;

            ClickTImer = 0;
        }

        public void ArrowMinusClickDown()
        {
            oclickArrowMinus = true;
        }
        public void ArrowMinusClickUp()
        {
            oclickArrowMinus = false;

            ClickTImer = 0;
        }
        float ClickTImer = 0;
        private void Update()
        {
            if (onclick)
            {
                ClickTImer += Time.deltaTime;
                if (ClickTImer > 0.5f)
                {
                    ClickTImer = 0;
                    GameManager.instance.ActiveItemDescription(i);
                }
            }
            if (oclickArrowPlus)
            {
                ClickTImer += Time.deltaTime;
                if (ClickTImer > 0.5f)
                {
                    PlusButtonInput();

                }
            }
            if (oclickArrowMinus)
            {
                ClickTImer += Time.deltaTime;
                if (ClickTImer > 0.5f)
                {
                    MinusButtonInput();
                }
            }

        }
        protected override void PlusButtonInput()
        {
            if (count < MaxCount)
            {
                base.PlusButtonInput();
                CountText.text = count + "/" + MaxCount;
                upgradepanel.GetExp(Expgain);
            }
        }
        protected override void MinusButtonInput()
        {
            if (count > 0)
            {
                base.MinusButtonInput();
                CountText.text = count + "/" + MaxCount;
                upgradepanel.GetExp(-Expgain);
            }

        }
        
        public void setItem(item i, int expgain, LVUpgradePanel panel)
        {
            this.i = i;
            if (i.data != null&&i.data.itemSprite != null)
                ItemImageButton.GetComponent<Image>().sprite = i.data.itemSprite;
            this.Expgain = expgain;
            MaxCount = i.Amount;
            upgradepanel = panel;
            CountText.text = count + "/" + MaxCount;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            count = 0;
            
        }
    }
}