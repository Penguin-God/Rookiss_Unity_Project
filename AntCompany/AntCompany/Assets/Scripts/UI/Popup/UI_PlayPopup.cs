//using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;


public class UI_PlayPopup : UI_Popup
{
	enum Texts
	{
		JobText,
		PlayTimeText,
		StressBarText,
		HpBarText,
		MoneyText,
		BlockText,
		AbilityButtonText,
		ProjectButtonText,
		BattleButtonText,
		ShopButtonText,
		CollectionSuccessText,
	}

	enum Buttons
	{
		AbilityButton,
		ProjectButton,
		BattleButton,
		ShopButton,
		PlayerInfoButton,
		TutorialButton
	}

	enum Images
	{
		StressBarFill,
		HpBarFill,
		AbilityBox,
		ProjectBox,
		BattleBox,
		ShopBox,
		CollectionSuccess
	}

	enum GameObjects
	{
		ProjectContent,
		AbilityContent,
		BattleContent,
		ShopContent,
		ProjectTab,
		AbilityTab,
		BattleTab,
		ShopTab,
		Cat,
		Intern,
		Sinib,
		Daeri,
		Gwajang,
		Bujang,
		Esa,
		Sajang,
		HpBar,
		Coin1,
		StressBarFill
	}

	public enum PlayTab
	{
		None,
		Ability,
		Project,
		Battle,
		Shop
	}

	enum AbilityItems
	{
		UI_AbilityItem_Stress, 
		UI_AbilityItem_HP,
		UI_AbilityItem_Work,
		UI_AbilityItem_Likeable,		
		UI_AbilityItem_Luck,
	}

	List<UI_ProjectItem> _projectItems = new List<UI_ProjectItem>();
	List<UI_BattleItem> _battleItems = new List<UI_BattleItem>();
	List<UI_ShopItem> _shopItems = new List<UI_ShopItem>();
	PlayTab _tab = PlayTab.None;

	GameManagerEx _game;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		_game = Managers.Game;

		BindText(typeof(Texts));
		BindButton(typeof(Buttons));
		BindObject(typeof(GameObjects));
		BindImage(typeof(Images));
		Bind<UI_AbilityItem>(typeof(AbilityItems));

		GetButton((int)Buttons.AbilityButton).gameObject.BindEvent(() => ShowTab(PlayTab.Ability));
		GetButton((int)Buttons.ProjectButton).gameObject.BindEvent(() => ShowTab(PlayTab.Project));
		GetButton((int)Buttons.BattleButton).gameObject.BindEvent(() => ShowTab(PlayTab.Battle));
		GetButton((int)Buttons.ShopButton).gameObject.BindEvent(() => ShowTab(PlayTab.Shop));

		GetButton((int)Buttons.PlayerInfoButton).gameObject.BindEvent(OnClickPlayerInfoButton);

		GetButton((int)Buttons.TutorialButton).gameObject.BindEvent(OnClickTutorialButton);
		
