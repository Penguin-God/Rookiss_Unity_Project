using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Define;

[Serializable]
public class PlayerState
{
	public AnimState state = AnimState.None;
	public bool dialogueEvent = false;
	public bool goHomeEvent = false;
}

public enum CollectionState
{
	None,
	Uncheck,
	Done
}

[Serializable]
public class GameData
{
	public string Name;
	public JobTitleType JobTitle;

	public int Hp;
	public int MaxHp;
	public int WorkAbility;
	public int Likeability;
	public int Luck;
	public int Stress;
	public int MaxStress;

	public int BlockCount;
	public int Money;
	public int Salary;

	public float PlayTime;
	public int LastStressIncreaseDay;
	public int LastHpDecreaseDay;
	public int LastPayDay;
	public int NextGoHomeTime;
	public int NextDialogueDay;
	public int LastSalaryNegotiationDay;
	public float LastProjectTime;
	public float LastProjectCoolTime;

	public CollectionState[] Collections = new CollectionState[MAX_COLLECTION_COUNT];
	public PlayerState[] Players = new PlayerState[JOB_TITLE_TYPE_COUNT + 1];

	// 프로젝트 완료 횟수
	public int[] Projects = new int[MAX_PROJECT_COUNT];

	// 클리어 한 엔딩
	public CollectionState[] Endings = new CollectionState[MAX_ENDING_COUNT];

	public float NextProjectTime { get { return LastProjectTime + LastProjectCoolTime; } }
}

public class GameManagerEx
{
	GameData _gameData = new GameData();
	public GameData SaveData { get { return _gameData; } set { _gameData = value; } }

	#region 스탯
	public string Name
	{
		get { return _gameData.Name; }
		set { _gameData.Name = value; }
	}

	public JobTitleType JobTitle
	{
		get { return _gameData.JobTitle; }
		set { _gameData.JobTitle = value; RefreshLevelCollections(); }
	}

	public int Hp
	{
		get { return _gameData.Hp; }
		set { _gameData.Hp = Mathf.Clamp(value, 0, MaxHp); }
	}

	public int MaxHp
	{
		get { return _gameData.MaxHp; }
		set { _gameData.MaxHp = value; RefreshStatCollections(); }
	}

	public int WorkAbility
	{
		get { return _gameData.WorkAbility; }
		set { _gameData.WorkAbility = value; RefreshStatCollections(); }
	}

	public int Likeability
	{
		get { return _gameData.Likeability; }
		set { _gameData.Likeability = value; RefreshStatCollections(); }
	}

	public int Luck
	{
		get { return _gameData.Luck; }
		set { _gameData.Luck = value; RefreshStatCollections(); }
	}

	public Action OnStressChanged;
	public int Stress
	{
		get { return _gameData.Stress; }
		set { _gameData.Stress = Mathf.Clamp(value, 0, MaxStress); RefreshStatCollections(); OnStressChanged?.Invoke(); }
	}

	public int MaxStress
	{
		get { return _gameData.MaxStress; }
		set { _gameData.MaxStress = value; }
	}

	public float HpPercent { get { return Hp * 100 / (float)MaxHp; } }
	public float StressPercent { get { return Stress * 100 / (float)MaxStress; } }

	public int Attack
	{
		get
		{
			int attack = Managers.Data.Start.atk;
			foreach (StatData statData in Managers.Data.Stats.Values)
				attack += GetStat(statData.type) * statData.difAtk;

			float incPercent = 0.0f;
			foreach (StatData statData in Managers.Data.Stats.Values)
				incPercent += GetStat(statData.type) * statData.difAllAtkPercent;

			attack += (int)(attack * incPercent);
			if (attack < 0)
				attack = 0;

			return attack;
		}
	}

	public float SalaryAdditionalIncreasePercent
	{
		get
		{
			float incPercent = 0.0f;
			foreach (StatData statData in Managers.Data.Stats.Values)
				incPercent += GetStat(statData.type) * statData.difSalaryPercent;

			return incPercent;
		}
	}

	public float AdditionalRevenuePercent
	{
		get
		{
			float incPercent = 0.0f;
			foreach (StatData statData in Managers.Data.Stats.Values)
				incPercent += GetStat(statData.type) * statData.difRevenuePercent;

			return incPercent;
		}
	}

	public float BlockHitSucessPercent
	{
		get
		{
			float percent = Managers.Data.Start.successPercent;

			float incPercent = 0.0f;
			foreach (StatData statData in Managers.Data.Stats.Values)
				incPercent += GetStat(statData.type) * statData.successPercent;

			return percent + incPercent;
		}
	}

	public float ProjectCoolTimePercent
	{
		get
		{
			float percent = Managers.Data.Start.cooltimePercent;

			float incPercent = 0.0f;
			foreach (StatData statData in Managers.Data.Stats.Values)
				incPercent += GetStat(statData.type) * statData.cooltimePercent;

			return percent + incPercent;
		}
	}

