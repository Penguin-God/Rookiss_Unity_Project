using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;
using static Define;

public struct RewardValuePair
{
	public RewardType type;
	public int value;
}

public enum ResultType
{
	Victory,
	Defeat,
	Project,
	GoHome,
	Dialogue,
	SalaryNegotiationSuccess,
	SalaryNegotiationFail
}

public class UI_ResultPopup : UI_Popup
{
	enum Texts
	{
		TitleText,
		PleaseTouchText,
		AnimatedImageText
	}

	enum GameObjects
	{
		Result,
		SuccessTitle,
		AnimatedImage,
		RewardsLayoutGroup,
		BattleFail
	}

	enum Images
	{
		CartoonImage,
	}

	ResultType _type;
	List<RewardValuePair> _rewards;
	string _path;
	string _text;

	List<UI_ResultItem> _items = new List<UI_ResultItem>();
	Coroutine _coWaitAnim = null;
	bool _animEnded = false;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindObject((typeof(GameObjects)));
		BindImage((typeof(Images)));

		switch (_type)
		{
			case ResultType.Victory:
				gameObject.BindEvent(OnCloseCartoon);
				break;
			case ResultType.Project:
				gameObject.BindEvent(OnCloseProject);
				break;
			case ResultType.GoHome:
				gameObject.BindEvent(OnCloseProject);
				break;
			default:
				gameObject.BindEvent(OnClosePopup);
				break;
		}

		RefreshUI();

