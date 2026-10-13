using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.PlayerLoop.EarlyUpdate;

public class GameClearUI : MonoBehaviour
{
    public GameClearCharacterIcon[] chricons = new GameClearCharacterIcon[5];
    public Transform ScrollContent;

    public ItemIconUI itemicon;
    List<GameObject> itemiconUI=new List<GameObject>();
    Vector3 ScrollContentPos;
    private void Awake()
    {
        ScrollContentPos = ScrollContent.position;
    }
    public void getPlayerData(PlayerData[] player, int exp)
    {
        for(int i=0; i<player.Length; i++)
        {
            chricons[i].gameObject.SetActive(true);
            chricons[i].ActiveUI(exp, player[i]);
        }
        for(int n = player.Length; n < 5; n++)
        {
            chricons[n].gameObject.SetActive(false);
        }
    }
   
    public void getStageReward(List<item> list)
    {
        foreach(item i in list)
        {
        
          GameObject obj=  Instantiate(itemicon.gameObject, ScrollContent);
            obj.GetComponent<ItemIconUI>().setItem(i);
            itemiconUI.Add(obj);
            obj.SetActive(false);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(ClearEffectCorutine());
    }
    IEnumerator ClearEffectCorutine()
    {
        int tmp = 0;
   

        for (int n = 0; n < itemiconUI.Count; n++)
        {
           
            if (n > 4)
            {
                ScrollContent.position = ScrollContent.position - (Vector3.right * 100);
            }
            itemiconUI[n].SetActive(true);

        }
        ScrollContent.position = ScrollContentPos;

        foreach (GameClearCharacterIcon i in chricons)
        {
            StartCoroutine(i.ExpUpdate());
        }
        while (true)
        {
            yield return null;
            if (Input.GetMouseButton(0)){
                foreach (GameClearCharacterIcon i in chricons)
                {
                    i.skipAnim();
                }
                Debug.Log("캐릭터 UI 애니 스킵");
                break;
            }
             foreach (GameClearCharacterIcon i in chricons)
            {
                if (i.AnimCheck)
                    tmp++;
            }
            if (tmp == chricons.Length)
                break;
            else
                tmp = 0;
        
        }
        
    }
  
}
