using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestPopup : UI_Popup
{
    enum GameObjects
    {
        QuestItemList,
        Block,
    }

    enum Buttons
    {
        CloseButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetObject((int)GameObjects.Block).BindEvent(OnClickCloseButton);

        Refresh();
        return true;
    }

    void Refresh()
    {
        if (_init == false)
            return;

        for(int i=0; i< Managers.Game.DailyQuestID.Length; i++)
        {
            Managers.UI.MakeSubItem<UI_QuestItem>(GetObject((int)GameObjects.QuestItemList).transform, callback: (item) =>
            {
                
            });
        }
    }

    #region EventHandler
    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
    #endregion
}
