using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using System.Linq;
using UnityEditor.AddressableAssets;
using Newtonsoft.Json;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
	[MenuItem("Tools/DeleteGameData %#G")]
	public static void DeleteGameData()
	{
		string path = Application.persistentDataPath + "/SaveData.json";
		if (File.Exists(path))
			File.Delete(path);
	}

	[MenuItem("Tools/ParseExcel %#K")]
	public static void ParseExcel()
	{
		ParseTextData("Text");
		ParseWeaponData("Weapon");
		ParseMonsterData("Monster");
		ParseStageData("Stage");
		ParseBossData("Boss");
		ParseChapterResourceData("ChapterResource");
	}

	static void ParseTextData(string filename)
	{
		TextDataLoader loader = new TextDataLoader();

		#region ExcelData
		string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

		for (int y = 1; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			int i = 0;

			loader.texts.Add(new TextData()
			{
				TextID = row[i++],
				Kor = row[i++],
				Eng = row[i++]
			});
		}
		#endregion

		string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
		File.WriteAllText($"{Application.dataPath}/@Resources/Data/Resources/{filename}Data.json", jsonStr);
		AssetDatabase.Refresh();
	}

	static void ParseMonsterData(string filename)
	{
		MonsterDataLoader loader = new MonsterDataLoader();

		#region ExcelData
		string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

		for (int y = 1; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			int i = 0;

			loader.monsters.Add(new MonsterData()
			{
				TemplateID = int.Parse(row[i++]),
				NameID = row[i++],
				Prefab = row[i++],
				SpriteID = row[i++],
				Hp = int.Parse(row[i++]),
				Damage = int.Parse(row[i++]),
				MoveTurn = int.Parse(row[i++]),
				MoveSpeed = int.Parse(row[i++]),
				AttackRange = int.Parse(row[i++]),
				DropCoin = int.Parse(row[i++]),
				SpecialAbility = int.Parse(row[i++]),
			});
		}
		#endregion

		string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
		File.WriteAllText($"{Application.dataPath}/@Resources/Data/Resources/{filename}Data.json", jsonStr);
		AssetDatabase.Refresh();
	}

	static void ParseWeaponData(string filename)
	{
		Dictionary<int, List<WeaponLevelData>> weaponTable = ParseWeaponLevelData("WeaponLevel");
		WeaponDataLoader loader = new WeaponDataLoader();

		#region ExcelData
		string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

		for (int y = 1; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			int i = 0;

			WeaponData weaponData = new WeaponData()
			{
                TemplateID = int.Parse(row[i++]),
                RangeType = (Define.WeaponRangeType)Enum.Parse(typeof(Define.WeaponRangeType),row[i++]),
                NameID = row[i++],
                Sprite = row[i++],
                RadarID = row[i++],
                TableID = int.Parse(row[i++]),
                KnockbackDirection = (Define.KnockbackDirection)Enum.Parse(typeof(Define.KnockbackDirection),row[i++]),
				GachaRate = float.Parse(row[i++]),
				AttackType = (Define.WeaponAttackType)Enum.Parse(typeof(Define.WeaponAttackType), row[i++]),
				ObjectID = row[i++],
            };

            if (weaponTable.TryGetValue(weaponData.TableID, out List<WeaponLevelData> weaponLevel))
                weaponData.weaponLevelData.AddRange(weaponLevel);

            loader.weapons.Add(weaponData);

        }
		#endregion

		string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
		File.WriteAllText($"{Application.dataPath}/@Resources/Data/Resources/{filename}Data.json", jsonStr);
		AssetDatabase.Refresh();
	}

    static Dictionary<int, List<WeaponLevelData>> ParseWeaponLevelData(string filename)
    {
        Dictionary<int, List<WeaponLevelData>> weaponTable = new Dictionary<int, List<WeaponLevelData>>();

        #region ExcelData
        string[] lines = File.ReadAllText($"{ Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;

            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            int tableID = int.Parse(row[i++]);
            WeaponLevelData weaponLevelData = new WeaponLevelData()
            {
                Level = int.Parse(row[i++]),
                Damage = int.Parse(row[i++]),
                Exp = int.Parse(row[i++]),
                Cost = int.Parse(row[i++]),
            };

            if (weaponTable.ContainsKey(tableID) == false)
                weaponTable.Add(tableID, new List<WeaponLevelData>());

            weaponTable[tableID].Add(weaponLevelData);
        }
        #endregion

        return weaponTable;
    }

    static void ParseStageData(string filename)
	{
		Dictionary<int, List<RespawnData>> respawnTable = ParseRespawnData("Respawn");
		StageDataLoader loader = new StageDataLoader();

		#region ExcelData
		string[] lines = File.ReadAllText($"{ Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

		for (int y = 1; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;

			if (string.IsNullOrEmpty(row[0]))
				continue;

			int i = 0;

			StageData stageData = new StageData()
			{
				TemplateID = int.Parse(row[i++]),
				ChapterID = int.Parse(row[i++]),
				StageID = int.Parse(row[i++]),
				ClearGoldReward = int.Parse(row[i++]),
				BossTemplateID = int.Parse(row[i++]),
				Turn = int.Parse(row[i++]),
				MapName = row[i++],
			};

			if (respawnTable.TryGetValue(stageData.TemplateID, out List<RespawnData> respawns))
				stageData.respawnData.AddRange(respawns);

			loader.stages.Add(stageData);
		}
		#endregion

		string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
		File.WriteAllText($"{Application.dataPath}/@Resources/Data/Resources/{filename}Data.json", jsonStr);
		AssetDatabase.Refresh();
	}

    static Dictionary<int, List<RespawnData>> ParseRespawnData(string filename)
	{
		Dictionary<int, List<RespawnData>> respawnTable = new Dictionary<int, List<RespawnData>>();

		#region ExcelData
		string[] lines = File.ReadAllText($"{ Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

		for (int y = 1; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;

			if (string.IsNullOrEmpty(row[0]))
				continue;

			int i = 0;
			int respawnID = int.Parse(row[i++]);
			RespawnData respawnData = new RespawnData()
			{
				MonsterID = int.Parse(row[i++]),
				SummonPoint = int.Parse(row[i++]),
				MinPoint = int.Parse(row[i++]),
				MaxPoint = int.Parse(row[i++]),
				ClearCount = int.Parse(row[i++]),
			};

			if (respawnTable.ContainsKey(respawnID) == false)
				respawnTable.Add(respawnID, new List<RespawnData>());

			respawnTable[respawnID].Add(respawnData);
		}
		#endregion

		return respawnTable;
	}

    static void ParseBossData(string filename)
    {
        Dictionary<int, List<BossSkillData>> bossSkillTable = ParseBossSkillData("BossSkill");
        BossDataLoader loader = new BossDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\n", "").Split(',');
            if (row.Length == 0)
                continue;

            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            BossData bossData = new BossData()
            {
                TemplateID = int.Parse(row[i++]),
                NameID = row[i++],
                Prefab = row[i++],
                SpriteID = row[i++],
                Hp = int.Parse(row[i++]),
                Damage = int.Parse(row[i++]),
                MoveTurn = int.Parse(row[i++]),
                MoveSpeed = int.Parse(row[i++]),
                AttackRange = int.Parse(row[i++]),
                DropCoin = int.Parse(row[i++]),
				UseSkillAnimationDuration = float.Parse(row[i++]),
				PositionY = float.Parse(row[i++]),
            };
            if (bossSkillTable.TryGetValue(bossData.TemplateID, out List<BossSkillData> skills))
                bossData.bossSkillData.AddRange(skills);

            loader.bosses.Add(bossData);
        }
        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/Resources/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }

    static Dictionary<int, List<BossSkillData>> ParseBossSkillData(string filename)
	{
		Dictionary<int, List<BossSkillData>> skillTable = new Dictionary<int, List<BossSkillData>>();

		#region ExcelData
		string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

		for (int y = 1; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;

			if (string.IsNullOrEmpty(row[0]))
				continue;

			int i = 0;
			int bossID = int.Parse(row[i++]);
			BossSkillData bossSkillData = new BossSkillData()
			{
				SkillID = (SkillType)Enum.Parse(typeof(SkillType), row[i++]),
				Cooltime = int.Parse(row[i++]),
				MonsterCount = int.Parse(row[i++]),
				Value = int.Parse(row[i++]),
				Turn = int.Parse(row[i++]),
			};

			if (skillTable.ContainsKey(bossID) == false)
				skillTable.Add(bossID, new List<BossSkillData>());

			skillTable[bossID].Add(bossSkillData);
		}
		#endregion

		return skillTable;
	}

    static void ParseChapterResourceData(string filename)
    {
		ChapterResourceDataLoader loader = new ChapterResourceDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;

			loader.chapters.Add(new ChapterResourceData()
			{
				TemplateID = int.Parse(row[i++]),
				MapTop = row[i++],
				MapBottom = row[i++],
				MapCenter = row[i++],
				Object1 = row[i++],
				Object2 = row[i++],
				Object3 = row[i++],
				Dot = row[i++],
				StageBlock = row[i++],
				StageTileSpine = row[i++],
				Shadow = row[i++],
			});
        }
        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/Resources/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }

	static void ParseQuestResourceData(string filename)
    {
		QuestDataLoader loader = new QuestDataLoader();
		#region ExcelData
		string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

		for(int y=1; y<lines.Length; y++)
        {
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			int i = 0;

			loader.quests.Add(new QuestData()
			{
				TemplateID = int.Parse(row[i++]),
				QuestID = (QuestType)Enum.Parse(typeof(QuestType), row[i++]),
				QuestCount = int.Parse(row[i++]),
				RewardType = row[i++],
				RewardCount = int.Parse(row[i++]),
			});
        }
        #endregion
    }
#endif
}