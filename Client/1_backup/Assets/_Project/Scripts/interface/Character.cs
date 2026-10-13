using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Character
{
    void AddObserver(Character_observer s);
    void DeleteObserver(Character_observer s);

    void notifyObserver();
}
