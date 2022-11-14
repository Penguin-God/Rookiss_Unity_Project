using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_InventoryPopup : UI_Popup
{
    enum GameObjects
    {
        BG,
        ShortRangeWeaponPanel,
        MiddleRangeWeaponPanel,
        LongRangeWeaponPanel,
        ShortRangeWeaponSlot,
        MiddleRangeWeaponSlot,
        LongRangeWeaponSlot,
    }

    enum Buttons
    {
        CloseButton,
    }

    Dictionary<int, UI_WeaponInventoryItem> _weaponInventoryItemUI = new Dictionary<int, UI_WeaponInventoryItem>();

    UI_WeaponInventoryItem _shortRangeWeaponItem;
    UI_WeaponInventoryItem _middleRangeWeaponItem;
    UI_WeaponInventoryItem _longRangeWeaponItem;

    int _shortRangeWeaponTemplateID;
    int _middleRangeWeaponTemplateID;
    int _longRangeWeaponTemplateID;

    public override bool Init()
    {
        Managers.Sound.Play(Sound.Effect, "Sound_Inventory");

        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetObject((int)GameObjects.BG).SetActive(false);

        for (int i = 0; i < WEAPON_COUNT; i++)
        {
            WeaponData weaponData = Managers.Data.Weapons[i+1];
            Transform parent;
            switch (weaponData.RangeType)
            {
                case WeaponRangeType.Short:
                    parent = GetObject((int)GameObjects.ShortRangeWeaponPanel).transform;
                    break;
                case WeaponRangeType.Middle:
                    parent = GetObject((int)GameObjects.MiddleRangeWeaponPanel).transform;
                    break;
                default:
                    parent = GetObject((int)GameObjects.LongRangeWeaponPanel).transform;
                    break;
            }

            Managers.UI.MakeSubItem<UI_WeaponInventoryItem>(parent, "UI_WeaponInventoryItem", callback: (item) =>
            {
                item.SetInfo(weaponData);
                _weaponInventoryItemUI.Add(weaponData.TemplateID, item);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parent);
            });
        }

        Managers.UI.MakeSubItem<UI_WeaponInventoryItem>(GetObject((int)GameObjects.ShortRangeWeaponSlot).transform, "UI_WeaponInventoryItem", callback: (item) =>
        {
            _shortRangeWeaponItem = item;
            _shortRangeWeaponItem.SetInfo(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID]);
            _weaponInventoryItemUI[Managers.Game.ShortRangeWeaponID].gameObject.SetActive(true);
        });

        Managers.UI.MakeSubItem<UI_WeaponInventoryItem>(GetObject((int)GameObjects.MiddleRangeWeaponSlot).transform, "UI_WeaponInventoryItem", callback: (item) =>
        {
            _middleRangeWeaponItem = item;
            _middleRangeWeaponItem.SetInfo(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID]);
            _weaponInventoryItemUI[Managers.Game.MiddleRangeWeaponID].gameObject.SetActive(true);
        });

        Managers.UI.MakeSubItem<UI_WeaponInventoryItem>(GetObject((int)GameObjects.LongRangeWeaponSlot).transform, "UI_WeaponInventoryItem", callback: (item) =>
        {
            _longRangeWeaponItem = item;
            _longRangeWeaponItem.SetInfo(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID]);
            _weaponInventoryItemUI[Managers.Game.LongRangeWeaponID].gameObject.SetActive(true);
        });

        StartCoroutine(CoWaitLoad());
        
        //Refresh();
        return true;
    }

    public void SetInfo()
    {
        _shortRangeWeaponTemplateID = Managers.Game.ShortRangeWeaponID;
        _middleRangeWeaponTemplateID = Managers.Game.MiddleRangeWeaponID;
        _longRangeWeaponTemplateID = Managers.Game.LongRangeWeaponID;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_shortRangeWeaponItem.WeaponData != null)
        {
            int currentShortRangeWeaponTemplateID = _shortRangeWeaponItem.WeaponData.TemplateID;
            _weaponInventoryItemUI[currentShortRangeWeaponTemplateID].gameObject.SetActive(true);
        }
        _shortRangeWeaponItem.SetInfo(Managers.Data.Weapons[_shortRangeWeaponTemplateID]);
        _weaponInventoryItemUI[_shortRangeWeaponTemplateID].gameObject.SetActive(true);

        if (_middleRangeWeaponItem.WeaponData != null)
        {
            int currentMiddleRangeWeaponTemplateID = _middleRangeWeaponItem.WeaponData.TemplateID;
            _weaponInventoryItemUI[currentMiddleRangeWeaponTemplateID].gameObject.SetActive(true);
        }
        _middleRangeWeaponItem.SetInfo(Managers.Data.Weapons[_middleRangeWeaponTemplateID]);
        _weaponInventoryItemUI[_middleRangeWeaponTemplateID].gameObject.SetActive(true);

        if (_longRangeWeaponItem.WeaponData != null)
        {
            int currentLongRangeWeaponTemplateID = _longRangeWeaponItem.WeaponData.TemplateID;
            _weaponInventoryItemUI[currentLongRangeWeaponTemplateID].gameObject.SetActive(true);
        }
        _longRangeWeaponItem.SetInfo(Managers.Data.Weapons[_longRangeWeaponTemplateID]);
        _weaponInventoryItemUI[_longRangeWeaponTemplateID].gameObject.SetActive(true);
    }

    IEnumerator CoWaitLoad()
    {
        while (Managers.Resource.HandlesCount > 0)
            yield return null;

        GetObject((int)GameObjects.BG).SetActive(true);
    }

    #region EventHandler
    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Sound.Play(Sound.Effect, "Sound_HomeButton");
    }
    #endregion
}
