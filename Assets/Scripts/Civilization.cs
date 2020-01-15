using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Civilization {

    public static string name { get; private set; }

    static Action<string> onNameSet;

    public static void SetName(string name)
    {
        Civilization.name = name;
        if (onNameSet != null)
            onNameSet(name);
    }

    #region Callbacks

    public static void RegisterOnNameSet(Action<string> callback)
    {
        onNameSet += callback;
    }

    public static void UnregisterOnNameSet(Action<string> callback)
    {
        onNameSet -= callback;
    }

    #endregion
}
