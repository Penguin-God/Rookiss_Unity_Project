using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_ResultItem : UI_Base
{
	enum Images
	{
		RewardIcon
	}

	enum Texts
	{
		RewardText,
		RewardValueText,
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindImage(typeof(Images));
		BindText(typeof(Texts));

		RefreshUI();

		return true;
	}

	RewardValuePair _reward;

	public void SetInfo(RewardValuePair reward)
	{
		_reward = reward;
		RefreshUI();
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		Sprite sprite = GetRewardSprite(_reward.type);
		GetImage((int)Images.RewardIcon).sprite = sprite;

		GetText((int)Texts.RewardText).text = Utils.GetRewardString(_reward.type);

		if (_reward.type == RewardType.Money)
			GetText((int)Texts.RewardValueText).text = Utils.GetMoneyString(_reward.value);
		else if (_reward.type == RewardType.SalaryIncrease)
			GetText((int)Texts.RewardValueText).text = $"{Utils.GetRewardValueString((int)(_reward.value + Managers.Game.SalaryAdditionalIncreasePercent))}%";
		else
			GetText((int)Texts.RewardValueText).text = Utils.GetRewardValueString(_reward.value);

		GetText((int)Texts.RewardValueText).color = Utils.GetRewardColor(_reward.type, _reward.value);
	}

	Sprite GetRewardSprite(RewardType rewardType)
	{
		string path = "";

		switch (rewardType)
		{
			case RewardType.Hp:
				path = "Sprites/Main/Ability/icon_strength";
				break;
			case RewardType.WorkAbility:
				path = "Sprites/Main/Ability/icon_ability";
				break;
			case RewardType.Likeability:
				path = "Sprites/Main/Ability/icon_heart";
				break;
			case RewardType.Luck:
				path = "Sprites/Main/Ability/icon_luck";
				break;
			case RewardType.Stress:
				path = "Sprites/Main/Ability/icon_stress";
				break;
			case RewardType.Money:
				path = "Sprites/Main/Project/icon_coin1";
				break;
			case RewardType.Block:
				path = "Sprites/Main/Project/icon_coin2";
				break;
			case RewardType.SalaryIncrease:
				path = "Sprites/Main/Project/icon_coin1";
				break;
		}

		return Managers.Resource.Load<Sprite>(path);
	}
}
