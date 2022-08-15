using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using DG.Tweening;
using static Define;
using UnityEngine.Advertisements;

public class UI_BattlePopup : UI_Popup
{
	enum Texts
	{
		BlockText,
		PlayerHpText,
		EnemyHpText,
		BubbleText,
	}

	enum GameObjects
	{
		Player,
		Enemy,
		Block,
		BlockStart,
		BlockDest,
		GaugeBlock,
		JitterScreen,
		BlockEffect
	}

	enum Buttons
	{
		BlockButton
	}

	enum Images
	{
		BlockBar,
		BlockGood,
		BlockPerfect,
		Bubble,
	}

	enum FireType
	{
		Normal,
		Good,
		Perfect,
	}

	Coroutine _waitCoroutine;
	float _blockSpeed = 700.0f;
	GameObject _block;
	GameObject _player;
	GameObject _enemy;

	int _enemyHp = 0;
	PlayerData _playerData;
	PlayerData _enemyData;

	FireType _fireType;

	float _gaugeBlockSpeed;
	GameObject _gaugeBlock; // 게이지 블록

	float _goodRatio;
	float _perfectRatio;

	enum BattleStatus
	{
		GaugeMoveStart,
		GaugeMove,
		PlayerAttackStart,
		PlayerAttack,
		EnemyAttackStart,
		EnemyAttack,
		Victory,
		Defeat
	}

	BattleStatus _status = BattleStatus.GaugeMoveStart;
	bool _ending = false;

	public override bool Init()
	{
		Managers.Sound.Clear();
		Managers.Sound.Play(Sound.Bgm, "Sound_Battle");
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindObject(typeof(GameObjects));
		BindButton(typeof(Buttons));
		BindImage(typeof(Images));

		_player = GetObject((int)GameObjects.Player);
		_enemy = GetObject((int)GameObjects.Enemy);
		
		_block = GetObject((int)GameObjects.Block);
		_block.SetActive(false);

		switch ((JobTitleType)_enemyData.ID)
		{
			case JobTitleType.Daeri:
				_gaugeBlockSpeed = 1700.0f;
				break;
			case JobTitleType.Gwajang:
				_gaugeBlockSpeed = 1900.0f;
				break;
			case JobTitleType.Bujang:
				_gaugeBlockSpeed = 2100.0f;
				break;
			case JobTitleType.Esa:
				_gaugeBlockSpeed = 2300.0f;
				break;
			case JobTitleType.Sajang:
				_gaugeBlockSpeed = 2500.0f;
				break;
		}

		_gaugeBlock = GetObject((int)GameObjects.GaugeBlock);
		PatrolController pc = _gaugeBlock.GetOrAddComponent<PatrolController>();
		pc.MovingSpeed = _gaugeBlockSpeed;
		pc.SwapLookDirection = false;

		if (Managers.Data.Players.TryGetValue((int)JobTitleType.Sinib, out _playerData) == false)
			Debug.Log("Player Data not found");

		_player.GetOrAddComponent<PlayerController>().JobTitle = JobTitleType.Sinib;
		_player.GetOrAddComponent<PlayerController>().SetSkeletonAsset(_playerData.spine);
		_player.GetOrAddComponent<PlayerController>().State = Define.AnimState.Idle;
		
		_enemy.GetOrAddComponent<PlayerController>().JobTitle = (JobTitleType)_enemyData.ID;
		_enemy.GetOrAddComponent<PlayerController>().SetSkeletonAsset(_enemyData.spine);
		_enemy.GetOrAddComponent<PlayerController>().State = Define.AnimState.Idle;

		_enemyHp = _enemyData.maxhp;

		_block.SetActive(false);
		_gaugeBlock.SetActive(false);
		_gaugeBlock.GetComponent<PatrolController>().StopMove = true;

		GetButton((int)Buttons.BlockButton).gameObject.BindEvent(OnFireBlock);
		GetText((int)Texts.PlayerHpText).text = "";
		GetText((int)Texts.EnemyHpText).text = "";
		GetImage((int)Images.Bubble).gameObject.SetActive(false);

		GetObject((int)GameObjects.BlockEffect).SetActive(false);

		// 체력 리셋
		Managers.Game.Hp = Managers.Game.MaxHp;
		_status = BattleStatus.GaugeMoveStart;

		_ending = false;

		RefreshUI();
		return true;
	}

