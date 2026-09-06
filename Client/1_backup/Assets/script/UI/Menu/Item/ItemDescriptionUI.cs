using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;
    public Image ItemSprite;
    public void ActiveItem(item i)
    {
        ItemName.text = i.data.Name;
        ItemDescription.text = i.data.desc;
        ItemSprite.sprite = i.data.itemSprite;
        this.gameObject.SetActive(true);
    }
  
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            this.gameObject.SetActive(false);
        }
    }

  
}
