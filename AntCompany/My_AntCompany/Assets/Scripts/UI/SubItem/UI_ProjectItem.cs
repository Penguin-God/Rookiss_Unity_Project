//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_ProjectItem : UI_Base
{
	enum Texts
	{
		TitleText,
	}

	enum Images
	{
		Icon,
		ProjectCoolTime
	}

	enum GameObjects
	{
		AbilityLayoutGroup
	}

	ProjectData _data;
	float _delay;
	List<UI_ProjectItemSubItem> _items = new List<UI_ProjectItemSubItem>();

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindImage(typeof(Images));
		BindObject(typeof(GameObjects));

		gameObject.BindEvent(OnClickProjectItem);
		//gameObject.GetComponent<DOTweenAnimation>().delay = _delay;

		RefreshUI();

		return true;
	}

	private void Update()
	{
		float ratio = GetProjectWaitRatio();
		GetImage((int)Images.ProjectCoolTime).fillAmount = 1.0f - ratio;
	}

	float GetProjectWaitRatio()
	{
		float playTime = Managers.Game.PlayTime;
		float projectTime = Managers.Game.LastProjectTime;

		float ratio = 1.0f;
		if (projectTime > 0 && projectTime < playTime)
			ratio = (playTime - projectTime) / Managers.Game.LastProjectCoolTime;

		return ratio;
	}

	void OnClickProjectItem()
	{
		Debug.Log("OnClickProjectItem");
		Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");

		float ratio = GetProjectWaitRatio();
		if (ratio < 1.0f)
			return;

		Managers.UI.ShowPopupUI<UI_ConfirmPopup>().SetInfo(() => 
		{
			Managers.Game.LastProjectTime = Managers.Game.PlayTime; 
			Managers.Game.LastProjectCoolTime = (_data.coolTime * (100.0f - Managers.Game.ProjectCoolTimePercent) / 100.0f) * Managers.Game.SecondPerGameDay;

			Managers.Game.Projects[_data.ID - 1]++;

			Managers.Game.RefreshProjectCollections();

			List<RewardValuePair> rewards = MakeRewards(_data);
			Managers.UI.ShowPopupUI<UI_ResultPopup>().SetInfo(ResultType.Project, rewards, _data.aniPath, "");
		}, Managers.GetText(Define.ProjectConfirmText));
	}

	public void SetInfo(ProjectData data, float delay)
	{
		_data = data;
		_delay = delay;
		RefreshUI();
	}

	public void RefreshCanExecuteProject()
	{
		if (CanExecuteProject())
			gameObject.SetActive(true);
		else
			gameObject.SetActive(false);
	}

	bool CanExecuteProject()
	{
		if (_data.reqLikability > Managers.Game.Likeability)
			return false;
		if (_data.reqAbility > Managers.Game.WorkAbility)
			return false;
		if (_data.reqLuck > Managers.Game.Luck)
			return false;

		return true;
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;

		if (string.IsNullOrEmpty(_data.iconPath) == false)
		{
			Sprite sprite = Managers.Resource.Load<Sprite>(_data.iconPath);
			GetImage((int)Images.Icon).sprite = sprite;
		}

		GetText((int)Texts.TitleText).text = Managers.GetText(_data.projectName);

		PopulateSubItems();
		RefreshCanExecuteProject();
	}

	void PopulateSubItems()
	{
		GameObject parent = GetObject((int)GameObjects.AbilityLayoutGroup);
		foreach (Transform t in parent.transform)
			Managers.Resource.Destroy(t.gameObject);

		_items.Clear();

		if (_data.difWorkAbility != 0)
		{
			UI_ProjectItemSubItem item = Managers.UI.MakeSubItem<UI_ProjectItemSubItem>(parent.transform);
			item.SetInfo(RewardType.WorkAbility, _data.difWorkAbility);
			_items.Add(item);
		}

		if (_data.difLikeability != 0)
		{
			UI_ProjectItemSubItem item = Managers.UI.MakeSubItem<UI_ProjectItemSubItem>(parent.transform);
			item.SetInfo(RewardType.Likeability, _data.difLikeability);
			_items.Add(item);
		}

		if (_data.difLuck != 0)
		{
			UI_ProjectItemSubItem item = Managers.UI.MakeSubItem<UI_ProjectItemSubItem>(parent.transform);
			item.SetInfo(RewardType.Luck, _data.difLuck);
			_items.Add(item);
		}

		if (_data.difStress != 0)
		{
			UI_ProjectItemSubItem item = Managers.UI.MakeSubItem<UI_ProjectItemSubItem>(parent.transform);
			item.SetInfo(RewardType.Stress, _data.difStress);
			_items.Add(item);
		}

		if (_data.difBlock != 0)
		{
			UI_ProjectItemSubItem item = Managers.UI.MakeSubItem<UI_ProjectItemSubItem>(parent.transform);
			item.SetInfo(RewardType.Block, _data.difBlock);
			_items.Add(item);
		}

		if (_data.difMoney != 0)
		{
			UI_ProjectItemSubItem item = Managers.UI.MakeSubItem<UI_ProjectItemSubItem>(parent.transform);
			item.SetInfo(RewardType.Money, _data.difMoney);
			_items.Add(item);
		}
	}

	List<RewardValuePair> MakeRewards(ProjectData data)
	{
		List<RewardValuePair> rewards = new List<RewardValuePair>();

		if (data.difWorkAbility != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.WorkAbility, value = data.difWorkAbility });

		if (data.difLikeability != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Likeability, value = data.difLikeability });

		if (data.difLuck != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Luck, value = data.difLuck });

		if (data.difStress != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Stress, value = data.difStress });

		if (data.difBlock != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Block, value = data.difBlock });

		if (data.difMoney != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Money, value = data.difMoney });

		return rewards;
	}
}
