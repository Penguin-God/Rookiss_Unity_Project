using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public static class Extension
{
    public static void Add_UIEvnet(this GameObject go, Action<PointerEventData> action, Define.UI_Event type = Define.UI_Event.Click)
        => UI_Base.Add_UIEvnet(go, action, type);
}
