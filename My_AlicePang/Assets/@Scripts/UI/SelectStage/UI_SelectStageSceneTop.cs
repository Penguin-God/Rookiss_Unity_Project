using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SelectStageSceneTop : UI_Base
{
    enum Texts
    {
        CoinText,
        DiaText,
    }

    enum Buttons
    {
        CoinPlusButton,
        DiaPlusButton,
    }

    UI_SelectStageScene _selectStageSceneUI;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CoinPlusButton).gameObject.BindEvent(OnClickCoinPlusButton);
        GetButton((int)Buttons.DiaPlusButton).gameObject.BindEvent(OnClickDiaPlusButton);
        
        Refresh();

        return true;
    }

    public void SetInfo(UI_SelectStageScene sceneUI)
    {
        _selectStageSceneUI = sceneUI;
        Refresh();
    }

    public void Refresh()
    {
        if (_init == false)
            return;

        GetText((int)Texts.CoinText).text = Managers.Game.Coin.ToString();
        GetText((int)Texts.DiaText).text = Managers.Game.Dia.ToString();
    }

    #region EventHandler

    void OnClickCoinPlusButton()
    {
        Debug.Log("OnClickCoinPlusButton");
    }

    void OnClickDiaPlusButton()
    {
        Debug.Log("OnClickDiaPlusButton");
    }

    #endregion
}
