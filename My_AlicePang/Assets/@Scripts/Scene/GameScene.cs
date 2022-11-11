using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using static Define;
using System.Linq;

public class GameScene : BaseScene
{
	class MissionData
	{
		public int currentCount;
		public int clearCount;
	}

	Dictionary<int, MissionData> _stageMissionData;
	int _remainingTurn;

	BattleState _state { get; set; } = BattleState.Ready;
	public BattleState State
	{
		get { return _state; }
		set
		{
			_state = value;
			switch (value)
			{
				case BattleState.Ready:
					_gameSceneUI.ShowWeaponButtons(false);
					OnChangeWeapon(WeaponRangeType.None);
					break;
				case BattleState.PlayerInput:
					_gameSceneUI.ShowWeaponButtons(true);
					OnChangeWeapon(Managers.Game.WeaponType);
					break;
				case BattleState.PlayerAttack:
					_gameSceneUI.ShowWeaponButtons(false);
					OnChangeWeapon(WeaponRangeType.None);
					break;
				case BattleState.BossAttack:
					_gameSceneUI.ShowWeaponButtons(false);
					OnChangeWeapon(WeaponRangeType.None);
					break;
				case BattleState.MonsterAttack:
					_gameSceneUI.ShowWeaponButtons(false);
					OnChangeWeapon(WeaponRangeType.None);
					break;
				case BattleState.GameOver:
					_gameSceneUI.ShowWeaponButtons(false);
					OnChangeWeapon(WeaponRangeType.None);
					break;
			}
		}
	}

	UI_GameScene _gameSceneUI;

    [SerializeField]
    SpriteRenderer _bg;

	[SerializeField]
	RadarController _shortRangeRadar;
	[SerializeField]
	RadarController _middleRangeRadar;
	[SerializeField]
	RadarController _longRangeRadar;

	IEnumerator _currentSequenceCoroutine;
	bool _gameStart = false;

	StageData _stageData;

