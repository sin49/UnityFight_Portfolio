using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ChriconUIPlace : MonoBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, Screen.height * 0.2f);
    }
}
