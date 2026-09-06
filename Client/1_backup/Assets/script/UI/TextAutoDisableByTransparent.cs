using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextAutoDisableByTransparent : MonoBehaviour
{
    TextMeshProUGUI Text;
    public float TextDisableTimer=2;
    float TextDisableSpeed { get; set; }
    private void Awake()
    {
        Text = this.GetComponent<TextMeshProUGUI>();
        TextDisableSpeed = 1 / TextDisableTimer;
    }
    private void OnEnable()
    {
        Text.color = new Vector4(Text.color.r, Text.color.g, Text.color.b, 1.0f);
    }
    void Update()
    {
        Text.color = new Vector4(Text.color.r, Text.color.g, Text.color.b, Text.color.a - TextDisableSpeed * Time.deltaTime);
        if (Text.color.a <= 0)
            this.gameObject.SetActive(false);
    }
}
