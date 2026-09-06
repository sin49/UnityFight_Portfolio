using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace menu
{
    public class itemListIconUI : ItemIconUI
    {
       

        public virtual void set_Data(item d, Transform t)
        {
            setItem(d);
            ItemButton.onClick.AddListener(() =>
            {
                if (!LoadManager.instance.onload)
                {
                    t.GetComponent<itemUI>().SelectItem = d;
                }
            });
      
        }
        
      
    }
}
