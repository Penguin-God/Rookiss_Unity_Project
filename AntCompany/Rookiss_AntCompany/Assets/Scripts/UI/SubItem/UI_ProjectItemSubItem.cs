using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_ProjectItemSubItem : UI_Base
{
	enum Texts
	{
		AbilityText,
		AbilityValueText,		
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		RefreshUI();

		return true;
	}

	int _value;
	RewardType _type;

	public void SetInfo(RewardType type, int value)
	{
		_type = type;
		_value = value;
		RefreshUI();
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;

		GetText((int)Texts.AbilityText).text = Utils.GetRewardString(_type);

		if (_type == RewardType.Money)
			GetText((int)Texts.AbilityValueText).text = Utils.GetMoneyString(_value);
		else
			GetText((int)Texts.AbilityValueText).text = Utils.GetRewardValueString(_value);

		GetText((int)Texts.AbilityValueText).color = Utils.GetRewardColor(_type, _value);
	}
}
