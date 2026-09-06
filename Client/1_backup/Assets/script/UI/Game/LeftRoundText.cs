using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeftRoundText : MonoBehaviour,StageObserver
{
    TextMeshProUGUI s;

    public void UpdateInformation(int n)
    {
        StartCoroutine(RoundTextColorChanged());
    }
    IEnumerator RoundTextColorChanged()
    {
        s.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        s.color = Color.black;
    }
    // Update is called once per frame
    private void Awake()
    {
        s = this.gameObject.GetComponent<TextMeshProUGUI>();
      
    }
    private void Start()
    {
        Stage._stage.AddObserver(this);
    }
    void Update()
    {
        s.text = Stage._stage.current_round_number.ToString()+"/"+ Stage._stage.info.waveNumber.ToString();
    }
}
