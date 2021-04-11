using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console
{
    public static bool isDebug = false;
    public static void Log(string msg)
    {
        if(isDebug)
            Debug.Log(msg);
    }
}
