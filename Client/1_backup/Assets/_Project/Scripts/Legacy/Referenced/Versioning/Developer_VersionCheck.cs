using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class Developer_VersionCheck : MonoBehaviour
{
    public TextMeshProUGUI VersionText;
    public Button UpdateButton;
    public TMP_InputField inputfield;
    [HideInInspector]
   public string FilePath;
    [HideInInspector]
    public string FileName;
            [HideInInspector]
   public string Version="";
  
    public void setText(string FileName,string FilePath,string Version)
    {
        this.Version = Version;

        this.FileName = FileName;
        this.FilePath = FilePath;

        inputfield.text = Version;
        VersionText.text = FileName + " 버전:" + Version;
        UpdateButton.onClick.AddListener(() =>
        {
            Debug.Log("갱신 완료\n"+ "주의)해당 에셋 번들 다시 생성할 것");
            Version = inputfield.text;
            VersionText.text = FileName + " 버전:" + Version;
            File.WriteAllText(FilePath, "Version:"+Version);
        });
        
    }
   
}
