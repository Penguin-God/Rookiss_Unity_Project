using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WeaponItem : UI_Base
{
    enum Texts
    {
        DamageText,
    }
    enum Images
    {
        WeaponImage,
    }

    enum Buttons
    {
        WeaponInfoButton,
    }

    WeaponData _weaponData;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.WeaponInfoButton).gameObject.BindEvent(OnClickWeaponInfoButton);

        Refresh();

        return true;
    }

    public void SetInfo(WeaponData weaponData)
    {
        _weaponData = weaponData;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        Managers.Resource.LoadAsync<Sprite>(_weaponData.Sprite, callback: (sprite) =>
        {
            GetImage((int)Images.WeaponImage).sprite = sprite;
        });

        int templateID = _weaponData.TemplateID;
        int level = Managers.Game.WeaponLevel[templateID - 1];
        int damage = Managers.Data.Weapons[templateID].weaponLevelData[level - 1].Damage;
        GetText((int)Texts.DamageText).text = damage.ToString();
    }

    #region EventHandler
    void OnClickWeaponInfoButton()
    {
        Managers.UI.ShowPopupUI<UI_InventoryPopup>(callback: (popup) =>
        {
            (Managers.UI.SceneUI as UI_SelectStageScene).InventoryPopupUI = popup;
        });
    }
    #endregion
}