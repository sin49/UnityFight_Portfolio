using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetActionType 
{
   public bool checkType(string s)
    {
        try
        {
           var a= Activator.CreateInstance(Type.GetType(s)) as iAct;

            if (a.actionchecker())
                return true;
            else
                return false;
        }
        catch
        {
            return false;
        }
    }
}
