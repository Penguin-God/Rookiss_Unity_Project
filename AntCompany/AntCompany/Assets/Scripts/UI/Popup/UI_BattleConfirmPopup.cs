using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_BattleConfirmPopup : UI_Popup
{
	enum Buttons
	{
		CloseButton,
		ConfirmButton
	}

	enum Images
	{
		EnemyImage
	}

	enum Texts
	{
		ReallyFightText,
		ConfirmButtonText
	}

	PlayerData _data;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindButton(typeof(Buttons));
		BindImage(typeof(Images));
		BindText(typeof(Texts));

		GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnConfirmButton);
		GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClosePopup);

		RefreshUI();

		return true;
	}

	public void SetInfo(PlayerData data)
	{
		_data = data;
		RefreshUI();
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		GetImage((int)Images.EnemyImage).sprite = Managers.Resource.Load<Sprite>(_data.battleIconPath);

		GetText((int)Texts.ReallyFightText).text = Managers.GetText(Define.BattleConfirm);
		GetText((int)Texts.ConfirmButtonText).text = Managers.GetText(Define.LetsBattleButton);
	}

	void OnConfirmButton()
	{
		Managers.UI.ClosePopupUI(this);
		Managers.UI.ShowPopupUI<UI_BattlePopup>().SetInfo(_data, 0.5f, 0.2f);
		Managers.Sound.Play(Sound.Effect, "Sound_CheckButton");
	}

	void OnClosePopup()
	{
		Managers.UI.ClosePopupUI(this);
		Managers.Sound.Play(Sound.Effect, "Sound_CancelButton");
	}
}
