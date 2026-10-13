using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Damage_font : MonoBehaviour
{
    TextMeshProUGUI damage_font;
    public float pawning_time;
    float _pawning_time;
    Vector3 character;
    Color c;

    private void OnEnable()
    {
        _pawning_time = 0;
   
        c = damage_font.color;
    }
    private void OnDisable()
    {
        character=Vector3.zero;
       
        c.a = 1;
        damage_font.color = c;
    }
    private void Awake()
    {
        damage_font = this.GetComponent<TextMeshProUGUI>();
    }
   
    public void set_font(int a,Transform chr)
    {
        damage_font.text ="-"+ a.ToString();
        character = chr.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        c.a = 1 - (_pawning_time / pawning_time);
        damage_font.color = c;
        this.transform.position = Camera.main.WorldToScreenPoint(character + new Vector3(0, 1f, 0.7f)+ new Vector3(0, 0.4f , 0.2f) * _pawning_time);
        _pawning_time += Time.deltaTime;
        if (_pawning_time >= pawning_time)
        {
            this.gameObject.SetActive(false);
        }
    }
}
