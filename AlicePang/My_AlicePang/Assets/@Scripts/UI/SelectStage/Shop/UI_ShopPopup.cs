using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Spine.Unity;

public class UI_ShopPopup : UI_Popup
{
    enum GameObjects
    {
        CoinProductScroll,
        DiaProductScroll,
        WeaponGacha,
        SelectCoinTab,
        SelectDiaTab,
        SelectWeaponTab,
        Gacha1_Normal,
        Gacha1_Pressed,
        Gacha10_Normal,
        Gacha10_Pressed,
        Block,
    }

    enum Buttons
    {
        CoinTabButton,
        DiaTabButton,
        WeaponTabButton,
        CloseButton,
        Gacha1_Button,
        Gacha10_Button,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.Block).SetActive(false);

        GetButton((int)Buttons.CoinTabButton).gameObject.BindEvent(OnClickCoinTabButton);
        GetButton((int)Buttons.DiaTabButton).gameObject.BindEvent(OnClickDiaTabButton);
        GetButton((int)Buttons.WeaponTabButton).gameObject.BindEvent(OnClickWeaponTabButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        GetButton((int)Buttons.Gacha1_Button).gameObject.BindEvent(OnClickGacha1_Button);
        GetButton((int)Buttons.Gacha1_Button).gameObject.BindEvent(OnPressGacha1_Button, Define.UIEvent.Press);

        GetButton((int)Buttons.Gacha10_Button).gameObject.BindEvent(OnClickGacha10_Button);
        GetButton((int)Buttons.Gacha10_Button).gameObject.BindEvent(OnPressGacha10_Button, Define.UIEvent.Press);

        Refresh();

        return true;
    }

    void Refresh()
    {
        if (_init == false)
            return;

        OnClickCoinTabButton();
    }

    #region EventHandler

    void OnClickCoinTabButton()
    {
        GetObject((int)GameObjects.SelectCoinTab).SetActive(true);
        GetObject((int)GameObjects.CoinProductScroll).SetActive(true);
        GetObject((int)GameObjects.SelectDiaTab).SetActive(false);
        GetObject((int)GameObjects.DiaProductScroll).SetActive(false);
        GetObject((int)GameObjects.SelectWeaponTab).SetActive(false);
        GetObject((int)GameObjects.WeaponGacha).SetActive(false);
    }

    void OnClickDiaTabButton()
    {
        GetObject((int)GameObjects.SelectCoinTab).SetActive(false);
        GetObject((int)GameObjects.CoinProductScroll).SetActive(false);
        GetObject((int)GameObjects.SelectDiaTab).SetActive(true);
        GetObject((int)GameObjects.DiaProductScroll).SetActive(true);
        GetObject((int)GameObjects.SelectWeaponTab).SetActive(false);
        GetObject((int)GameObjects.WeaponGacha).SetActive(false);
    }

    void OnClickWeaponTabButton()
    {
        GetObject((int)GameObjects.SelectCoinTab).SetActive(false);
        GetObject((int)GameObjects.CoinProductScroll).SetActive(false);
        GetObject((int)GameObjects.SelectDiaTab).SetActive(false);
        GetObject((int)GameObjects.DiaProductScroll).SetActive(false);
        GetObject((int)GameObjects.SelectWeaponTab).SetActive(true);
        GetObject((int)GameObjects.WeaponGacha).SetActive(true);
    }
    
    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Sound.Play(Sound.Effect, "Sound_HomeButton");
    }


    void OnClickGacha1_Button()
    {
        GetObject((int)GameObjects.Block).SetActive(true);
        GetObject((int)GameObjects.Gacha1_Pressed).SetActive(false);
        GetObject((int)GameObjects.Gacha1_Normal).SetActive(true);
        Managers.UI.ShowPopupUI<UI_WeaponGachaPopup>(callback: (popup) =>
        {
            popup.SetInfo(1);
            GetObject((int)GameObjects.Block).SetActive(false);
        });
    }

    void OnPressGacha1_Button()
    {
        GetObject((int)GameObjects.Gacha1_Pressed).SetActive(true);
        GetObject((int)GameObjects.Gacha1_Normal).SetActive(false);
    }

    void OnClickGacha10_Button()
    {
        GetObject((int)GameObjects.Block).SetActive(true);
        GetObject((int)GameObjects.Gacha10_Pressed).SetActive(false);
        GetObject((int)GameObjects.Gacha10_Normal).SetActive(true);
        Managers.UI.ShowPopupUI<UI_WeaponGachaPopup>(callback: (popup) =>
        {
            popup.SetInfo(10);
            GetObject((int)GameObjects.Block).SetActive(false);
        });
    }

    void OnPressGacha10_Button()
    {
        GetObject((int)GameObjects.Gacha10_Pressed).SetActive(true);
        GetObject((int)GameObjects.Gacha10_Normal).SetActive(false);
    }
    #endregion
}