	void Update()
	{
		// 대기 시간
		if (_waitCoroutine != null)
			return;

		switch (_status)
		{
			case BattleStatus.GaugeMoveStart:
				UpdateGaugeMoveStart();
				break;
			case BattleStatus.GaugeMove:
				UpdateGaugeMove();
				break;
			case BattleStatus.PlayerAttackStart:
				UpdatePlayerAttackStart();
				break;
			case BattleStatus.PlayerAttack:
				UpdatePlayerAttack();
				break;
			case BattleStatus.EnemyAttackStart:
				UpdateEnemyAttackStart();
				break;
			case BattleStatus.EnemyAttack:
				UpdateEnemyAttack();
				break;
			case BattleStatus.Victory:
				UpdateVictory();
				break;
			case BattleStatus.Defeat:
				UpdateDefeat();
				break;
		}
	}

	void UpdateGaugeMoveStart()
	{
		// 온갖 초기화 끝내고 GaugeMove 상태로 이동
		_block.SetActive(false);
		_gaugeBlock.SetActive(true);
		_gaugeBlock.GetComponent<PatrolController>().StopMove = false;
		_status = BattleStatus.GaugeMove;
		GetText((int)Texts.PlayerHpText).gameObject.SetActive(false);

		// 벽돌 없으면 공격권이 없음
		if (Managers.Game.BlockCount == 0)
		{
			_block.SetActive(false);
			_gaugeBlock.SetActive(false);
			_gaugeBlock.GetComponent<PatrolController>().StopMove = true;
			_status = BattleStatus.EnemyAttackStart;
		}
	}

	void UpdateGaugeMove()
	{
		// OnFireBlock 트리거하면 종료된다.		
	}

	void UpdatePlayerAttackStart()
	{
		// 온갖 초기화 끝내고 PlayerAttack 상태로 이동
		_block.SetActive(true);
		_block.transform.position = GetObject((int)GameObjects.BlockStart).transform.position;
		_gaugeBlock.SetActive(false);
		_gaugeBlock.GetComponent<PatrolController>().StopMove = true;
		_player.GetOrAddComponent<PlayerController>().State = Define.AnimState.Attack;
		_status = BattleStatus.PlayerAttack;
	}

	const float EPSILON = 50.0f;

	void UpdatePlayerAttack()
	{
		Vector3 dir = (GetObject((int)GameObjects.BlockDest).transform.position - _block.transform.position);

		// 목표 지점에 도착
		if (dir.magnitude < EPSILON)
		{
			_block.SetActive(false);
			Managers.Sound.Play(Sound.Effect, ("Sound_EnemyAttacked"));
			//GetText((int)Texts.EnemyHpText).gameObject.GetComponent<DOTweenAnimation>().DORestartAllById("EnemyHp");
			//GetObject((int)GameObjects.JitterScreen).GetOrAddComponent<DOTweenAnimation>().DORestartAllById("Jitter");

			GetObject((int)GameObjects.BlockEffect).SetActive(true);
			GetObject((int)GameObjects.BlockEffect).GetOrAddComponent<BaseController>().Refresh();

			int damage = Managers.Game.Attack;
			switch (_fireType)
			{
				case FireType.Normal:
					damage = (int)(damage * 0.7);
					break;
				case FireType.Good:
					damage = (int)(damage * 1.0);
					break;
				case FireType.Perfect:
					damage = (int)(damage * 1.2);
					break;
			}
		
			_enemyHp = Mathf.Max(_enemyHp - damage, 0);
			GetText((int)Texts.EnemyHpText).text = $"-{damage}";
			GetText((int)Texts.EnemyHpText).gameObject.SetActive(true);

			// 몬스터가 죽었을 때
			if (_enemyHp <= 0)
			{
				//GetObject((int)GameObjects.Enemy).GetOrAddComponent<DOTweenAnimation>().DORestartAllById("EnemyDied");
				_status = BattleStatus.Victory;
				_waitCoroutine = StartCoroutine(CoWait(2.0f));
				return;
			}

			// 적군 턴으로 넘긴다
			_status = BattleStatus.EnemyAttackStart;
			_waitCoroutine = StartCoroutine(CoWait(1.0f));
			return;
		}

		// 벽돌 이동
		_block.transform.position += dir.normalized * Math.Min(dir.magnitude, _blockSpeed * Time.deltaTime);
	}
	
	void UpdateEnemyAttackStart()
	{
		_enemy.GetOrAddComponent<PlayerController>().State = AnimState.Attack;
		int randId = _enemyData.attackTexts.GetRandom<int>();
		string attackText = Managers.GetText(randId);
		GetImage((int)Images.Bubble).gameObject.SetActive(true);
		//GetImage((int)Images.Bubble).gameObject.GetComponent<DOTweenAnimation>().DORestartAllById("Bubble");
		Managers.Sound.Play(Sound.Effect, ("Sound_Bubble"));
		GetText((int)Texts.BubbleText).text = attackText;

		GetText((int)Texts.EnemyHpText).gameObject.SetActive(false);

		_status = BattleStatus.EnemyAttack;
		_waitCoroutine = StartCoroutine(CoWait(1.0f));

	}

