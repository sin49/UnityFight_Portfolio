using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ScriptMessageBox : MonoBehaviour
{
    public TextMeshProUGUI Messagebox;
protected bool MessageBoxOutPutComplete;
    public float MessageOutputHighSpeed = 0.2f;
    public float MessageOutputnormalSpeed = 0.5f;
    public float MessageOutputslowSpeed = 1f;
    public float MessageOutputSpeed;
    public float MessageTouchDelayTime;
  protected  string MessageScript;
    public int index = 0;
    private void Awake()
    {
        Messagebox.text = null;

        //임시ㅗㅠㅎㅍ
        MessageOutputSpeed = MessageOutputnormalSpeed;
    }
   public string textscript;
  public virtual  void MessageBoxOutPutCompleteCheck()
    {
        MessageBoxOutPutComplete = true;
    }
    public void riseTextindex() {
        index++;
        Debug.Log("index가 올랐습니다:" + index);
        textscript = TextScriptReader.SliceTextScript(MessageScript, index.ToString());
    }
  public virtual  IEnumerator OutputMessageBox(string script)
    {
        int length = script.Length;
        MessageBoxOutPutComplete = false;
        MessageTouchDelayTime = 0.5f;
        Messagebox.text = "";
        for (int n = 0; n < length; n++)
        {
            if (Input.GetMouseButton(0)&& MessageTouchDelayTime<=0)
            {
                Messagebox.text = script;
                MessageBoxOutPutCompleteCheck();
                yield break;
            }
            if(MessageTouchDelayTime>0)
            MessageTouchDelayTime -= Time.deltaTime;
            Messagebox.text += script[n];
            yield return new WaitForSecondsRealtime(MessageOutputSpeed);
        }
        MessageBoxOutPutCompleteCheck();
    }
    public virtual void UpdateMessageBox()
    {
        
        if (MessageBoxOutPutComplete)
        {
            riseTextindex();
            StartCoroutine(OutputMessageBox(textscript));
        }
        
    }
  
    
}
