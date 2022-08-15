using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInfoPopup : UI_Popup
{
	enum Texts
	{
		NameText,
		LevelText,
		SalaryText,
		SalaryValueText,
		MaxHpText,
		MaxHpValueText,
		AttackText,
		AttackValueText,
		ProjectCooltimeText,
		ProjectCooltimeValueText,
		SalaryIncreaseText,
		SalaryIncreaseValueText,
		MoneyIncreaseText,
		MoneyIncreaseValueText,
		BlockSuccessText,
		BlockSuccessValueText,
		StatText
	}

	enum Images
	{
		Background,
		PlayerImage,
		PopupImage
	}

	GameManagerEx _game;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindImage(typeof(Images));
		
		GetImage((int)Images.PopupImage).gameObject.BindEvent(OnClosePopup);

		_game = Managers.Game;

		RefreshUI();
		return true;
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;

		GetText((int)Texts.NameText).text = _game.Name;
		GetText((int)Texts.LevelText).text = Utils.GetJobTitleString(_game.JobTitle);
		GetText((int)Texts.SalaryText).text = Managers.GetText(Define.SalaryText);
		GetText((int)Texts.SalaryValueText).text = $"{Utils.GetMoneyString(_game.Salary * 12)}";
		GetText((int)Texts.MaxHpText).text = Managers.GetText(Define.MaxHpText);
		GetText((int)Texts.MaxHpValueText).text = $"{_game.MaxHp}";
		GetText((int)Texts.AttackText).text = Managers.GetText(Define.AttackText);
		GetText((int)Texts.AttackValueText).text = $"{_game.Attack}";
		GetText((int)Texts.ProjectCooltimeText).text = Managers.GetText(Define.ProjectCooltimeText);
		GetText((int)Texts.ProjectCooltimeValueText).text = $"{_game.ProjectCoolTimePercent}%";
		GetText((int)Texts.SalaryIncreaseText).text = Managers.GetText(Define.SalaryIncreaseText);
		GetText((int)Texts.SalaryIncreaseValueText).text = $"{_game.SalaryAdditionalIncreasePercent}%";
		GetText((int)Texts.MoneyIncreaseText).text = Managers.GetText(Define.MoneyIncreaseText);
		GetText((int)Texts.MoneyIncreaseValueText).text = $"{_game.AdditionalRevenuePercent}%";
		GetText((int)Texts.BlockSuccessText).text = Managers.GetText(Define.BlockSuccessText);
		GetText((int)Texts.BlockSuccessValueText).text = $"{_game.BlockHitSucessPercent}%";
	}

	void OnClosePopup()
	{
		Debug.Log("OnClosePopup");
		Managers.UI.ClosePopupUI(this);
	}
}
