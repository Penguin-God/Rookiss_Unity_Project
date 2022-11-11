using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_GamePausePopup : UI_Popup
{
    enum GameObjects
    {
        BG,
    }

    enum Buttons
    {
        ExitButton,
        ContinueButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
        GetButton((int)Buttons.ContinueButton).gameObject.BindEvent(OnClickContinueButton);

        GetObject((int)GameObjects.BG).gameObject.BindEvent(OnClickContinueButton);
        Managers.Sound.Play(Sound.Effect, "Sound_HomeButton");

        return true;
    }

    #region EventHandler
    void OnClickExitButton()
    {
        
        Managers.Scene.ChangeScene(Define.SceneType.SelectStageScene);

        
    }

    void OnClickContinueButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
    #endregion
}
