using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DevScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Game;
        Managers.Game.Init();
        Managers.Game.Money = 200000000;
        Managers.UI.ShowPopupUI<UI_PlayPopup>();
        return true;
	}
}
