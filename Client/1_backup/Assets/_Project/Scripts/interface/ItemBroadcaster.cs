using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ItemBroadcaster 
{
    void AddObserver(ItemObserver s);
    void DeleteObserver(ItemObserver s);

    void notifyObserver(item n);
}
