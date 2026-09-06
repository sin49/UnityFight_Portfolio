using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iAct
{
    void Active(int ATK, List<GameCharacter> chr = null);
   void Active(int ATK, int LV, ActionClass actionClass, List<GameCharacter> chr);
    bool actionchecker();
}
