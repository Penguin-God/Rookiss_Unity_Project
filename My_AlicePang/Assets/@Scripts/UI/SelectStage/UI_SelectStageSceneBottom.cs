using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SelectStageSceneBottom : UI_Base
{
    enum Buttons
    {
        InventoryButton,
        PlayButton,
        ShopButton,
    }

    UI_SelectStageScene _selectStageSceneUI;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.InventoryButton).gameObject.BindEvent(OnClickInventoryButton);
        GetButton((int)Buttons.PlayButton).gameObject.BindEvent(OnClickPlayButton);
        GetButton((int)Buttons.ShopButton).gameObject.BindEvent(OnClickShopButton);
        
        return true;
    }

    public void SetInfo(UI_SelectStageScene sceneUI)
    {
        _selectStageSceneUI = sceneUI;
    }

    #region EventHandler
    void OnClickInventoryButton()
    {
        Managers.UI.ShowPopupUI<UI_InventoryPopup>(callback: (popup) =>
        {
             _selectStageSceneUI.InventoryPopupUI = popup;
        });
    }

    void OnClickPlayButton()
    {
        _selectStageSceneUI.ShowStartStagePopup();
    }

    void OnClickShopButton()
    {
        Managers.UI.ShowPopupUI<UI_ShopPopup>();
    }
    #endregion
}