		PopulateProject();
		PopulateBattle();
		PopulateShop();

		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_Stress).SetInfo(StatType.Stress, 0.0f, 0.1f); 
		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_HP).SetInfo(StatType.MaxHp, 0.1f, 0.1f);
		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_Work).SetInfo(StatType.WorkAbility, 0.2f, 0.1f);
		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_Likeable).SetInfo(StatType.Likeability, 0.4f, 0.1f);		
		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_Luck).SetInfo(StatType.Luck, 0.8f, 0.1f);

		foreach (JobTitleType type in Enum.GetValues(typeof(JobTitleType)))
		{
			GetPlayer(type).SetInfo(type);

			PlayerState ps = Managers.Game.GetPlayerState(type);
			
			// 게임 처음 실행
			if (ps.state == AnimState.None)
			{
				if (type == JobTitleType.Sajang)
					GetPlayer(type).State = AnimState.Walking;
				else if (type != JobTitleType.Cat)
					GetPlayer(type).State = AnimState.Working;
			}
			else
			{
				GetPlayer(type).State = ps.state;
				if (ps.dialogueEvent)
					GetPlayer(type).DialogueEvent = ps.dialogueEvent;
				if (ps.goHomeEvent)
					GetPlayer(type).GoHomeEvent = ps.goHomeEvent;
			}
		}
		
		RefreshUI();

		_game.CalcNextDialogueDay();

		StartCoroutine(CoSaveGame(3.0f));
		Managers.Sound.Clear();
		Managers.Sound.Play(Sound.Bgm, "Sound_MainPlayBGM", volume: 0.2f);

		ShowTab(PlayTab.Ability);

		GetImage((int)Images.CollectionSuccess).gameObject.SetActive(false);

		Managers.Game.OnNewCollection = OnNewCollection;

		return true;
	}

	void OnNewCollection(CollectionData data)
	{
		GetImage((int)Images.CollectionSuccess).gameObject.SetActive(true);
		GetText((int)Texts.CollectionSuccessText).text = $"{Managers.GetText(data.nameID)} {Managers.GetText(Define.CollectionSuccessPopup)}";

		if (_coHideCollection != null)
			StopCoroutine(_coHideCollection);
		_coHideCollection = StartCoroutine(CoHideCollection(3.0f));
	}

	Coroutine _coHideCollection;
	IEnumerator CoHideCollection(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		GetImage((int)Images.CollectionSuccess).gameObject.SetActive(false);
	}

	PlayerController GetPlayer(JobTitleType type)
	{
		switch (type)
		{
			case JobTitleType.Cat:
				return GetObject((int)GameObjects.Cat).GetOrAddComponent<PlayerController>();
			case JobTitleType.Intern:
				return GetObject((int)GameObjects.Intern).GetOrAddComponent<PlayerController>();
			case JobTitleType.Sinib:
				return GetObject((int)GameObjects.Sinib).GetOrAddComponent<PlayerController>();						
			case JobTitleType.Daeri:
				return GetObject((int)GameObjects.Daeri).GetOrAddComponent<PlayerController>();
			case JobTitleType.Gwajang:
				return GetObject((int)GameObjects.Gwajang).GetOrAddComponent<PlayerController>();
			case JobTitleType.Bujang:
				return GetObject((int)GameObjects.Bujang).GetOrAddComponent<PlayerController>();
			case JobTitleType.Esa:
				return GetObject((int)GameObjects.Esa).GetOrAddComponent<PlayerController>();
			case JobTitleType.Sajang:
				return GetObject((int)GameObjects.Sajang).GetOrAddComponent<PlayerController>();
		};

		return null;
	}

	float _pitch = NORMAL_PITCH;
	const float FAST_PITCH = 1.2f;
	const float NORMAL_PITCH = 1.0f;

	private void Update()
	{
		if (Managers.UI.PeekPopupUI<UI_PlayPopup>() != this)
			return;

		if (Managers.Game.StressPercent >= 70)
		{
			if (_pitch != FAST_PITCH)
			{
				_pitch = FAST_PITCH;
				Managers.Sound.SetPitch(Sound.Bgm, FAST_PITCH);
				//Managers.Game.OnStressChanged = () => GetImage((int)Images.StressBarFill).GetComponent<DOTweenAnimation>().DORestartAllById("GetStress");
			}
		}
		else
		{
			if (_pitch != NORMAL_PITCH)
			{
				_pitch = NORMAL_PITCH;
				Managers.Sound.SetPitch(Sound.Bgm, NORMAL_PITCH);
				//Managers.Game.OnStressChanged = () => GetImage((int)Images.StressBarFill).GetComponent<DOTweenAnimation>().DOComplete();
			}
		}

		_game.PlayTime += Time.deltaTime;
		RefreshTime();

		int gameDay = _game.GameDay;

		// 시간 초과로 인한 엔딩 확인
		if (gameDay >= _game.MaxGameDays)
		{
			EndingData endingData = Managers.Data.Endings.Values.Where(e => { return e.type == EndingType.Level && e.value == (int)_game.JobTitle; }).FirstOrDefault();
			if (endingData != null)
				Managers.UI.ShowPopupUI<UI_EndingPopup>().SetInfo(endingData);
		}
		// 스트레스 초과로 인한 엔딩 확인
		else if (_game.Stress >= _game.MaxStress)
		{
			EndingData endingData = Managers.Data.Endings.Values.Where(e => { return e.type == EndingType.Stress; }).FirstOrDefault();
			if (endingData != null)
				Managers.UI.ShowPopupUI<UI_EndingPopup>().SetInfo(endingData);
		}
		// 사장이 된 엔딩
		else if (_game.JobTitle == JobTitleType.Sajang) 
		{
			EndingData endingData = Managers.Data.Endings.Values.Where(e => { return e.type == EndingType.Level && e.value == (int)_game.JobTitle; }).FirstOrDefault();
			if (endingData != null)
				Managers.UI.ShowPopupUI<UI_EndingPopup>().SetInfo(endingData);
		}

		// 체력 감소
		if (_game.LastHpDecreaseDay < gameDay)
		{
			float hpDecreasePercent = 7.5f;
			int diffHp = (int)(_game.MaxHp * hpDecreasePercent / 100);
			_game.Hp = Math.Max(0, _game.Hp - diffHp);
			_game.LastHpDecreaseDay = gameDay;
			RefreshHpBar();

			// 퇴근 이벤트 발동
			if (_game.Hp == 0)
			{
				GetPlayer(JobTitleType.Sinib).State = AnimState.Sweat; // 이모티콘?
				GetPlayer(JobTitleType.Sinib).GoHomeEvent = true; // 다음 클릭시 퇴근 이벤트 진행
			}

			foreach (UI_BattleItem item in _battleItems)
				item.RefreshButton();

			// 묻어가기 :)
			RefreshStatUpgradeButtons();
		}

		// 스트레스 증가
		if (_game.LastStressIncreaseDay < gameDay)
		{
			_game.Stress += Managers.Data.Start.increaseStress;
			_game.LastStressIncreaseDay = gameDay;
			RefreshStressBar();
			Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_Stress).RefreshUI();
		}

		// 월급 처리
		int nextPayDay = _game.NextPayDay;
		if (nextPayDay <= gameDay)
		{
			_game.LastPayDay = nextPayDay;
			_game.Money += (int)(_game.Salary * (100.0f + Managers.Game.AdditionalRevenuePercent) / 100.0f);
			RefreshMoney(true);
		}

		// 대화 이벤트
		if (_game.NextDialogueDay <= _game.GameDay)
		{
			JobTitleType npcJob = Utils.GetRandomNpc();
			GetPlayer(npcJob).State = AnimState.Sweat; // 이모티콘?
			GetPlayer(npcJob).DialogueEvent = true; // 다음 클릭시 대화 이벤트 진행

			_game.CalcNextDialogueDay();
		}

		// 연봉 협상 이벤트
		if (_game.NextSalaryNegotiationDay <= _game.GameDay)
		{
			// 연봉 협상 이벤트 발동
			GetPlayer(JobTitleType.Cat).DialogueEvent = true;

			// 시간 갱신
			_game.LastSalaryNegotiationDay = _game.NextSalaryNegotiationDay;
		}
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;

		ShowTab(_tab);
		RefreshStat();
		RefreshMoney();
		RefreshTime();
	}

	public void RefreshStat()
	{
		GetText((int)Texts.JobText).text = Utils.GetJobTitleString(_game.JobTitle);

		RefreshStatUpgradeButtons();
		RefreshHpBar();
		RefreshStressBar();
		RefreshProjectUI();
	}

	void RefreshStatUpgradeButtons()
	{
		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_Stress).RefreshUI();
		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_HP).RefreshUI();
		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_Work).RefreshUI();
		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_Likeable).RefreshUI();
		Get<UI_AbilityItem>((int)AbilityItems.UI_AbilityItem_Luck).RefreshUI();
	}

	public void RefreshTime()
	{
		GetText((int)Texts.PlayTimeText).text = $"{_game.GameDay}";
	}

	public void RefreshHpBar()
	{
		float hpPercent = _game.HpPercent;
		GetImage((int)Images.HpBarFill).fillAmount = hpPercent / 100.0f;
		GetText((int)Texts.HpBarText).text = $"HP : {(int)hpPercent}%";

		if (_game.HpPercent <= 30)
		{
			//GetObject((int)GameObjects.HpBar).GetOrAddComponent<DOTweenAnimation>().DORestartAllById("Damage");
			//GetObject((int)GameObjects.HpBar).GetOrAddComponent<DOTweenAnimation>().DORestartAllById("Color");
		}
	}

	public void RefreshStressBar()
	{
		float stressPercent = _game.StressPercent;
		GetImage((int)Images.StressBarFill).fillAmount = stressPercent / 100.0f;
		GetText((int)Texts.StressBarText).text = $"Stress : {(int)stressPercent}%";
	}

	public void RefreshMoney(bool playSoundAndEffect = false)
	{
		GetText((int)Texts.BlockText).text = $"{_game.BlockCount}";

		if (GetText((int)Texts.MoneyText).text != Utils.GetMoneyString(_game.Money))
		{
			if (playSoundAndEffect)
			{
				//GetObject((int)GameObjects.Coin1).GetOrAddComponent<DOTweenAnimation>().DORestartAllById("Coin");
				Managers.Sound.Play(Sound.Effect, "Sound_Coin");
			}
			GetText((int)Texts.MoneyText).text = Utils.GetMoneyString(_game.Money);
		}	
	}

	public void RefreshProjectUI()
	{
		if (_init == false)
			return;

		foreach (UI_ProjectItem item in _projectItems)
		{
			item.RefreshCanExecuteProject();
		}
	}

	public void ShowTab(PlayTab tab)
	{
		if (_tab == tab)
			return;

		_tab = tab;

		GetObject((int)GameObjects.AbilityTab).gameObject.SetActive(false);
		GetObject((int)GameObjects.ProjectTab).gameObject.SetActive(false);
		GetObject((int)GameObjects.BattleTab).gameObject.SetActive(false);
		GetObject((int)GameObjects.ShopTab).gameObject.SetActive(false);
		GetButton((int)Buttons.AbilityButton).image.sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_05");
		GetButton((int)Buttons.ProjectButton).image.sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_06");
		GetButton((int)Buttons.BattleButton).image.sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_07");
		GetButton((int)Buttons.ShopButton).image.sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_08");
		GetImage((int)Images.AbilityBox).sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_04");
		GetImage((int)Images.ProjectBox).sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_04");
		GetImage((int)Images.BattleBox).sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_04");
		GetImage((int)Images.ShopBox).sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_04");

		switch (_tab)
		{
			case PlayTab.Ability:
				Managers.Sound.Play(Sound.Effect, "Sound_MainButton");
				GetObject((int)GameObjects.AbilityTab).gameObject.SetActive(true);
				GetObject((int)GameObjects.AbilityTab).GetComponent<ScrollRect>().ResetVertical();
				GetButton((int)Buttons.AbilityButton).image.sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_18");
				GetImage((int)Images.AbilityBox).sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_12");
				break;
			case PlayTab.Project:
				Managers.Sound.Play(Sound.Effect, "Sound_MainButton");
				GetObject((int)GameObjects.ProjectTab).gameObject.SetActive(true);
				GetObject((int)GameObjects.ProjectTab).GetComponent<ScrollRect>().ResetHorizontal();
				GetButton((int)Buttons.ProjectButton).image.sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_19");
				GetImage((int)Images.ProjectBox).sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_12");
				break;
			case PlayTab.Battle:
				Managers.Sound.Play(Sound.Effect, "Sound_MainButton");
				GetObject((int)GameObjects.BattleTab).gameObject.SetActive(true);
				GetObject((int)GameObjects.BattleTab).GetComponent<ScrollRect>().ResetHorizontal();
				GetButton((int)Buttons.BattleButton).image.sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_20");
				GetImage((int)Images.BattleBox).sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_12");
				break;
			case PlayTab.Shop:
				Managers.Sound.Play(Sound.Effect, "Sound_MainButton");
				GetObject((int)GameObjects.ShopTab).gameObject.SetActive(true);
				GetObject((int)GameObjects.ShopTab).GetComponent<ScrollRect>().ResetHorizontal();
				GetButton((int)Buttons.ShopButton).image.sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_21");
				GetImage((int)Images.ShopBox).sprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_12");
				break;
		}
	}

	void PopulateProject()
	{
		_projectItems.Clear();

		var parent = GetObject((int)GameObjects.ProjectContent);

		foreach (Transform child in parent.transform)
			Managers.Resource.Destroy(child.gameObject);

		List<ProjectData> projects = Managers.Data.Projects.Values.ToList();
		for (int i = 0; i < projects.Count; i++)
		{
			UI_ProjectItem item = Managers.UI.MakeSubItem<UI_ProjectItem>(parent.transform);
			item.SetInfo(projects[i], i * 0.1f);

			_projectItems.Add(item);
		}
	}

	public Action OnHpChanged;

	public void PopulateBattle()
	{
		OnHpChanged = null;

		_battleItems.Clear();

		var parent = GetObject((int)GameObjects.BattleContent);

		foreach (Transform child in parent.transform)
			Managers.Resource.Destroy(child.gameObject);

		// 나보다 직급이 낮으면 전투할 필요 없다.
		int level = (int)_game.JobTitle;
		List<PlayerData> enemies = Managers.Data.Players.Values.Where(p => { return p.ID > level && p.ID < JOB_TITLE_TYPE_COUNT; }).ToList(); // 고양이 제외!
			
		foreach (PlayerData enemy in enemies)
		{
			UI_BattleItem item = Managers.UI.MakeSubItem<UI_BattleItem>(parent.transform);
			item.SetInfo(enemy); 
			
			_battleItems.Add(item);			
		}
	}

	public void PopulateShop()
	{
		_shopItems.Clear();

		var parent = GetObject((int)GameObjects.ShopContent);

		foreach (Transform child in parent.transform)
			Managers.Resource.Destroy(child.gameObject);

		foreach (ShopData shopData in Managers.Data.Shops.Values)
		{
			// 이미 광고 제거 구매했으면 안 뜬다
			if (shopData.rewardType == ShopRewardType.NoAds && Managers.IAP.IsNoAds)
				continue;

			UI_ShopItem item = Managers.UI.MakeSubItem<UI_ShopItem>(parent.transform);
			item.SetInfo(shopData);

			_shopItems.Add(item);
		}
	}

	void OnClickPlayerInfoButton()
	{
		Debug.Log("OnClickPlayerInfoButton");
		Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");
		Managers.UI.ShowPopupUI<UI_PlayerInfoPopup>();
	}

	void OnClickTutorialButton()
	{
		Debug.Log("OnClickTutorialButton");

		Managers.UI.ShowPopupUI<UI_IntroPopup>().SetInfo((int)UI_IntroPopup.GameObjects.Guide1, (int)UI_IntroPopup.GameObjects.Guide2, null) ;
	}

	IEnumerator CoSaveGame(float interval)
	{
		while (true)
		{
			yield return new WaitForSeconds(interval);
			Managers.Game.SaveGame();
		}
	}
}
