using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StageRoundPanel : MonoBehaviour,StageObserver
{
    public TextMeshProUGUI RoundText;
    Animator ani;

    public void UpdateInformation(int n)
    {
        StartCoroutine(RoundPanelActive(n));
    }

    private void Awake()
    {
       
        ani=this.GetComponent<Animator>();
        Stage._stage.AddObserver(this);
    }
    IEnumerator RoundPanelActive(int n)
    {
        this.gameObject.SetActive(true);
        yield return Stage._stage;
      RoundText.text = "Round" +n;
        ani.Play("RoundPanel");
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
