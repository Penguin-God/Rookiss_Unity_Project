using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;

public class UI_GameScene : UI_Scene
{
	enum GameObjects
    {
		MissionPanel,
		CoinObject,
        TouchArea,
    }

	enum Buttons
	{
		ShortRangeButton,
		MiddleRangeButton,
		LongRangeButton,
		SelectedShortRangeButton,
		SelectedMiddleRangeButton,
		SelectedLongRangeButton,
        PauseButton,
	}

	enum Images
    {
		ShortRangeWeaponImage,
		MiddleRangeWeaponImage,
		LongRangeWeaponImage,
		SelectedShortRangeWeaponImage,
		SelectedMiddleRangeWeaponImage,
		SelectedLongRangeWeaponImage,
    }

	enum Texts
	{
		ComboText,
		CoinText,
        TurnText,
        ShortRangeWeaponDamageText,
        MiddleRangeWeaponDamageText,
        LongRangeWeaponDamageText,
        SelectedShortRangeWeaponDamageText,
        SelectedMiddleRangeWeaponDamageText,
        SelectedLongRangeWeaponDamageText,
    }

	GameManagerEx _game;
	public Dictionary<int, UI_MissionMonsterItem> MissionMonsterItems;

    List<RespawnData> _respawnData;

    int _showCoin = 0;
    int _remainingTurn;

    public int ShowCoin
    {
        get { return _showCoin; }

        set
        {
            _showCoin = value;
            SetCoinText(_showCoin);
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        GetButton((int)Buttons.ShortRangeButton).gameObject.BindEvent(OnClickShortRangeButton);
        GetButton((int)Buttons.MiddleRangeButton).gameObject.BindEvent(OnClickMiddleRangeButton);
        GetButton((int)Buttons.LongRangeButton).gameObject.BindEvent(OnClickLongRangeButton);

        GetButton((int)Buttons.SelectedShortRangeButton).gameObject.BindEvent(OnClickShortRangeButton);
        GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.BindEvent(OnClickMiddleRangeButton);
        GetButton((int)Buttons.SelectedLongRangeButton).gameObject.BindEvent(OnClickLongRangeButton);

        GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(false);
        GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(false);
        GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(false);

        GetButton((int)Buttons.PauseButton).gameObject.BindEvent(OnClickShowPausePopup);

        GetObject((int)GameObjects.TouchArea).BindEvent(OnClickTouchArea);

        ShowWeaponButtons(false);
        //SetMission(3, 51);
        _game = Managers.Game;

        SetCoinText(0);

        Refresh();

        return true;
    }

    public void SetInfo(List<RespawnData> respawnData, int turn)
    {
        _respawnData = respawnData;
        _remainingTurn = turn;
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        MissionMonsterItems = new Dictionary<int, UI_MissionMonsterItem>();
        GameObject parent = GetObject((int)GameObjects.MissionPanel);

        for (int i = 0; i < _respawnData.Count; i++)
        {
            if (_respawnData[i].ClearCount <= 0)
            {
                continue;
            }

            Managers.UI.MakeSubItem<UI_MissionMonsterItem>(parent.transform, "UI_MissionMonsterItem", callback: (item) =>
            {
                item.SetInfo(_respawnData[i].MonsterID, _respawnData[i].ClearCount);
                MissionMonsterItems.Add(_respawnData[i].MonsterID, item);
            });
        }
        Managers.Resource.LoadAsync<Sprite>(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID].Sprite, callback: (sprite) =>
        {
            GetImage((int)Images.ShortRangeWeaponImage).sprite = sprite;
            GetImage((int)Images.SelectedShortRangeWeaponImage).sprite = sprite;
        });

        Managers.Resource.LoadAsync<Sprite>(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID].Sprite, callback: (sprite) =>
        {
            GetImage((int)Images.MiddleRangeWeaponImage).sprite = sprite;
            GetImage((int)Images.SelectedMiddleRangeWeaponImage).sprite = sprite;
        });

