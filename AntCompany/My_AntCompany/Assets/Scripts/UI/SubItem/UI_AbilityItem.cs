using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_AbilityItem : UI_Base
{
	enum Texts
	{
		TitleText,
		ChangeText,
		DiffText,
		UpgradeText,
		MoneyText
	}

	enum Buttons
	{
		UpgradeButton
	}

	StatType _statType;
	StatData _statData;

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindButton(typeof(Buttons));

		GetText((int)Texts.UpgradeText).text = Managers.GetText(Define.Upgrade);

		GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(() => new AbilityUseCase().TryStatUpgrade(_statType), UIEvent.Pressed);
		GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(OnPointerUp, UIEvent.PointerUp);

		RefreshUI();

		return true;
	}

	public void SetInfo(StatType statType, float moveDelay, float rotateDelay)
	{
		_statType = statType;
		//_moveAnim.delay = moveDelay;
		//_rotateAnim.delay = rotateDelay;

		int id = GetStatUpgradeId(_statType);
		if (Managers.Data.Stats.TryGetValue(id, out _statData) == false)
		{
			Debug.Log($"UI_AbilityItem SetInfo Failed : {statType}");
			return;
		}

		RefreshUI();
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;

		int value = Utils.GetStatValue(_statType);

		GetText((int)Texts.TitleText).text = Managers.GetText(_statData.nameID);
		GetText((int)Texts.ChangeText).text = $"{value} → {GetIncreasedValue()}";
		GetText((int)Texts.MoneyText).text = Utils.GetMoneyString(_statData.price);

		if (_statType == StatType.Luck)
			GetText((int)Texts.ChangeText).text = $"{Managers.Game.Luck}";

		if (CanUpgrade())
			GetButton((int)Buttons.UpgradeButton).interactable = true;
		else
			GetButton((int)Buttons.UpgradeButton).interactable = false;

		if (_statType == StatType.Luck)
			GetButton((int)Buttons.UpgradeButton).gameObject.SetActive(false);
		
		GetText((int)Texts.DiffText).gameObject.SetActive(false);
	}

	int GetIncreasedValue()
	{
		int value = Utils.GetStatValue(_statType);

		if (_statType == StatType.Stress)
			return Math.Max(0, value + _statData.increaseStat);

		return value + _statData.increaseStat;
	}

	int GetStatUpgradeId(StatType type)
	{
		switch (type)
		{
			case StatType.MaxHp:
				return 1;
			case StatType.WorkAbility:
				return 2;
			case StatType.Likeability:
				return 3;
			case StatType.Stress:
				return 4;
			case StatType.Luck:
				return 5;
		}

		return 0;
	}

	Coroutine _coolTime;

	void OnPressUpgradeButton()
	{	
		if (_coolTime == null)
		{
			Debug.Log("OnPressUpgradeButton");

			if (CanUpgrade())
			{
				Managers.Game.Money -= _statData.price;
				int value = _statData.increaseStat;

				switch (_statType)
				{
					case StatType.MaxHp:
						Managers.Game.MaxHp += value; 
						Managers.Game.Hp = Math.Min(Managers.Game.Hp + value, Managers.Game.MaxHp);
						break;
					case StatType.WorkAbility:
						Managers.Game.WorkAbility += value;
						break;
					case StatType.Likeability:
						Managers.Game.Likeability += value;
						break;
					case StatType.Luck:
						Managers.Game.Luck += value;
						break;
					case StatType.Stress:
						Managers.Game.Stress += value;
						break;
				} 

				if(_statType != StatType.Luck)
					Managers.Sound.Play(Sound.Effect, "Sound_UpgradeDone");

				RefreshUI();

				Managers.UI.FindPopup<UI_PlayPopup>()?.RefreshHpBar();
				Managers.UI.FindPopup<UI_PlayPopup>()?.RefreshStat();
				Managers.UI.FindPopup<UI_PlayPopup>()?.RefreshMoney();
			}

            _coolTime = StartCoroutine(CoStartUpgradeCoolTime(0.1f));
		}
	}

	bool CanUpgrade()
	{
		switch (_statType)
		{
			case StatType.Luck:
				return false;
			case StatType.Stress:
				return Managers.Game.Stress > 0 && Managers.Game.Money >= _statData.price;
		}

		return Managers.Game.Money >= _statData.price;
	}

	void OnPointerUp()
	{
		if (_coolTime != null)
		{
			StopCoroutine(_coolTime);
			_coolTime = null;
		}
	}

	IEnumerator CoStartUpgradeCoolTime(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_coolTime = null;
	}
}

class AbilityUseCase
{
	public void TryStatUpgrade(StatType statType)
	{
		Debug.Log("Try Upgrade Stat");
		if (Managers.Data.Stats.TryGetValue(GetStatUpgradeId(statType), out StatData statData) == false)
			return;

		if (CanUpgrade(statType, statData))
		{
			Managers.Game.Money -= statData.price;
			int value = statData.increaseStat;

			switch (statType)
			{
				case StatType.MaxHp:
					Managers.Game.MaxHp += value;
					Managers.Game.Hp = Math.Min(Managers.Game.Hp + value, Managers.Game.MaxHp);
					break;
				case StatType.WorkAbility:
					Managers.Game.WorkAbility += value;
					break;
				case StatType.Likeability:
					Managers.Game.Likeability += value;
					break;
				case StatType.Luck:
					Managers.Game.Luck += value;
					break;
				case StatType.Stress:
					Managers.Game.Stress += value;
					break;
			}

			if (statType != StatType.Luck)
				Managers.Sound.Play(Sound.Effect, "Sound_UpgradeDone");

			Managers.UI.FindPopup<UI_PlayPopup>()?.RefreshAbilityItems();
			Managers.UI.FindPopup<UI_PlayPopup>()?.RefreshHpBar();
			Managers.UI.FindPopup<UI_PlayPopup>()?.RefreshStat();
			Managers.UI.FindPopup<UI_PlayPopup>()?.RefreshMoney();
		}
	}

	bool CanUpgrade(StatType statType, StatData statData)
	{
		switch (statType)
		{
			case StatType.Luck:
				return false;
			case StatType.Stress:
				return Managers.Game.Stress > 0 && Managers.Game.Money >= statData.price;
		}

		return Managers.Game.Money >= statData.price;
	}

	int GetStatUpgradeId(StatType type)
	{
		switch (type)
		{
			case StatType.MaxHp: return 1;
			case StatType.WorkAbility: return 2;
			case StatType.Likeability: return 3;
			case StatType.Stress: return 4;
			case StatType.Luck: return 5;
		}

		return 0;
	}
}
