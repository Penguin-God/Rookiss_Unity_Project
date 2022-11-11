using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WeaponInventoryItem : UI_Base
{
    enum GameObjects
    {
        XPFill,
        XPFill_Full,
        XPIcon,
        XPIcon_Full,
        ExpFullIcon,
        Block,
    }

    enum Buttons
    {
        WeaponInfoButton,
    }

    enum Images
    {
        WeaponImage,
    }

    enum Texts
    {
        LevelText,
        DamageText,
        ExpText,
    }

    enum Sliders
    {
        ExpSlider,
    }

    WeaponData _weaponData;
    public WeaponData WeaponData { get { return _weaponData; } }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindSlider(typeof(Sliders));

        GetButton((int)Buttons.WeaponInfoButton).gameObject.BindEvent(ShowPopup);

        Refresh();

        return true;
    }

    public void SetInfo(WeaponData weaponData)
    {
        _weaponData = weaponData;        
        Refresh();
    }

    public void Refresh()
    {
        if (_init == false)
            return;
        if (_weaponData == null)
            return;

        int weaponIndex = _weaponData.TemplateID - 1;
        if (weaponIndex < 0 || weaponIndex >= Managers.Game.WeaponLevel.Length)
		{
            Debug.Log($"Invalid Index {weaponIndex}");
            return;
		}

        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];
        
        GetText((int)Texts.LevelText).text = $"LV{weaponLevel}";
        if (weaponLevel == 0)
        {
            //GetImage((int)Images.WeaponImage).color = Color.gray;
            GetText((int)Texts.ExpText).gameObject.SetActive(false);
            GetSlider((int)Sliders.ExpSlider).gameObject.SetActive(false);
            GetObject((int)GameObjects.Block).SetActive(true);
            ExpFullResource(false);
        }
        else if (weaponLevel < _weaponData.weaponLevelData.Count)
        {
            //GetImage((int)Images.WeaponImage).color = Color.white;
            GetText((int)Texts.ExpText).text = $"{Managers.Game.WeaponExp[weaponIndex]} / {_weaponData.weaponLevelData[weaponLevel - 1].Exp}";
            GetSlider((int)Sliders.ExpSlider).value = Managers.Game.WeaponExp[weaponIndex] / (float)_weaponData.weaponLevelData[weaponLevel-1].Exp;
            if (Managers.Game.WeaponExp[weaponIndex] >= _weaponData.weaponLevelData[weaponLevel - 1].Exp)
                ExpFullResource(true);
            else
                ExpFullResource(false);

            GetText((int)Texts.DamageText).text = _weaponData.weaponLevelData[weaponLevel - 1].Damage.ToString();
            GetText((int)Texts.ExpText).gameObject.SetActive(true);
            GetSlider((int)Sliders.ExpSlider).gameObject.SetActive(true);
            GetObject((int)GameObjects.Block).SetActive(false);
        }
        else
        {
            //GetImage((int)Images.WeaponImage).color = Color.white;
            GetText((int)Texts.ExpText).text = $"MAX";
            GetSlider((int)Sliders.ExpSlider).value = 1f;
            GetText((int)Texts.DamageText).text = _weaponData.weaponLevelData[weaponLevel - 1].Damage.ToString();
            GetText((int)Texts.ExpText).gameObject.SetActive(true);
            GetSlider((int)Sliders.ExpSlider).gameObject.SetActive(true);
            GetObject((int)GameObjects.Block).SetActive(false);
        }

        Managers.Resource.LoadAsync<Sprite>(_weaponData.Sprite, callback: (sprite) =>
        {
            GetImage((int)Images.WeaponImage).sprite = sprite;
        });
    }

    void ExpFullResource(bool isFull)
    {
        GetObject((int)GameObjects.ExpFullIcon).SetActive(isFull);
        GetObject((int)GameObjects.XPFill).SetActive(!isFull);
        GetObject((int)GameObjects.XPFill_Full).SetActive(isFull);
        GetObject((int)GameObjects.XPIcon).SetActive(!isFull);
        GetObject((int)GameObjects.XPIcon_Full).SetActive(isFull);
    }

    void ShowPopup()
    {
        Managers.UI.ShowPopupUI<UI_WeaponInfoPopup>(callback: (popup) =>
        {
            popup.SetInfo(_weaponData, this);
        });
    }
}
