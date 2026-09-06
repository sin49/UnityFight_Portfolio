using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeNameButton : MonoBehaviour
{
    public GameObject nameinput;
    Button b;
    private void Awake()
    {
        nameinput.gameObject.SetActive(false);
        b = this.GetComponent<Button>();
        b.onClick.AddListener(() => nameinput.SetActive(true));
    }
}