		return true;
	}

	public void SetInfo(ResultType type, List<RewardValuePair> rewards, string path, string text)
	{
		_type = type;
		_rewards = rewards;
		_path = path;
		_text = text;
		_animEnded = false;
		RefreshUI();
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		switch (_type)
		{
			case ResultType.Victory:
				GetText((int)Texts.TitleText).text = Managers.GetText(Define.PromoteSuccess);
				GetObject((int)GameObjects.BattleFail).SetActive(false);
				break;
			case ResultType.Defeat:
				GetText((int)Texts.TitleText).text = Managers.GetText(Define.PromoteFail);
				GetObject((int)GameObjects.BattleFail).SetActive(true);
				break;
			case ResultType.Project:
				GetText((int)Texts.TitleText).text = Managers.GetText(Define.ProjectSuccess);
				GetObject((int)GameObjects.BattleFail).SetActive(false);
				break;
			case ResultType.GoHome:
				GetText((int)Texts.TitleText).text = Managers.GetText(Define.GoHomeSuccess);
				GetObject((int)GameObjects.BattleFail).SetActive(false);
				break;
			case ResultType.Dialogue:
				GetText((int)Texts.TitleText).text = Managers.GetText(Define.DialogueSuccess);
				GetObject((int)GameObjects.BattleFail).SetActive(false);
				break;
			case ResultType.SalaryNegotiationSuccess:
				GetText((int)Texts.TitleText).text = Managers.GetText(Define.SalaryNegotiationSuccess);
				GetObject((int)GameObjects.BattleFail).SetActive(false);
				break;
			case ResultType.SalaryNegotiationFail:
				GetText((int)Texts.TitleText).text = Managers.GetText(Define.SalaryNegotiationFail);
				GetObject((int)GameObjects.BattleFail).SetActive(false);
				break;
		}
		
		if (_type == ResultType.Victory)
		{			
			GetObject((int)GameObjects.SuccessTitle).SetActive(false);
			GetText((int)Texts.TitleText).gameObject.SetActive(false);
			GetImage((int)Images.CartoonImage).gameObject.SetActive(true);
			//GetImage((int)Images.CartoonImage).gameObject.GetOrAddComponent<DOTweenAnimation>().DORestartAllById("Victory");
			GetImage((int)Images.CartoonImage).sprite = Managers.Resource.Load<Sprite>(_path);
		}
		else
		{
			GetObject((int)GameObjects.SuccessTitle).SetActive(false);
			GetText((int)Texts.TitleText).gameObject.SetActive(true);
			GetImage((int)Images.CartoonImage).gameObject.SetActive(false);

			if (_animEnded == false && string.IsNullOrEmpty(_path) == false)
			{
				// 애니메이션 대기
				GetObject((int)GameObjects.AnimatedImage).GetOrAddComponent<BaseController>().SetSkeletonAsset(_path);
				_coWaitAnim = StartCoroutine(CoWaitAnimation(1.5f));
				Managers.Sound.Play(Sound.Effect, "Sound_ProjectItem");
			}
		}

		if (_coWaitAnim != null)
		{
			GetObject((int)GameObjects.Result).gameObject.SetActive(false);
			GetObject((int)GameObjects.AnimatedImage).gameObject.SetActive(true);
		}
		else
		{
			GetObject((int)GameObjects.Result).gameObject.SetActive(true);
			GetObject((int)GameObjects.AnimatedImage).gameObject.SetActive(false);
		}

		GetText((int)Texts.AnimatedImageText).text = _text;

		// Rewards
		GameObject parent = GetObject((int)GameObjects.RewardsLayoutGroup);
		foreach (Transform t in parent.transform)
			Managers.Resource.Destroy(t.gameObject);

		_items.Clear();

		for (int i = 0; i < _rewards.Count; i++)
		{
			RewardValuePair reward = _rewards[i];
			if (reward.type == RewardType.Promotion)
				continue;

			UI_ResultItem item = Managers.UI.MakeSubItem<UI_ResultItem>(parent.transform);
			item.SetInfo(reward);

			_items.Add(item);
		}
	}

	void OnCloseCartoon()
	{
		Debug.Log("OnCloseCartoon");

		GetObject((int)GameObjects.SuccessTitle).SetActive(true);
		GetText((int)Texts.TitleText).gameObject.SetActive(false);
		GetImage((int)Images.CartoonImage).gameObject.SetActive(false);

		// 다음 터치에는 종료
		gameObject.BindEvent(OnClosePopup);
	}

	void OnCloseProject()
	{
		GetObject((int)GameObjects.Result).gameObject.SetActive(true);
		GetObject((int)GameObjects.AnimatedImage).gameObject.SetActive(false);

		// 다음 터치에는 종료
		gameObject.BindEvent(OnClosePopup);
	}

	void OnClosePopup()
	{
		Debug.Log("OnClosePopup");
		Managers.Sound.Play(Sound.Effect, "Sound_ResultStat");

		// 보상 적용
		foreach (RewardValuePair reward in _rewards)
		{
			switch (reward.type)
			{
				case RewardType.Hp:
					Managers.Game.Hp += reward.value;
					break;
				case RewardType.WorkAbility:
					Managers.Game.WorkAbility += reward.value;
					break;
				case RewardType.Likeability:
					Managers.Game.Likeability += reward.value;
					break;
				case RewardType.Luck:
					Managers.Game.Luck += reward.value;
					break;
				case RewardType.Stress:
					Managers.Game.Stress += reward.value;
					break;
				case RewardType.Block:
					Managers.Game.BlockCount += reward.value;
					break;
				case RewardType.Money:
					Managers.Game.Money += (int)(reward.value * (100.0f + Managers.Game.AdditionalRevenuePercent) / 100.0f);
					break;
				case RewardType.SalaryIncrease:
					Managers.Game.Salary = (int)(Managers.Game.Salary * (100.0f + reward.value + Managers.Game.SalaryAdditionalIncreasePercent) / 100.0f);
					break;
				case RewardType.Promotion:
					Managers.Game.JobTitle = (JobTitleType)reward.value; // 승급
					break;
			}
		}

		// 특수 사양 적용
		switch (_type)
		{
			case ResultType.Victory:
				break;
			case ResultType.Defeat:
				break;
			case ResultType.Project:
				break;
			case ResultType.GoHome:
				Managers.Game.Hp = Managers.Game.MaxHp;
				break;
			case ResultType.Dialogue:
				break;
			case ResultType.SalaryNegotiationSuccess:
				break;
			case ResultType.SalaryNegotiationFail:
				break;
		}

		Managers.UI.FindPopup<UI_PlayPopup>().RefreshStat();
		Managers.UI.FindPopup<UI_PlayPopup>().RefreshMoney();

		if (_type == ResultType.Victory)
		{
			Managers.UI.FindPopup<UI_PlayPopup>().PopulateBattle();
			Managers.UI.FindPopup<UI_PlayPopup>().ShowTab(UI_PlayPopup.PlayTab.Battle);
		}	

		Managers.UI.ClosePopupUI(this);
	}

	IEnumerator CoWaitAnimation(float seconds)
	{
		_animEnded = false;
		yield return new WaitForSeconds(seconds);
		_animEnded = true;
		_coWaitAnim = null;
		RefreshUI();
	}
}