	protected override bool Init()
	{
		if (base.Init() == false)
			return false;

		SceneType = Define.SceneType.GameScene;

		Managers.Object.ResetStageObjects();

        _shortRangeRadar.SetInfo(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID].RadarID);
		_middleRangeRadar.SetInfo(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID].RadarID);
		_longRangeRadar.SetInfo(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID].RadarID);

		OnChangeWeapon(WeaponRangeType.None);
		Managers.Game.DisableWeaponRangeType = WeaponRangeType.None;

		StartCoroutine(CoWaitLoad());

		return true;
	}

	IEnumerator CoWaitLoad()
	{
		while (Managers.Data.Loaded() == false)
			yield return null;
		//while (Managers.UI.SceneUI == null)
		//	yield return null;

        // TEMP
        Managers.Object.SpawnPlayer("Player", Vector2.up * 12f);
		int templateID = (Managers.Game.SelectedChapter - 1) * 20 + Managers.Game.SelectedStage;

		if (Managers.Data.Stages.TryGetValue(templateID, out StageData stageData) == false)
			yield break;

		_stageData = stageData;
		_stageMissionData = new Dictionary<int, MissionData>();
		_remainingTurn = _stageData.Turn;

		Managers.Resource.LoadAsync<Sprite>(_stageData.MapName, callback: (sprite) =>
		{
			_bg.sprite = sprite;
		});

		Managers.UI.ShowSceneUI<UI_GameScene>(callback: (gameSceneUI) =>
		{ 
			_gameSceneUI = gameSceneUI;

			for (int i = 0; i < _stageData.respawnData.Count; i++)
			{
				if (_stageData.respawnData[i].ClearCount <= 0)
					continue;

				_stageMissionData.Add(_stageData.respawnData[i].MonsterID,
						new MissionData
						{
							currentCount = 0,
							clearCount = _stageData.respawnData[i].ClearCount,
						});
			}
			_gameSceneUI.SetInfo(_stageData.respawnData, _remainingTurn);
		});


		Managers.Object.LoadStageData(_stageData);

		Managers.Sound.Play(Sound.Bgm, "Sound_Battle1");

		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(RespawnMonsters(LAYER_COUNT - 1));
		_gameStart = true;
	}

	IEnumerator RespawnMonsters(int layer)
	{
		Managers.Object.ClearFreeIndex(layer);
		

		yield return StartCoroutine(Managers.Object.SpawnMonster(layer));
	}

	void Update()
	{
		if (!_gameStart)
			return;

		switch (State)
		{
			case BattleState.Ready:
				UpdateReady();
				break;
			case BattleState.PlayerInput:
				UpdatePlayerInput();
				break;
			case BattleState.PlayerAttack:
				UpdatePlayerAttack();
				break;
			case BattleState.BossAttack:
				UpdateBossAttack();
				break;
				//case BattleState.MonsterAttack:
				//	UpdateMonsterAttack();
				//	break;
		}
	}

	void UpdatePlayerInput()
	{

	}

	public void BossAttack()
	{
		if (Managers.Object.Boss == null)
		{
			State = BattleState.MonsterAttack;
			StartMonsterAttack();
			return;
		}

		Managers.Object.Boss.StartBossTurn();
	}

	void UpdateBossAttack()
	{
		if (Managers.Object.Boss.IsBusy)
			return;

		State = BattleState.MonsterAttack;
		StartMonsterAttack();
	}

	public void UpdatePlayerAttack()
	{
		if (Managers.Object.Player.IsBusy)
			return;


		if (CheckClearStage())
		{
			int killReward = Managers.Game.CurrentStageGetCoin;
			Managers.UI.ShowPopupUI<UI_GameClearPopup>(callback: (popup) =>
			 {
				 popup.SetInfo(killReward, _stageData.ClearGoldReward, _stageData.StageID);
			 });
			State = BattleState.GameOver;

			Managers.Game.GetStageCoin(_stageData.ClearGoldReward);

			Managers.Game.CheckBreakStageRecord();

			_gameSceneUI.Combo(0);
			return;
		}
		if (_currentSequenceCoroutine != null)
			return;

		//Managers.Object.AnimateCoin();
		_gameSceneUI.Combo(0);

		_remainingTurn--;

		_gameSceneUI.SetTurn(_remainingTurn);
		if(_remainingTurn <= 0)
        {
			GameOver();
			return;
        }
        State = BattleState.BossAttack;

		BossAttack();
	}

	void StartMonsterAttack()
	{
		Managers.Sound.Play(Sound.Effect, "Sound_MonsterTurn");
		_currentSequenceCoroutine = MonsterAttackCo();

		StartCoroutine(_currentSequenceCoroutine);
	}

	IEnumerator MonsterAttackCo()
	{
		
		yield return new WaitForSeconds(0.1f);
		// 가까운 몬스터 순서대로 AI 시작하고 상태 전환
		var monsters = Managers.Object.GetMonsters();

		foreach (var monster in monsters)
			monster.StartMonsterTurn();

		foreach (MonsterController monster in monsters)
			yield return StartCoroutine(monster.StartMonsterAttack());

		while (true)
		{
			if (monsters.Count == monsters.FindAll(monster => !monster.IsBusy).Count)
			{
				break;
			}
			yield return null;
		}

		yield return StartCoroutine(RespawnMonsters(LAYER_COUNT - 1));
		State = BattleState.Ready;

		_currentSequenceCoroutine = null;
	}

	void UpdateReady()
	{
		var monsters = Managers.Object.GetMonsters();
		foreach (var monster in monsters)
		{
			if (monster.IsBusy)
				return;
		}
		State = BattleState.PlayerInput;
		Managers.Game.PlayerInput();
	}

	public void GameOver()
	{
		State = BattleState.GameOver;
		if (_currentSequenceCoroutine != null)
		{
			StopCoroutine(_currentSequenceCoroutine);
			_currentSequenceCoroutine = null;
		}
		_gameSceneUI.Combo(0);

		var monsters = Managers.Object.GetMonsters();
		foreach (MonsterController monster in monsters)
			monster.EndTurn();

		Managers.UI.ShowPopupUI<UI_GameOverPopup>(callback: (popup) =>
		 {
			 //popup.Init();
		 });
	}

	public void RestartGame()
	{
		if (_currentSequenceCoroutine != null)
		{
			StopCoroutine(_currentSequenceCoroutine);
			_currentSequenceCoroutine = null;
		}

		Managers.Object.ResetStageObjects();

		OnChangeWeapon(WeaponRangeType.Short);

		Managers.Object.Player.RevivePlayer();

		StartCoroutine(RespawnMonsters(LAYER_COUNT - 1));
		State = BattleState.PlayerInput;
        Managers.Game.PlayerInput();
    }

    public void RevivePlayer()
	{
		if (_currentSequenceCoroutine != null)
		{
			StopCoroutine(_currentSequenceCoroutine);
			_currentSequenceCoroutine = null;
		}

		_remainingTurn += 5;

		Managers.Object.Player.RevivePlayer();
		State = BattleState.PlayerInput;
        Managers.Game.PlayerInput();
    }

    public void PlayerAttackStart()
	{
		List<MonsterController> monsters = Managers.Object.GetLowestHpSelectedMonsters();
		Managers.Object.Player.StartAttack(monsters);

		State = BattleState.PlayerAttack;
	}

	public bool CheckClearStage()
	{
		//bool clear = true;
		for (int i = 0; i < _stageData.respawnData.Count; i++)
		{
			int monsterID = _stageData.respawnData[i].MonsterID;

			if (!Check_ClearMonster(monsterID))
				return false;

		}
		//Managers.Object.AnimateCoin();
		//gameClear
		Debug.Log("Game Clear");
		return true;
		//ui표기

	}

	bool Check_ClearMonster(int monsterTemplateID)
	{
		if (!_stageMissionData.TryGetValue(monsterTemplateID, out MissionData missionData))
			return true;

		if (missionData.currentCount >= missionData.clearCount)
			return true;
		else
			return false;
	}

	public int CountRemainMonster(MonsterController mc)
	{
		if (!_stageMissionData.TryGetValue(mc.TemplateID, out MissionData missionData))
			return 0;

		missionData.currentCount++;
		return missionData.clearCount - missionData.currentCount;
	}

	#region EventHandler
	void OnChangeWeapon(WeaponRangeType weaponType)
	{
		if (_init == false)
			return;

		switch (weaponType)
		{
			case WeaponRangeType.None:
				_shortRangeRadar.gameObject.SetActive(false);
				_middleRangeRadar.gameObject.SetActive(false);
				_longRangeRadar.gameObject.SetActive(false);
				break;
			case WeaponRangeType.Short:
				_shortRangeRadar.gameObject.SetActive(true);
				_shortRangeRadar.Rotate();
				_middleRangeRadar.gameObject.SetActive(false);
				_longRangeRadar.gameObject.SetActive(false);
				break;
			case WeaponRangeType.Middle:
				_shortRangeRadar.gameObject.SetActive(false);
				_middleRangeRadar.gameObject.SetActive(true);
				_middleRangeRadar.Rotate();
				_longRangeRadar.gameObject.SetActive(false);
				break;
			case WeaponRangeType.Long:
				_shortRangeRadar.gameObject.SetActive(false);
				_middleRangeRadar.gameObject.SetActive(false);
				_longRangeRadar.gameObject.SetActive(true);
				_longRangeRadar.Rotate();
				break;
		}
		Managers.Object.Player?.ChangeWeaponImage();
	}

	void OnPlayerAttack()
	{
		if (State != BattleState.PlayerInput)
			return;

		switch (Managers.Game.WeaponType)
		{
			case WeaponRangeType.None:
				return;
			case WeaponRangeType.Short:
				_shortRangeRadar.RadarFadeInOut();
				break;
			case WeaponRangeType.Middle:
				_middleRangeRadar.RadarFadeInOut();
				break;
			case WeaponRangeType.Long:
				_longRangeRadar.RadarFadeInOut();
				break;
		}
	}

	void OnMonsterDead(MonsterController mc)
	{
		Managers.Object.DespawnMonster(mc);
	}

	void OnEnable()
	{
		if (Managers.Game == null)
			return;

		Managers.Game.OnChangeWeapon -= OnChangeWeapon;
		Managers.Game.OnChangeWeapon += OnChangeWeapon;
		Managers.Game.OnPlayerAttack -= OnPlayerAttack;
		Managers.Game.OnPlayerAttack += OnPlayerAttack;
		Managers.Game.OnMonsterDead -= OnMonsterDead;
		Managers.Game.OnMonsterDead += OnMonsterDead;
	}

	void OnDisable()
	{
		if (Managers.Game == null)
			return;

		Managers.Game.OnChangeWeapon -= OnChangeWeapon;
		Managers.Game.OnPlayerAttack -= OnPlayerAttack;
		Managers.Game.OnMonsterDead -= OnMonsterDead;
		Managers.Object.Player = null;
	}
	#endregion
}
