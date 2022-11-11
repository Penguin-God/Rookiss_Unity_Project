using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using static Define;

[Serializable]
public class TextData
{
	public string TextID;
	public string Kor;
	public string Eng;
}

[Serializable]
public class TextDataLoader : ILoader<string, TextData>
{
	public List<TextData> texts = new List<TextData>();

	public Dictionary<string, TextData> MakeDic()
	{
		Dictionary<string, TextData> dic = new Dictionary<string, TextData>();

		foreach (TextData text in texts)
			dic.Add(text.TextID, text);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}

[Serializable]
public class MonsterData
{
	public int TemplateID;
	public string NameID;
	public string Prefab;
	public string SpriteID;
	public int Hp;
	public int Damage;
	public int MoveTurn;
	public int MoveSpeed;
	public int AttackRange;
	public int DropCoin;
	public int SpecialAbility;
}

[Serializable]
public class MonsterDataLoader : ILoader<int, MonsterData>
{
	public List<MonsterData> monsters = new List<MonsterData>();

	public Dictionary<int, MonsterData> MakeDic()
	{
		Dictionary<int, MonsterData> dic = new Dictionary<int, MonsterData>();

		foreach (MonsterData monster in monsters)
			dic.Add(monster.TemplateID, monster);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}

[Serializable]
public class WeaponData
{
	public int TemplateID;
    public WeaponRangeType RangeType;
    public string NameID;
	public string Sprite;
	public string RadarID;
	public int TableID;
	public KnockbackDirection KnockbackDirection;
	public float GachaRate;
	public WeaponAttackType AttackType;
	public string ObjectID;
	public List<WeaponLevelData> weaponLevelData = new List<WeaponLevelData>();
}

[Serializable]
public class WeaponLevelData
{
	public int Level;
	public int Damage;
	public int Exp;
	public int Cost;
}


[Serializable]
public class WeaponDataLoader : ILoader<int, WeaponData>
{
	public List<WeaponData> weapons = new List<WeaponData>();

	public Dictionary<int, WeaponData> MakeDic()
	{
		Dictionary<int, WeaponData> dic = new Dictionary<int, WeaponData>();

		foreach (WeaponData weapon in weapons)
			dic.Add(weapon.TemplateID, weapon);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}

[Serializable]
public class StageData
{
	public int TemplateID;
	public int ChapterID;
	public int StageID;
	public int ClearGoldReward;
	public int BossTemplateID;
	public int Turn;
	public string MapName;
	public List<RespawnData> respawnData = new List<RespawnData>();
}
[Serializable]
public class StageDataLoader : ILoader<int, StageData>
{
	public List<StageData> stages = new List<StageData>();
	public Dictionary<int, StageData> MakeDic()
	{
		Dictionary<int, StageData> dic = new Dictionary<int, StageData>();

		foreach (StageData stage in stages)
			dic.Add(stage.TemplateID, stage);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}

[Serializable]
public class RespawnData
{
	public int MonsterID;
	public int SummonPoint;
	public int MinPoint;
	public int MaxPoint;
	public int ClearCount;
}

[Serializable]
public class BossData
{
    public int TemplateID;
    public string NameID;
    public string Prefab;
    public string SpriteID;
    public int Hp;
    public int Damage;
    public int MoveTurn;
    public int MoveSpeed;
    public int AttackRange;
    public int DropCoin;
	public float UseSkillAnimationDuration;
	public float PositionY;
	public List<BossSkillData> bossSkillData = new List<BossSkillData>();
}

[Serializable]
public class BossDataLoader : ILoader<int, BossData>
{
	public List<BossData> bosses = new List<BossData>();
	public Dictionary<int, BossData> MakeDic()
    {
		Dictionary<int, BossData> dic = new Dictionary<int, BossData>();

		foreach (BossData boss in bosses)
			dic.Add(boss.TemplateID, boss);

		return dic;
    }

	public bool Validate()
    {
		return true;
    }
}

[Serializable]
public class BossSkillData
{
	public int BossID;
	public SkillType SkillID;
	public int Cooltime;
	public int MonsterCount;
	public int Value;
	public int Turn;
}

public enum SkillType
{
    ControllRadar,
	SummonMonster,
	ReduceTurn,
	LockWeapon,
}

[Serializable]
public class ChapterResourceData
{
	public int TemplateID;
	public string MapTop;
	public string MapBottom;
	public string MapCenter;
	public string Object1;
	public string Object2;
	public string Object3;
	public string Dot;
	public string StageBlock;
	public string StageTileSpine;
	public string Shadow;
}

[Serializable]
public class ChapterResourceDataLoader : ILoader<int, ChapterResourceData>
{
    public List<ChapterResourceData> chapters = new List<ChapterResourceData>();
    public Dictionary<int, ChapterResourceData> MakeDic()
    {
        Dictionary<int, ChapterResourceData> dic = new Dictionary<int, ChapterResourceData>();

        foreach (ChapterResourceData stage in chapters)
            dic.Add(stage.TemplateID, stage);

        return dic;
    }

    public bool Validate()
    {
        return true;
    }
}

[Serializable]
public class QuestData
{
	public int TemplateID;
	public QuestType QuestID;
	public int QuestCount;
	public string RewardType;
	public int RewardCount;
}

public enum QuestType
{ 
	
}

[Serializable]
public class QuestDataLoader : ILoader<int, QuestData>
{
	public List<QuestData> quests = new List<QuestData>();
	public Dictionary<int, QuestData> MakeDic()
	{
		Dictionary<int, QuestData> dic = new Dictionary<int, QuestData>();

		foreach (QuestData quest in quests)
			dic.Add(quest.TemplateID, quest);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}