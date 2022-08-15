using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
//using DG.Tweening;

public class UI_BattleItem : UI_Base
{
	enum Images
	{ 
		PortraitIcon,
		BattleLock
	}

	enum Texts
	{
		PortraitNameText,
		BattleStartButtonText,
	}

	enum Buttons
	{
		BattleStartButton,
	}

	PlayerData _data;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindImage(typeof(Images));
		BindText(typeof(Texts));
		BindButton(typeof(Buttons));

		GetButton((int)Buttons.BattleStartButton).gameObject.BindEvent(OnClickBattleButton);		

		RefreshUI();

		return true;
	}

	public void SetInfo(PlayerData data)
	{
		_data = data;
		RefreshUI();
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;

		GetText((int)Texts.PortraitNameText).text = Managers.GetText(_data.nameID);

		Sprite sprite = Managers.Resource.Load<Sprite>(_data.illustPath);
		GetImage((int)Images.PortraitIcon).sprite = sprite;

		GetButton((int)Buttons.BattleStartButton).gameObject.SetActive(true);
		GetText((int)Texts.BattleStartButtonText).text = Managers.GetText(Define.LetsBattle);

		RefreshButton();
	}

	public void RefreshButton()
	{
		if (_init == false)
			return;

		if (Managers.Game.Hp == 0 || Managers.Game.BlockCount == 0)
		{
			GetButton((int)Buttons.BattleStartButton).gameObject.SetActive(false);
			GetImage((int)Images.BattleLock).gameObject.SetActive(true);
		}
		else
		{
			GetButton((int)Buttons.BattleStartButton).gameObject.SetActive(true);
			GetImage((int)Images.BattleLock).gameObject.SetActive(false);
		}
	}

	void OnClickBattleButton()
	{
		UI_BattleConfirmPopup popup = Managers.UI.ShowPopupUI<UI_BattleConfirmPopup>();
		Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");
		popup.SetInfo(_data);
	}
}
