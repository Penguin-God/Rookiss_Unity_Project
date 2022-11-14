using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    UI_TitleScene _titleSceneUI;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.SceneType.TitleScene;

        Managers.UI.ShowSceneUI<UI_TitleScene>(callback: (titleSceneUI) =>
        {
            _titleSceneUI = titleSceneUI;
        });

        StartCoroutine(CoWaitLoad());

        return true;
    }

    IEnumerator CoWaitLoad()
    {
        while (Managers.Data.Loaded() == false)
            yield return null;

        while (Managers.UI.SceneUI == null)
            yield return null;

        while (Managers.Game.IsLoaded == false)
            yield return null;

        _titleSceneUI.ReadyToStart();
    }
}
