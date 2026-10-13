using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DescriptionPopUp : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public void ActivePopUp(string name, Transform t)
    {
    
        this.transform.position = t.position + (Vector3.up * 100);
        Name.text = name;

    }
    public void ActivePopUp(string name ,string desc,Transform t)
    {
  
        this.transform.position = Camera.main.WorldToViewportPoint(t.position + (Vector3.up * 0.2f));
        Name.text = name;
        Description.text = desc;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            this.gameObject.SetActive(false);
        }
    }
}
