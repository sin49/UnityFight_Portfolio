using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.IO;

//다운로드 관련
public class DownLoadManager:MonoBehaviour
{
    //public event AsyncCompletedEventHandler DownloadFileCompleted;
    //public event DownloadProgressChangedEventHandler DownloadProgressChanged;

    //bool OnDownLoad;
    bool DownLoadBtnPush;
    long DownLoadedByte;

    string[] url;
    string[] FileName;
    int Filebyte;
    public void getDownloadInfo(string[] url, string[]filename,int filebyte)
    {
        this.url = url;
        this.FileName = filename;
        this.Filebyte = filebyte;
    }
    public static DownLoadManager instance;
    public TextMeshProUGUI DownloadTaskText;
    public Button DownLoadBtn;
    public Slider ProgressBar;
    public TextMeshProUGUI ProgressText;
    public Queue<long> DownLoadByteQueue = new Queue<long>();
 
    void intialize()
    {
        //OnDownLoad = false;
        DownLoadBtnPush = false;
        DownLoadedByte = 0;

    }
    private void Start()
    {
        
    }
    public void StartDownload(string[] URL, string[] FileName,int FIleByte)
    {
       
        StartCoroutine(DownLoadBundle(URL, FileName, FIleByte));
    }
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        DownLoadBtn.onClick.AddListener(() => { DownLoadBtnPush = true;
            DownloadTaskText.transform.parent.gameObject.SetActive(false);
            ProgressBar.transform.parent.gameObject.SetActive(true);
        });
    }

    #region UnityWebRequest 구현
 public   IEnumerator DownLoadBundle()
    {

        intialize();
        Debug.Log("2");
        yield return StartCoroutine(DownLoadFileByteCalculate(Filebyte));


        for (int n = 0; n < url.Length; n++)
        {
            var client = UnityWebRequest.Get(url[n]);
            client.SendWebRequest();

            while (!client.isDone)
            {
                ProgressBar.minValue = DownLoadedByte + (long)client.downloadedBytes;
                yield return null;
            }
            DownLoadedByte = (long)client.downloadedBytes;
            ProgressBar.minValue = DownLoadedByte;
            File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\Assets\\StreamingAssets" + "\\Assets" + $"\\{FileName[n]}", client.downloadHandler.data);
        }
        yield return new WaitForSeconds(0.5f);
        ProgressBar.transform.parent.gameObject.SetActive(false);
        GameManager.instance.ServerMessage = "Asset Download Complete";
    }
    IEnumerator DownLoadBundle(string[] URL, string[] FileName,int FileByte)
    {

        intialize();
        Debug.Log("2");
        yield return StartCoroutine(DownLoadFileByteCalculate(FileByte));


        for (int n = 0; n < URL.Length; n++)
        {
            var client = UnityWebRequestAssetBundle.GetAssetBundle(URL[n]);
            client.SendWebRequest();
            
            while (!client.isDone)
            {
                ProgressBar.minValue = DownLoadedByte + (long)client.downloadedBytes;
                yield return null;
            }
            DownLoadedByte = (long)client.downloadedBytes;
            ProgressBar.minValue = DownLoadedByte;
            File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\Assets\\StreamingAssets" + "\\Assets" + $"\\{FileName[n]}", client.downloadHandler.data);
        }
        ProgressBar.transform.parent.gameObject.SetActive(false);
        GameManager.instance.ServerMessage = "Asset Download Complete";
    }
    #endregion
    //#region c#구현
    //public IEnumerator DownLoadBundlebyC(string[] URL,string Directory,int FileByte)
    //{

    //    //intialize();
    //    yield return StartCoroutine(DownLoadFileByteCalculate(FileByte));
    //    for(int n = 0; n < URL.Length; n++)
    //    {
    //        WebClient client = new WebClient();
    //        client.DownloadFileAsync(new Uri(URL[n]), Directory);
    //        OnDownLoad = true;
            
    //        yield return new WaitUntil(() => OnDownLoad = false);
    //    }
      
        
    //}
    //void fileDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    //{

    //    // e.BytesReceived
    //    //   받은 데이터의 크기를 저장합니다.
    //    ProgressBar.minValue =DownLoadByte+ e.BytesReceived;
    //    // e.TotalBytesToReceive
    //    //   받아야 할 모든 데이터의 크기를 저장합니다.




    //    ProgressText.text = $"{((ProgressBar.minValue / ProgressBar.maxValue) *100).ToString()}%";



    //}

    //void fileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
    //{
    //    //다운로드 ui 제거

    //    DownLoadByte += DownLoadByteQueue.Dequeue();


    //    OnDownLoad = false;
    //    DownloadTaskText.transform.parent.gameObject.SetActive(false);
    //}
    //#endregion
    #region 파일 용량 계산
    IEnumerator DownLoadFileByteCalculate(int FileByte)
    {
        long DownLoadByteMax = FileByte * 1024;
        string s = "kb";
        float DownLoadByte = FileByte;
        if (FileByte > 1024)
        {
           s = "mb";
            DownLoadByte = MathF.Round((DownLoadByte / 1024), 1);
            if (DownLoadByte > 1024)
            {
                s = "gb";
                DownLoadByte = MathF.Round((DownLoadByte / 1024), 1);
            }
        }
        
        ProgressBar.maxValue = DownLoadByteMax;
        Debug.Log("다운로드 바이트" + DownLoadByte);
        DownloadTaskText.text = $"{DownLoadByte}{s}의 데이터를 다운로드 합니다";
        DownloadTaskText.transform.parent.gameObject.SetActive(true);
        //다운로드 버튼 확인시
        yield return new WaitUntil(() => { return DownLoadBtnPush; });
    }
    #endregion

}
