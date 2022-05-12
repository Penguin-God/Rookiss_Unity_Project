using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        => Util.GetOrAddComponent<T>(go);

    public static void BindEvnet(this GameObject go, Action<PointerEventData> action, Define.UI_Event type = Define.UI_Event.Click)
        => UI_Base.BindEvnet(go, action, type);
}