	void UpdateEnemyAttack()
	{
		Managers.Sound.Play(Sound.Effect, ("Sound_PlayerAttacked"));
		Managers.Game.Hp -= _enemyData.atk;
		GetImage((int)Images.Bubble).gameObject.SetActive(false);
		GetText((int)Texts.PlayerHpText).text = $"-{_enemyData.atk}";
		GetText((int)Texts.PlayerHpText).gameObject.SetActive(true);
		//GetText((int)Texts.PlayerHpText).gameObject.GetComponent<DOTweenAnimation>().DORestartAllById("PlayerHp");
		

		_status = BattleStatus.GaugeMoveStart;
		_waitCoroutine = StartCoroutine(CoWait(1.2f));

		// 죽었으면 끝
		if (Managers.Game.Hp <= 0)
		{
			_status = BattleStatus.Defeat;
			//GetObject((int)GameObjects.Enemy).GetOrAddComponent<DOTweenAnimation>().DORestartAllById("PlayerDied");
			
		}
	}

	void UpdateVictory()
	{
		// 승리
		Debug.Log("Battle Won!");
		Managers.UI.ClosePopupUI(this);
		UI_ResultPopup popup = Managers.UI.ShowPopupUI<UI_ResultPopup>();
		var rewards = new List<RewardValuePair>() { new RewardValuePair() { type = RewardType.Promotion, value = _enemyData.ID } };
		popup.SetInfo(ResultType.Victory, rewards, path: _enemyData.promotion, text: "");

		// 컬렉션 확인
		int diff = _enemyData.ID - _playerData.ID;
		Managers.Game.RefreshBattleCollections(diff);
		Managers.Sound.Clear();
		Managers.Sound.Play(Sound.Bgm, "Sound_MainPlayBGM", volume: 0.2f);
	}

	void UpdateDefeat()
	{
		if (_ending)
			return;

		_ending = true;

		// 패배
		// 광고 띄우기
		if (Managers.IAP.IsNoAds == false)
			Managers.Ads.ShowInterstitialAds();

		Debug.Log("Battle Lost");
		Managers.UI.ClosePopupUI();
		UI_ResultPopup popup = Managers.UI.ShowPopupUI<UI_ResultPopup>();
		popup.SetInfo(ResultType.Defeat, new List<RewardValuePair>(), path: "", text: "");
		Managers.Sound.Clear();
		Managers.Sound.Play(Sound.Bgm, "Sound_MainPlayBGM", volume: 0.2f);
	}

	void OnFireBlock()
	{
		Debug.Log("OnFireBlock");

		if (_status != BattleStatus.GaugeMove)
			return;
		if (Managers.Game.BlockCount == 0)
			return;

		Managers.Game.BlockCount--;
		RefreshUI();

		// 점수 계산
		float width = GetImage((int)Images.BlockBar).GetComponent<RectTransform>().rect.width / 2;
		float x = _gaugeBlock.GetComponent<RectTransform>().anchoredPosition.x;
		float ratio = Math.Abs(x / width);

		if (ratio < _perfectRatio)
			_fireType = FireType.Perfect;
		else if (ratio < _goodRatio)
			_fireType = FireType.Good;
		else
			_fireType = FireType.Normal;

		Debug.Log($"Ratio : {ratio}");
		Debug.Log($"Ratio : {_fireType}");

		Managers.Sound.Play(Sound.Effect, ("Sound_AttackButton"));
		_status = BattleStatus.PlayerAttackStart;
	}

	IEnumerator CoWait(float seconds)
	{
		if (_waitCoroutine != null)
			StopCoroutine(_waitCoroutine);

		yield return new WaitForSeconds(seconds);
		_waitCoroutine = null;
	}

	public void SetInfo(PlayerData enemyData, float goodRatio, float perfectRatio)
	{
		_goodRatio = goodRatio;
		_perfectRatio = perfectRatio;
		_enemyData = enemyData;
		
		RefreshUI();
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		GetText((int)Texts.BlockText).text = $"{Managers.Game.BlockCount}";
	}

	void OnDestroy()
	{
		if (_waitCoroutine != null)
		{
			StopCoroutine(_waitCoroutine);
			_waitCoroutine = null;
		}			
	}
}
