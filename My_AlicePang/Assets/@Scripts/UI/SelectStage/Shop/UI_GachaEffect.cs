using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_GachaEffect : UI_Base
{
    enum GameObjects
    {
        Gacha1_Normal,
        Gacha1_Pressed,
        Gacha10_Normal,
        Gacha10_Pressed,
    }

    enum Buttons
    {
        Gacha1_Button,
        Gacha10_Button,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.Gacha1_Button).gameObject.BindEvent(OnClickGacha1_Button);
        GetButton((int)Buttons.Gacha1_Button).gameObject.BindEvent(OnPressGacha1_Button, Define.UIEvent.Press);

        GetObject((int)GameObjects.Gacha1_Pressed).SetActive(false);
        GetObject((int)GameObjects.Gacha10_Pressed).SetActive(false);

        return true;
    }

    #region EventHandler

    void OnClickGacha1_Button()
    {
        GetObject((int)GameObjects.Gacha1_Pressed).SetActive(false);
        GetObject((int)GameObjects.Gacha1_Normal).SetActive(true);
    }

    void OnPressGacha1_Button()
    {
        GetObject((int)GameObjects.Gacha1_Pressed).SetActive(true);
        GetObject((int)GameObjects.Gacha1_Normal).SetActive(false);
    }

    void OnClickGacha10_Button()
    {

        GetObject((int)GameObjects.Gacha10_Pressed).SetActive(false);
        GetObject((int)GameObjects.Gacha10_Normal).SetActive(true);
    }

    void OnPressGacha10_Button()
    {
        GetObject((int)GameObjects.Gacha10_Pressed).SetActive(true);
        GetObject((int)GameObjects.Gacha10_Normal).SetActive(false);
    }

    #endregion
}
