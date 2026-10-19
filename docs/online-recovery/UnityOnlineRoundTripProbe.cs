// Optional Unity Play verification helper. Keep outside Assets until you want to run it.
// Copy into a temporary Assets folder, attach on the Title GameManager object,
// select Online mode, then use this component's context menu during Play.
using System;
using System.Collections;
using UnityEngine;

public class UnityOnlineRoundTripProbe : MonoBehaviour
{
    [ContextMenu("Run online account roundtrip")]
    public void RunProbe()
    {
        if (!Application.isPlaying || GameManager.instance == null ||
            GameManager.instance.RunMode != ClientRunMode.Online)
        {
            Debug.LogError("Run on Title during Play with GameManager Run Mode = Online.");
            return;
        }
        if (running) return;
        StartCoroutine(Probe());
    }

    bool running;
    IEnumerator Probe()
    {
        running = true;
        try
        {
            var manager = GameManager.instance;
            string name = "online_probe_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            yield return manager.createaccountcorutine(name);
            if (manager.OnError || manager.Account == null) yield break;
            string uid = manager.Account.uid;
            string expected = name + "_saved";
            manager.Account.username = expected;
            manager.Account.Exp = 41;
            yield return manager.saveAccountcorutine(null);
            if (manager.OnError) yield break;
            yield return manager.ConnectAccountCorutine(uid);
            if (manager.OnError || manager.Account == null) yield break;
            if (manager.Account.uid != uid || manager.Account.username != expected || manager.Account.Exp != 41)
            {
                Debug.LogError("FAIL online saved account differs after reload.");
                yield break;
            }
            Debug.Log("PASS UNITY ONLINE account create/save/reload UID=" + uid);
        }
        finally { running = false; }
    }
}