        Managers.Resource.LoadAsync<Sprite>(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID].Sprite, callback: (sprite) =>
        {
            GetImage((int)Images.LongRangeWeaponImage).sprite = sprite;
            GetImage((int)Images.SelectedLongRangeWeaponImage).sprite = sprite;
        });

        int templateID = Managers.Game.ShortRangeWeaponID;
        int level = Managers.Game.WeaponLevel[templateID - 1];
        int damage = Managers.Data.Weapons[templateID].weaponLevelData[level - 1].Damage;
        GetText((int)Texts.ShortRangeWeaponDamageText).text = damage.ToString();
        GetText((int)Texts.SelectedShortRangeWeaponDamageText).text = damage.ToString();

        templateID = Managers.Game.MiddleRangeWeaponID;
        level = Managers.Game.WeaponLevel[templateID - 1];
        damage = Managers.Data.Weapons[templateID].weaponLevelData[level - 1].Damage;
        GetText((int)Texts.MiddleRangeWeaponDamageText).text = damage.ToString();
        GetText((int)Texts.SelectedMiddleRangeWeaponDamageText).text = damage.ToString();

        templateID = Managers.Game.LongRangeWeaponID;
        level = Managers.Game.WeaponLevel[templateID - 1];
        damage = Managers.Data.Weapons[templateID].weaponLevelData[level - 1].Damage;
        GetText((int)Texts.LongRangeWeaponDamageText).text = damage.ToString();
        GetText((int)Texts.SelectedLongRangeWeaponDamageText).text = damage.ToString();

        GetText((int)Texts.TurnText).text = _remainingTurn.ToString();
    }

    public void ShowWeaponButtons(bool show)
	{
		GetButton((int)Buttons.ShortRangeButton).gameObject.SetActive(show);
		GetButton((int)Buttons.MiddleRangeButton).gameObject.SetActive(show);
		GetButton((int)Buttons.LongRangeButton).gameObject.SetActive(show);
        switch (Managers.Game.WeaponType)
        {
            case WeaponRangeType.Short:
                GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(show);
                break;

            case WeaponRangeType.Middle:
                GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(show);
                break;

            case WeaponRangeType.Long:
                GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(show);
                break;
        }
    }
    bool isShakeMission = false;
	public void SetTurn(int turn)
    {
        _remainingTurn = turn;
        if(_remainingTurn < 6 && !isShakeMission)
        {
            for (int i = 0; i < _respawnData.Count; i++)
            {
                if (MissionMonsterItems.TryGetValue(_respawnData[i].MonsterID, out UI_MissionMonsterItem item))
                    item.ShakeImage();
            }
            isShakeMission = true;
        }
        GetText((int)Texts.TurnText).text = _remainingTurn.ToString();
    }

	public void MonsterDead(int monsterTemplateID, int remainMonster)
    {
		if (MissionMonsterItems.TryGetValue(monsterTemplateID, out UI_MissionMonsterItem item))
			item.SetRemainMonsterCount(remainMonster);
    }

	public Vector3 CoinPosition()
    {
        
		Vector3 screenPosition = Camera.main.ScreenToWorldPoint(GetObject((int)GameObjects.CoinObject).transform.position);
        return screenPosition;

		//return GetObject((int)GameObjects.CoinObject).transform.position;
	}

    

	public void ChangeSelectedButton(WeaponRangeType weaponRangeType)
    {
		switch(weaponRangeType)
        {
			case WeaponRangeType.Short:
				GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(true);
                GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(false);
                GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(false);
                break;
		
			case WeaponRangeType.Middle:
                GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(false);
                GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(true);
                GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(false);
                break;
		
			case WeaponRangeType.Long:
                GetButton((int)Buttons.SelectedShortRangeButton).gameObject.SetActive(false);
                GetButton((int)Buttons.SelectedMiddleRangeButton).gameObject.SetActive(false);
                GetButton((int)Buttons.SelectedLongRangeButton).gameObject.SetActive(true);
                break;
        }
    }

	public void Combo(int comboCount)
	{
		if (comboCount < 2)
			GetText((int)Texts.ComboText).text = string.Empty;
		else if (comboCount >= 100)
		{
			int digit100 = comboCount / 100;
			int digit10 = (comboCount - digit100) / 10;
			int digit1 = comboCount % 10;
            GetText((int)Texts.ComboText).text = $"<sprite=11><sprite=12><sprite=13><sprite=14><sprite=12><sprite=10><sprite={digit100}><sprite={digit10}><sprite={digit1}>";
        }
        else if (comboCount >= 10)
		{
			int digit10 = comboCount / 10;
			int digit1 = comboCount % 10;
			GetText((int)Texts.ComboText).text = $"<sprite=11><sprite=12><sprite=13><sprite=14><sprite=12><sprite=10><sprite={digit10}><sprite={digit1}>";
		}
		else
		{
            GetText((int)Texts.ComboText).text = $"<sprite=11><sprite=12><sprite=13><sprite=14><sprite=12><sprite=10><sprite={comboCount}>";
        }
    }

	public void SetCoinText(int coin)
    {
        GetText((int)Texts.CoinText).text = coin.ToString();
    }

	public void DisableWeaponButton(WeaponRangeType disableWeaponRangeType)
    {
        switch (disableWeaponRangeType)
        {
            case WeaponRangeType.None:
                Debug.Log("None");
                GetButton((int)Buttons.ShortRangeButton).interactable = true;
                GetButton((int)Buttons.MiddleRangeButton).interactable = true;
                GetButton((int)Buttons.LongRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedShortRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedMiddleRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedLongRangeButton).interactable = true;
                break;

            case WeaponRangeType.Short:
                Debug.Log("Short");

                GetButton((int)Buttons.ShortRangeButton).interactable = false;
                GetButton((int)Buttons.MiddleRangeButton).interactable = true;
                GetButton((int)Buttons.LongRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedShortRangeButton).interactable = false;
                GetButton((int)Buttons.SelectedMiddleRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedLongRangeButton).interactable = true;
                break;

            case WeaponRangeType.Middle:
                Debug.Log("Middle");

                GetButton((int)Buttons.ShortRangeButton).interactable = true;
                GetButton((int)Buttons.MiddleRangeButton).interactable = false;
                GetButton((int)Buttons.LongRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedShortRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedMiddleRangeButton).interactable = false;
                GetButton((int)Buttons.SelectedLongRangeButton).interactable = true;
                break;

            case WeaponRangeType.Long:
                Debug.Log("Long");

                GetButton((int)Buttons.ShortRangeButton).interactable = true;
                GetButton((int)Buttons.MiddleRangeButton).interactable = true;
                GetButton((int)Buttons.LongRangeButton).interactable = false;
                GetButton((int)Buttons.SelectedShortRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedMiddleRangeButton).interactable = true;
                GetButton((int)Buttons.SelectedLongRangeButton).interactable = false;
                break;
        }
    }
    #region EventHandler
    void OnClickShortRangeButton()
    {
        if (_game.DisableWeaponRangeType == WeaponRangeType.Short)
            return;

        Debug.Log("OnClickShortRangeButton");

        // 두번째 클릭이라면.
        if (_game.WeaponType == WeaponRangeType.Short)
        {
            Managers.Sound.Stop(Sound.SubBgm);
            Managers.Sound.Play(Sound.Effect, "Sound_ButtonSelected");
            ShowWeaponButtons(false);
            _game.PlayerAttack();
        }
        else
        {
            _game.WeaponType = WeaponRangeType.Short;
            Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");
        }
    }

    void OnClickMiddleRangeButton()
    {
        if (_game.DisableWeaponRangeType == WeaponRangeType.Middle)
            return;

        Debug.Log("OnClickMiddleRangeButton");

        // 두번째 클릭이라면.
        if (_game.WeaponType == WeaponRangeType.Middle)
        {
            Managers.Sound.Stop(Sound.SubBgm);
            Managers.Sound.Play(Sound.Effect, "Sound_ButtonSelected");
            ShowWeaponButtons(false);
            _game.PlayerAttack();
        }
        else
        {
            _game.WeaponType = WeaponRangeType.Middle;
            Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");
        }
    }

    void OnClickLongRangeButton()
    {
        if (_game.DisableWeaponRangeType == WeaponRangeType.Long)
            return;

        Debug.Log("OnClickLongRangeButton");

        // 두번째 클릭이라면.
        if (_game.WeaponType == WeaponRangeType.Long)
        {
            Managers.Sound.Stop(Sound.SubBgm);
            Managers.Sound.Play(Sound.Effect, "Sound_ButtonSelected");
            ShowWeaponButtons(false);
            _game.PlayerAttack();
        }
        else
        {
            _game.WeaponType = WeaponRangeType.Long;
            Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");
        }
    }

    void OnClickShowPausePopup()
    {
        Managers.UI.ShowPopupUI<UI_GamePausePopup>();
    }

    void OnClickTouchArea()
    {
        if (_game.WeaponType == WeaponRangeType.None)
            return;

        ShowWeaponButtons(false);
        _game.PlayerAttack();
    }

    #endregion
}