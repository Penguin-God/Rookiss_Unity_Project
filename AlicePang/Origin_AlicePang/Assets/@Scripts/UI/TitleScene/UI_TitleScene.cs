using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_TitleScene : UI_Scene
{
    enum GameObjects
    {
        BG,
    }

    enum Texts
    {
        StartText,
    }

    bool _isLoaded = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));

        GetObject((int)GameObjects.BG).BindEvent(OnClickBG);
        Managers.Sound.Play(Sound.Effect, "Sound_Opening");        

        return true;
    }

    public void ReadyToStart()
    {
        _isLoaded = true;
        GetText((int)Texts.StartText).enabled = true;
    }

    #region EventHandler
    void OnClickBG()
    {
        if(_isLoaded)
            Managers.Scene.ChangeScene(Define.SceneType.SelectStageScene);

        Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");
    }
    #endregion
}