	public int GetStat(StatType type)
	{
		switch (type)
		{
			case StatType.MaxHp:
				return MaxHp;
			case StatType.WorkAbility:
				return WorkAbility;
			case StatType.Likeability:
				return Likeability;
			case StatType.Luck:
				return Luck;
			case StatType.Stress:
				return Stress;
		}

		return 0;
	}

	public PlayerState GetPlayerState(JobTitleType type)
	{
		return _gameData.Players[(int)type];
	}
	#endregion

	#region 재화
	public int BlockCount
	{
		get { return _gameData.BlockCount; }
		set { _gameData.BlockCount = value; RefreshWealthCollections(); }
	}

	public int Money
	{
		get { return _gameData.Money; }
		set { _gameData.Money = value; RefreshWealthCollections(); }
	}

	public int Salary
	{
		get { return _gameData.Salary; }
		set { _gameData.Salary = value; RefreshWealthCollections(); }
	}
	#endregion

	#region 시간

	public float PlayTime
	{
		get { return _gameData.PlayTime; }
		set { _gameData.PlayTime = value; }
	}

	public int LastHpDecreaseDay
	{
		get { return _gameData.LastHpDecreaseDay; }
		set { _gameData.LastHpDecreaseDay = value; }
	}

	public int LastStressIncreaseDay
	{
		get { return _gameData.LastStressIncreaseDay; }
		set { _gameData.LastStressIncreaseDay = value; }
	}

	public int LastPayDay
	{
		get { return _gameData.LastPayDay; }
		set { _gameData.LastPayDay = value; }
	}

	public int NextGoHomeTime
	{
		get { return _gameData.NextGoHomeTime; }
		set { _gameData.NextGoHomeTime = value; }
	}

	public int NextDialogueDay
	{
		get { return _gameData.NextDialogueDay; }
		set { _gameData.NextDialogueDay = value; }
	}

	public int LastSalaryNegotiationDay
	{
		get { return _gameData.LastSalaryNegotiationDay; }
		set { _gameData.LastSalaryNegotiationDay = value; }
	}

	public int NextSalaryNegotiationDay { get { return LastSalaryNegotiationDay + 120; } }

	public float LastProjectTime
	{
		get { return _gameData.LastProjectTime; }
		set { _gameData.LastProjectTime = value; }
	}

	public float LastProjectCoolTime
	{
		get { return _gameData.LastProjectCoolTime; }
		set { _gameData.LastProjectCoolTime = value; }
	}

	public float NextProjectTime { get { return LastProjectTime + LastProjectCoolTime; } }

	public int MaxGameDays { get; set; }
	public float SecondPerGameDay { get; set; }

	public int GameDay
	{
		get
		{
			int gameDays = (int)(PlayTime / SecondPerGameDay);
			return Mathf.Min(gameDays, MaxGameDays);
		}
	}

	public int NextPayDay { get { return LastPayDay + 30; } }

	public void CalcNextDialogueDay()
	{
		int randValue = UnityEngine.Random.Range(0, 14);
		NextDialogueDay = GameDay + randValue;
	}

	#endregion

	#region 컬렉션 & 프로젝트
	public CollectionState[] Collections { get { return _gameData.Collections; } }

	public int[] Projects { get { return _gameData.Projects; } }
	public CollectionState[] Endings { get { return _gameData.Endings; } }

	public Action<CollectionData> OnNewCollection;

	public void RefreshStatCollections()
	{
		foreach (CollectionData data in Managers.Data.StatCollections)
		{
			if (Collections[data.ID - 1] != CollectionState.None)
				continue;

			if (data.reqMaxHp > MaxHp)
				continue;
			if (data.reqWorkAbility > WorkAbility)
				continue;
			if (data.reqLikability > Likeability)
				continue;
			if (data.reqLuck > Luck)
				continue;
			if (data.reqStress > Stress)
				continue;

			Collections[data.ID - 1] = CollectionState.Uncheck;
			Debug.Log($"Collection Clear : {data.ID}");

			MaxHp += data.difMaxHp;
			WorkAbility += data.difWorkAbility;
			Likeability += data.difLikability;
			Luck += data.difLuck;

			OnNewCollection?.Invoke(data);
		}
	}

	public void RefreshWealthCollections()
	{
		foreach (CollectionData data in Managers.Data.WealthCollections)
		{
			CollectionState state = Collections[data.ID - 1];
			if (state != CollectionState.None)
				continue;

			if (data.reqMoney > Money)
				continue;
			if (data.reqBlock > BlockCount)
				continue;
			if (data.reqSalary > Salary)
				continue;

			Collections[data.ID - 1] = CollectionState.Uncheck;
			Debug.Log($"Collection Clear : {data.ID}");

			MaxHp += data.difMaxHp;
			WorkAbility += data.difWorkAbility;
			Likeability += data.difLikability;
			Luck += data.difLuck;

			OnNewCollection?.Invoke(data);
		}
	}

