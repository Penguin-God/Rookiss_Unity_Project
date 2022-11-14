using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using DG.Tweening;

public class UI_WeaponInfoPopup : UI_Popup
{
    enum GameObjects
    {
        Block,
        XPFill_Before,
        XPFill_After,
        XPFill_Full_Before,
        XPFill_Full_After,
        XPIcon_Before,
        XPIcon_After,
        AfterWeaponInfo,
        BeforeWeaponInfo,
        DisableAfterSlot,
        DisableUpgradeButton,
        DisableEquipButton,
        Popup,
    }

    enum Images
    {
        WeaponImage_Before,
        WeaponImage_After,
    }

    enum Texts
    {
        LevelText_Before,
        LevelText_After,
        CostText,
        ExpText_Before,
        ExpText_After,
        DamageText_Before,
        DamageText_After,
        WeaponNameText,
        EquipText,
    }

    enum Buttons
    { 
        WeaponEquipButton,
        WeaponUpgradeButton,
        CloseButton, 
        
    }

    enum Sliders
    {
        ExpSlider_Before,
        ExpSlider_After,
    }

    WeaponData _weaponData;
    UI_WeaponInventoryItem _weaponInventoryItemUI;
    int _index;

    bool isActive = false;

    public override bool Init()
    {
        Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindSlider(typeof(Sliders));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)Buttons.WeaponEquipButton).gameObject.BindEvent(OnClickEquipButton);
        GetButton((int)Buttons.WeaponUpgradeButton).gameObject.BindEvent(OnClickUpgradeButton);

        GetObject((int)GameObjects.Block).gameObject.BindEvent(OnClickCloseButton);

        Refresh();

        return true;
    }

    public void SetInfo(WeaponData weaponData, UI_WeaponInventoryItem weaponInventoryItemUI)
    {
        _weaponData = weaponData;
        _weaponInventoryItemUI = weaponInventoryItemUI;
        //_index = index;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        int weaponIndex = _weaponData.TemplateID - 1;
        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];

        GetText((int)Texts.LevelText_Before).text = $"LV{Managers.Game.WeaponLevel[weaponIndex]}";
        GetText((int)Texts.WeaponNameText).text = _weaponData.NameID;
        Managers.Resource.LoadAsync<Sprite>(_weaponData.Sprite, callback: (sprite) =>
         {
             GetImage((int)Images.WeaponImage_Before).sprite = sprite;
             GetImage((int)Images.WeaponImage_After).sprite = sprite;
         });

        if (weaponLevel == 0)
        {
            GetText((int)Texts.ExpText_Before).gameObject.SetActive(false);
            GetText((int)Texts.ExpText_After).gameObject.SetActive(false);
            GetSlider((int)Sliders.ExpSlider_Before).gameObject.SetActive(false);
            GetSlider((int)Sliders.ExpSlider_After).gameObject.SetActive(false);
            GetText((int)Texts.DamageText_Before).gameObject.SetActive(false);
            GetText((int)Texts.DamageText_After).gameObject.SetActive(false);
            GetText((int)Texts.CostText).gameObject.SetActive(false);
        }
        else if (weaponLevel < _weaponData.weaponLevelData.Count)
        {
            GetText((int)Texts.LevelText_After).text = $"LV{Managers.Game.WeaponLevel[weaponIndex] + 1}";

            GetText((int)Texts.ExpText_Before).text = $"{Managers.Game.WeaponExp[weaponIndex]} / {_weaponData.weaponLevelData[weaponLevel - 1].Exp}";
            GetSlider((int)Sliders.ExpSlider_Before).value = Managers.Game.WeaponExp[weaponIndex] / (float)_weaponData.weaponLevelData[weaponLevel - 1].Exp;
            if (weaponLevel < _weaponData.weaponLevelData.Count - 1)
            {
                GetText((int)Texts.ExpText_After).text = $"{Managers.Game.WeaponExp[weaponIndex] - _weaponData.weaponLevelData[weaponLevel - 1].Exp} / {_weaponData.weaponLevelData[weaponLevel].Exp}";
                GetSlider((int)Sliders.ExpSlider_After).value = (Managers.Game.WeaponExp[weaponIndex] - _weaponData.weaponLevelData[weaponLevel - 1].Exp) / (float)_weaponData.weaponLevelData[weaponLevel].Exp;
                ExpFullResource(Managers.Game.WeaponExp[weaponIndex] >= _weaponData.weaponLevelData[weaponLevel - 1].Exp, (Managers.Game.WeaponExp[weaponIndex] - _weaponData.weaponLevelData[weaponLevel - 1].Exp) >= _weaponData.weaponLevelData[weaponLevel].Exp);
            }
            else
            {
                GetObject((int)GameObjects.AfterWeaponInfo).SetActive(false);
                ExpFullResource(Managers.Game.WeaponExp[weaponIndex] >= _weaponData.weaponLevelData[weaponLevel - 1].Exp);
            }

            int currentLevelDamage = _weaponData.weaponLevelData[weaponLevel - 1].Damage;
            int nextLevelDamage = _weaponData.weaponLevelData[weaponLevel].Damage;
            GetText((int)Texts.DamageText_Before).text = $"{currentLevelDamage}";
            GetText((int)Texts.DamageText_After).text = $"{nextLevelDamage}";

            GetText((int)Texts.CostText).text = $"{_weaponData.weaponLevelData[weaponLevel - 1].Cost}";
            GetButton((int)Buttons.WeaponUpgradeButton).interactable = CanUpgrade();
        }
        else
        {
            GetObject((int)GameObjects.AfterWeaponInfo).SetActive(false);
            GetText((int)Texts.LevelText_After).text = $"MAX";
            GetText((int)Texts.ExpText_Before).text = $"MAX";
            GetSlider((int)Sliders.ExpSlider_Before).value = 1f;
            GetText((int)Texts.DamageText_Before).text = $"{_weaponData.weaponLevelData[weaponLevel - 1].Damage}";
            GetText((int)Texts.CostText).text = $"MAX";
            GetButton((int)Buttons.WeaponUpgradeButton).interactable = false;
        }

        if (_weaponData.TemplateID == Managers.Game.ShortRangeWeaponID || _weaponData.TemplateID == Managers.Game.MiddleRangeWeaponID || _weaponData.TemplateID == Managers.Game.LongRangeWeaponID)
        {
            GetText((int)Texts.EquipText).text = "ÀåÂøÁß";
            GetObject((int)GameObjects.DisableEquipButton).SetActive(true);
        }
        else
        {
            GetText((int)Texts.EquipText).text = "ÀåÂø";
            GetObject((int)GameObjects.DisableEquipButton).SetActive(false);
        }
        if (isActive == false)
        {
            GetObject((int)GameObjects.Popup).transform.DOScale(1f, 0.2f).From(0f);
            isActive = true;
        }
        else
        {
            GetText((int)Texts.DamageText_Before).transform.DOScale(1f, 0.2f).From(0f);
        }
    }

    void ExpFullResource(bool isFull_Before, bool isFull_After = false)
    {
        GetObject((int)GameObjects.XPFill_Before).SetActive(!isFull_Before);
        GetObject((int)GameObjects.XPFill_Full_Before).SetActive(isFull_Before);
        GetObject((int)GameObjects.XPIcon_Before).SetActive(!isFull_Before);
        if (isFull_Before)
        {
            GetObject((int)GameObjects.XPFill_After).SetActive(!isFull_After);
            GetObject((int)GameObjects.XPFill_Full_After).SetActive(isFull_After);
            GetObject((int)GameObjects.XPIcon_After).SetActive(!isFull_After);
            GetObject((int)GameObjects.DisableAfterSlot).SetActive(false);
        }
        else
        {
            GetSlider((int)Sliders.ExpSlider_After).gameObject.SetActive(false);
            GetObject((int)GameObjects.DisableAfterSlot).SetActive(true);
        }
    }

    bool CanUpgrade()
    {
        int weaponIndex = _weaponData.TemplateID - 1;
        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];

        int needExp = _weaponData.weaponLevelData[weaponLevel - 1].Exp;
        int haveExp = Managers.Game.WeaponExp[weaponIndex];

        if (needExp > haveExp)
        {
            GetObject((int)GameObjects.DisableUpgradeButton).SetActive(true);
            return false;
        }
        if (Managers.Game.CheckCoin(_weaponData.weaponLevelData[weaponLevel - 1].Cost) == false)
        {
            GetObject((int)GameObjects.DisableUpgradeButton).SetActive(true);
            return false;
        }

        GetObject((int)GameObjects.DisableUpgradeButton).SetActive(false);
        return true;
    }

    #region EventHandler
    void OnClickEquipButton()
    {
        if (_weaponData.TemplateID == Managers.Game.ShortRangeWeaponID || _weaponData.TemplateID == Managers.Game.MiddleRangeWeaponID || _weaponData.TemplateID == Managers.Game.LongRangeWeaponID)
            return;

        int weaponIndex = _weaponData.TemplateID - 1;
        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];
        int weaponTemplateId = _weaponData.TemplateID;

        if (weaponLevel < 1)
            return;

        switch (_weaponData.RangeType)
        {
            case WeaponRangeType.Short:
                Managers.Game.ShortRangeWeaponID = weaponTemplateId;
                break;

            case WeaponRangeType.Middle:
                Managers.Game.MiddleRangeWeaponID = weaponTemplateId;
                break;

            default:
                Managers.Game.LongRangeWeaponID = weaponTemplateId;
                break;
        }
        (Managers.UI.SceneUI as UI_SelectStageScene).InventoryPopupUI.SetInfo();
        Managers.UI.ClosePopupUI(this);
        Managers.Sound.Play(Sound.Effect, "Sound_EquipButton");
        Managers.Game.SaveGame();
    }

    void OnClickUpgradeButton()
    {
       
        int weaponIndex = _weaponData.TemplateID - 1;
        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];

        if (CanUpgrade() == false)
            return;

        //Coin????
        Managers.Game.SpendCoin(_weaponData.weaponLevelData[weaponLevel - 1].Cost);

        int needExp = _weaponData.weaponLevelData[weaponLevel - 1].Exp;

        Managers.Game.WeaponExp[weaponIndex] -= needExp;
        Managers.Game.WeaponLevel[weaponIndex]++;

        Managers.Game.SaveGame();
        Managers.Sound.Play(Sound.Effect, "Sound_UpgradeWeapon");
        
        Refresh();
        _weaponInventoryItemUI.Refresh();
    }

    void OnClickCloseButton()
    {
        Managers.Sound.Play(Sound.Effect, "Sound_HomeButton");
        Managers.UI.ClosePopupUI(this);
        Managers.Game.SaveGame();
    }

    #endregion
}
