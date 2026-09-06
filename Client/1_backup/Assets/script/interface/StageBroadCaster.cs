using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface StageBroadCaster 
{
    void AddObserver(StageObserver s);
    void DeleteObserver(StageObserver s);

    void notifyObserver();
}