	public void RefreshLevelCollections()
	{
		foreach (CollectionData data in Managers.Data.LevelCollections)
		{
			CollectionState state = Collections[data.ID - 1];
			if (state != CollectionState.None)
				continue;

			if (data.reqLevel > (int)JobTitle)
				continue;

			Collections[data.ID - 1] = CollectionState.Uncheck;
			Debug.Log($"Collection Clear : {data.ID}");

			MaxHp += data.difMaxHp;
			WorkAbility += data.difWorkAbility;
			Likeability += data.difLikability;
			Luck += data.difLuck;

			OnNewCollection?.Invoke(data);
		}
	}

	public void RefreshProjectCollections()
	{
		foreach (CollectionData data in Managers.Data.ProjectCollections)
		{
			CollectionState state = Collections[data.ID - 1];
			if (state != CollectionState.None)
				continue;

			int clearCount = Projects[data.projectID - 1];
			if (data.reqCount > clearCount)
				continue;

			Collections[data.ID - 1] = CollectionState.Uncheck;
			Debug.Log($"Collection Clear : {data.ID}");

			MaxHp += data.difMaxHp;
			WorkAbility += data.difWorkAbility;
			Likeability += data.difLikability;
			Luck += data.difLuck;

			OnNewCollection?.Invoke(data);
		}
	}

	public void RefreshBattleCollections(int levelDiff)
	{
		CollectionData data = Managers.Data.BattleCollections.Where(c => { return c.leveldif == levelDiff; }).FirstOrDefault();
		if (data == null)
			return;

		CollectionState state = Collections[data.ID - 1];
		if (state != CollectionState.None)
			return;

		Collections[data.ID - 1] = CollectionState.Uncheck;
		Debug.Log($"Collection Clear : {data.ID}");

		MaxHp += data.difMaxHp;
		WorkAbility += data.difWorkAbility;
		Likeability += data.difLikability;
		Luck += data.difLuck;

		OnNewCollection?.Invoke(data);
	}

	void ReApplyCollectionStats()
	{
		foreach (CollectionData data in Managers.Data.Collections.Values)
		{
			CollectionState state = Collections[data.ID - 1];
			if (state == CollectionState.None)
				continue;

			Debug.Log($"Apply Collection : {data.ID}");
			MaxHp += data.difMaxHp;
			WorkAbility += data.difWorkAbility;
			Likeability += data.difLikability;
			Luck += data.difLuck;

			OnNewCollection?.Invoke(data);
		}
	}
	#endregion

	public void Init()
	{
		StartData data = Managers.Data.Start;

		if (File.Exists(_path))
		{
			string fileStr = File.ReadAllText(_path);
			_gameData.Collections = JsonUtility.FromJson<GameData>(fileStr).Collections;
		}

		if (_gameData.Collections == null || _gameData.Collections.Length == 0)
			_gameData.Collections = new CollectionState[MAX_COLLECTION_COUNT];

		_gameData.Players = new PlayerState[JOB_TITLE_TYPE_COUNT + 1];
		for (int i = 0; i < _gameData.Players.Length; i++)
			_gameData.Players[i] = new PlayerState();

		Name = "NoName";
		JobTitle = JobTitleType.Sinib;

		MaxHp = data.maxHp;
		Hp = data.maxHp;
		WorkAbility = data.workAbility;
		Likeability = data.likeAbility;
		Luck = data.luck;
		MaxStress = data.maxStress;
		Stress = data.stress;

		BlockCount = data.block;
		Money = data.money;
		Salary = data.salary;

		PlayTime = 0.0f;
		MaxGameDays = 120 * 15;
		SecondPerGameDay = (float)1.0;
		LastStressIncreaseDay = 0;
		LastHpDecreaseDay = 0;
		LastPayDay = 0;
		NextGoHomeTime = 0;
		NextDialogueDay = 0;
		LastProjectTime = 0;
		LastProjectCoolTime = 0;
		LastSalaryNegotiationDay = 0;

		// 컬렉션 수치 적용
		ReApplyCollectionStats();

		Hp = MaxHp;
	}

	#region Save & Load	
	public string _path = Application.persistentDataPath + "/SaveData.json";

	public void SaveGame()
	{
		string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
		File.WriteAllText(_path, jsonStr);
		Debug.Log($"Save Game Completed : {_path}");
	}

	public bool LoadGame()
	{
		if (File.Exists(_path) == false)
			return false;

		string fileStr = File.ReadAllText(_path);
		GameData data = JsonUtility.FromJson<GameData>(fileStr);
		if (data != null)
		{
			Managers.Game.SaveData = data;
		}

		Debug.Log($"Save Game Loaded : {_path}");
		return true;
	}
	#endregion
}
