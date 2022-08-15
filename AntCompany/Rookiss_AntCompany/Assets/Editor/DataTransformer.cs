using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using static Define;
using System.Linq;

public class DataTransformer : EditorWindow
{
	[MenuItem("Tools/RemoveSaveData")]
	public static void RemoveSaveData()
	{
		string path = Application.persistentDataPath + "/SaveData.json";
		if (File.Exists(path))
		{
			File.Delete(path);
			Debug.Log("SaveFile Deleted");
		}
		else
		{
			Debug.Log("No SaveFile Detected");
		}
	}

	[MenuItem("Tools/ParseExcel")]
	public static void ParseExcel()
	{
		ParseStartData();
		ParseSalaryNegotiationData();

		ParseShopData();
		ParseTextData();
		ParseCollectionData();
		ParseEndingData();
		ParsePlayerData();
		ParseGoHomeData();		
		ParseProjectData();
		ParseDialogueEventData();
		ParseBlockEventData();
		ParseStatData();
	}

	static void ParseStartData()
	{
		StartData startData;

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/StartData").text.Split("\n");

		// 두번째 라인까지 스킵
		string[] row = lines[2].Replace("\r", "").Split(',');

		startData = new StartData()
		{
			ID = int.Parse(row[0]),
			maxHp = int.Parse(row[1]),
			maxhpIconPath = row[2],
			atk = int.Parse(row[3]),
			money = int.Parse(row[4]),
			moneyIconPath = row[5],
			block = int.Parse(row[6]),
			blockIconPath = row[7],
			salary = int.Parse(row[8]),
			salaryPercent = float.Parse(row[9]),
			revenuePercent = float.Parse(row[10]),
			cooltimePercent = float.Parse(row[11]),
			successPercent = float.Parse(row[12]),
			workAbility = int.Parse(row[13]),
			workAbilityIconPath = row[14],
			likeAbility = int.Parse(row[15]),
			likeAbilityIconPath = row[16],
			stress = int.Parse(row[17]),
			maxStress = int.Parse(row[18]),
			increaseStress = int.Parse(row[19]),			
			stressIconPath = row[20],
			luck = int.Parse(row[21]),
			luckIconPath = row[22]
		};
		#endregion

		string xmlString = ToXML(startData);
		File.WriteAllText($"{Application.dataPath}/Resources/Data/StartData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParseSalaryNegotiationData()
	{
		SalaryNegotiationData salaryNegotiationDatas = new SalaryNegotiationData();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/SalaryNegotiationData").text.Split("\n");

		// 두번째 라인까지 스킵
		string[] row = lines[2].Replace("\r", "").Split(',');

		salaryNegotiationDatas = new SalaryNegotiationData()
		{
			questionID = int.Parse(row[0]),
			yesAnswerID = int.Parse(row[1]),
			noAnswerID = int.Parse(row[2]),
			yesResultID = int.Parse(row[3]),
			noResultGoodID = int.Parse(row[4]),
			noResultBadID = int.Parse(row[5]),
			yesIncreaseSalaryPercent = int.Parse(row[6]),
			noIncreaseSalaryPercentGood = int.Parse(row[7]),
			noIncreaseSalaryPercentBad = int.Parse(row[8])
		};
		#endregion

		string xmlString = ToXML(salaryNegotiationDatas);
		File.WriteAllText($"{Application.dataPath}/Resources/Data/SalaryNegotiationData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParseShopData()
	{
		List<ShopData> shopDatas = new List<ShopData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/ShopData").text.Split("\n");

		// 첫번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			ShopData shopData = new ShopData()
			{
				ID = int.Parse(row[0]),
				name = int.Parse(row[1]),
				condition = (row[2] == "cash" ? ShopConditionType.Cash : ShopConditionType.Ads),
				price = int.Parse(row[3]),
				productID = row[4],
				rewardCount = int.Parse(row[6]),
				icon = row[7],
			};

			switch (row[5])
			{
				case "block":
					shopData.rewardType = ShopRewardType.Block;
					break;
				case "money":
					shopData.rewardType = ShopRewardType.Money;
					break;
				case "noads":
					shopData.rewardType = ShopRewardType.NoAds;
					break;
				case "luck":
					shopData.rewardType = ShopRewardType.Luck;
					break;
			}			

			shopDatas.Add(shopData);
		}
		#endregion

		string xmlString = ToXML(new ShopDataLoader() { _shopDatas = shopDatas });
		File.WriteAllText($"{Application.dataPath}/Resources/Data/ShopData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParseTextData()
	{
		List<TextData> textDatas = new List<TextData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/TextData").text.Split("\n");

		// 첫번째 라인까지 스킵
		for (int y = 1; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			textDatas.Add(new TextData()
			{
				ID = int.Parse(row[0]),
				kor = row[1],
				eng = row[2]
			});
		}
		#endregion

		string xmlString = ToXML(new TextDataLoader() { _textData = textDatas });
		File.WriteAllText($"{Application.dataPath}/Resources/Data/TextData.xml", xmlString);
		AssetDatabase.Refresh();
	}


	static void ParseCollectionData()
	{
		List<CollectionData> collectionDatas = new List<CollectionData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/CollectionData").text.Split("\n");

		// 두번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			collectionDatas.Add(new CollectionData()
			{
				ID = int.Parse(row[0]),
				nameID = int.Parse(row[1]),
				type = row[2] == "stat" ? CollectionType.Stat : (row[2] == "level" ? CollectionType.Level : CollectionType.Wealth),
				iconPath = row[3],
				reqLevel = int.Parse(row[4]),
				leveldif = int.Parse(row[5]),
				reqMaxHp = int.Parse(row[6]),
				reqWorkAbility = int.Parse(row[7]),
				reqLikability = int.Parse(row[8]),
				reqLuck = int.Parse(row[9]),
				reqStress = int.Parse(row[10]),
				reqMoney = int.Parse(row[11]),
				reqBlock = int.Parse(row[12]),
				reqSalary = int.Parse(row[13]),
				projectID = int.Parse(row[14]),
				reqCount = int.Parse(row[15]),
				difMaxHp = int.Parse(row[16]),
				difWorkAbility = int.Parse(row[17]),
				difLikability = int.Parse(row[18]),
				difLuck = int.Parse(row[19])
			});

			CollectionType type = CollectionType.Stat;
			switch (row[2])
			{
				case "stat":
					type = CollectionType.Stat;
					break;
				case "level":
					type = CollectionType.Level;
					break;
				case "riches":
					type = CollectionType.Wealth;
					break;
				case "projectCount":
					type = CollectionType.Project;
					break;
				case "battle":
					type = CollectionType.Battle;
					break;
			}

			collectionDatas.Last().type = type;
		}
		#endregion

		string xmlString = ToXML(new CollectionDataLoader() { _collectionData = collectionDatas });
		File.WriteAllText($"{Application.dataPath}/Resources/Data/CollectionData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParseEndingData()
	{
		List<EndingData> endingDatas = new List<EndingData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/EndingData").text.Split("\n");

		// 두번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			endingDatas.Add(new EndingData()
			{
				ID = int.Parse(row[0]),
				nameID = int.Parse(row[1]),
				type = (row[2] == "level" ? EndingType.Level : EndingType.Stress),
				value = int.Parse(row[3]),
				aniPath = row[4],
				illustPath = row[5]

			});
		}
		#endregion

		string xmlString = ToXML(endingDatas);
		File.WriteAllText($"{Application.dataPath}/Resources/Data/EndingData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParsePlayerData()
	{
		List<PlayerData> playerDatas = new List<PlayerData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/PlayerData").text.Split("\n");

		// 두번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			PlayerData playerData = new PlayerData()
			{
				ID = int.Parse(row[0]),
				nameID = int.Parse(row[1]),
				illustPath = row[2],
				battleIconPath = row[3],
				spine = row[4],
				aniIdle = row[5],
				aniIdleSkin = row[6],
				aniWorking = row[7],
				aniWorkingSkin = row[8],
				aniAttack = row[9],
				aniAttackSkin = row[10],
				aniWalk = row[11],
				aniWalkSkin = row[12],
				aniSweat = row[13],
				aniSweatSkin = row[14],
				maxhp = int.Parse(row[15]),
				atk = int.Parse(row[16]),
				promotion = row[20],
			};

			playerData.attackTexts.Add(int.Parse(row[17]));
			playerData.attackTexts.Add(int.Parse(row[18]));
			playerData.attackTexts.Add(int.Parse(row[19]));

			playerDatas.Add(playerData);
		}
		#endregion

		string xmlString = ToXML(playerDatas);
		File.WriteAllText($"{Application.dataPath}/Resources/Data/PlayerData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParseGoHomeData()
	{
		List<GoHomeData> goHomeDatas = new List<GoHomeData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/GoHomeData").text.Split("\n");

		// 두번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			goHomeDatas.Add(new GoHomeData()
			{
				ID = int.Parse(row[0]),
				nameID = int.Parse(row[1]),
				aniPath = row[2],
				difWorkAbility = int.Parse(row[3]),
				difLikeability = int.Parse(row[4]),
				difLuck = int.Parse(row[5]),
				difStress = int.Parse(row[6]),
				difMoney = int.Parse(row[7]),
				textID = int.Parse(row[8])
			});
		}
		#endregion

		string xmlString = ToXML(goHomeDatas);
		File.WriteAllText($"{Application.dataPath}/Resources/Data/GoHomeData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParseProjectData()
	{
		List<ProjectData> projectDatas = new List<ProjectData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/ProjectData").text.Split("\n");

		// 두번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			projectDatas.Add(new ProjectData()
			{
				ID = int.Parse(row[0]),
				projectName = int.Parse(row[1]),
				iconPath = row[2],
				aniPath = row[3],
				coolTime = int.Parse(row[4]),
				reqAbility = int.Parse(row[5]),
				reqLikability = int.Parse(row[6]),
				reqLuck = int.Parse(row[7]),
				difWorkAbility = int.Parse(row[8]),
				difLikeability = int.Parse(row[9]),
				difLuck = int.Parse(row[10]),
				difStress = int.Parse(row[11]),
				difBlock = int.Parse(row[12]),
				difMoney = int.Parse(row[13])
			});
		}
		#endregion

		string xmlString = ToXML(projectDatas);
		File.WriteAllText($"{Application.dataPath}/Resources/Data/ProjectData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParseDialogueEventData()
	{
		List<DialogueEventExcelData> dialogueEventDatas = new List<DialogueEventExcelData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/DialogueEventData").text.Split("\n");

		// 첫번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			dialogueEventDatas.Add(new DialogueEventExcelData()
			{
				questionID = int.Parse(row[0]),
				answerID = int.Parse(row[1]),
				resultID = int.Parse(row[2]),
				difWorkAbility = int.Parse(row[3]),
				difLikability = int.Parse(row[4]),
				difLuck = int.Parse(row[5]),
				difStress = int.Parse(row[6]),
				difMoney = int.Parse(row[7]),
				difBlock = int.Parse(row[8]),
				enemyType = int.Parse(row[9])
			});
		}
		#endregion

		#region 데이터 변환
		Dictionary<int, DialogueEventData> dic = new Dictionary<int, DialogueEventData>();

		DialogueEventDataLoader loader = new DialogueEventDataLoader();

		foreach (DialogueEventExcelData excelData in dialogueEventDatas)
		{
			DialogueEventData data;
			if (dic.TryGetValue(excelData.questionID, out data) == false)
			{
				data = new DialogueEventData() 
				{ 
					questionID = excelData.questionID, 
					enemyType = excelData.enemyType 
				};

				dic.Add(excelData.questionID, data);
			}

			data.answers.Add(new DialogueAnsData()
			{
				answerID = excelData.answerID,
				resultID = excelData.resultID,
				difWorkAbility = excelData.difWorkAbility,
				difLikeability = excelData.difLikability,
				difLuck = excelData.difLuck,
				difStress = excelData.difStress,
				difMoney = excelData.difMoney,
				difBlock = excelData.difBlock,
			});
		}
		#endregion

		string xmlString = ToXML(new DialogueEventDataLoader() { _dialogueEventData = dic.Values.ToList() }); ;
		File.WriteAllText($"{Application.dataPath}/Resources/Data/DialogueEventData.xml", xmlString);
		AssetDatabase.Refresh();
	}


	static void ParseBlockEventData()
	{
		List<BlockEventExcelData> blockEventExcelDatas = new List<BlockEventExcelData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/BlockEventData").text.Split("\n");

		// 두번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			blockEventExcelDatas.Add(new BlockEventExcelData()
			{
				enemyID = int.Parse(row[0]),
				answerID = int.Parse(row[1]),
				success = int.Parse(row[2]),
				resultID = int.Parse(row[3]),
				isDead = int.Parse(row[4]),
				difWorkAbility = int.Parse(row[5]),
				difLikability = int.Parse(row[6]),
				difLuck = int.Parse(row[7]),
				difBlock = int.Parse(row[8])
			});
		}
		#endregion

		#region 데이터 변환
		Dictionary<int, BlockEventData> dic = new Dictionary<int, BlockEventData>();

		foreach (BlockEventExcelData excelData in blockEventExcelDatas)
		{
			BlockEventData data;
			if (dic.TryGetValue(excelData.enemyID, out data) == false)
			{
				data = new BlockEventData()
				{
					enemyID = excelData.enemyID,
				};

				dic.Add(excelData.enemyID, data);
			}

			data.ansData.Add(new BlockEventAnsData()
			{
				answerID = excelData.answerID,
				success = excelData.success,
				resultID = excelData.resultID,
				isDead = excelData.isDead,
				difWorkAbility = excelData.difWorkAbility,
				difLikability = excelData.difLikability,
				difLuck = excelData.difLuck,
				difBlock = excelData.difBlock,
			});
		}
		#endregion

		string xmlString = ToXML(new BlockEventDataLoader() { _blockEventData = dic.Values.ToList() });
		File.WriteAllText($"{Application.dataPath}/Resources/Data/BlockEventData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParseStatData()
	{
		List<StatData> statDatas = new List<StatData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/StatData").text.Split("\n");

		// 두번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			statDatas.Add(new StatData()
			{
				ID = int.Parse(row[0]),
				type = (StatType)Enum.Parse(typeof(StatType), row[1], ignoreCase: true), // TODO
				nameID = int.Parse(row[2]),
				price = int.Parse(row[3]),
				increaseStat = int.Parse(row[4]),
				difMaxHp = int.Parse(row[5]),
				difAtk = int.Parse(row[6]),
				difAllAtkPercent = float.Parse(row[7]),
				difSalaryPercent = float.Parse(row[8]),
				difRevenuePercent = float.Parse(row[9]),
				cooltimePercent = float.Parse(row[10]),
				successPercent = float.Parse(row[11])
			});
		}
		#endregion

		string xmlString = ToXML(new StatDataLoader() { _statData = statDatas });
		File.WriteAllText($"{Application.dataPath}/Resources/Data/StatData.xml", xmlString);
		AssetDatabase.Refresh();
	}

	static void ParseRewardData()
	{
		List<RewardData> rewardDatas = new List<RewardData>();

		#region ExcelData
		string[] lines = Resources.Load<TextAsset>($"Data/Excel/RewardData").text.Split("\n");

		// 두번째 라인까지 스킵
		for (int y = 2; y < lines.Length; y++)
		{
			string[] row = lines[y].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			rewardDatas.Add(new RewardData()
			{
				ID = row[0],
				nameID = row[1],
				iconPath = row[2],

			});
		}
		#endregion

		string xmlString = ToXML(rewardDatas);
		File.WriteAllText($"{Application.dataPath}/Resources/Data/RewardData.xml", xmlString);
		AssetDatabase.Refresh();
	}



	#region XML 유틸
	public sealed class ExtentedStringWriter : StringWriter
	{
		private readonly Encoding stringWriterEncoding;

		public ExtentedStringWriter(StringBuilder builder, Encoding desiredEncoding) : base(builder)
		{
			this.stringWriterEncoding = desiredEncoding;
		}

		public override Encoding Encoding
		{
			get
			{
				return this.stringWriterEncoding;
			}
		}
	}

	public static string ToXML<T>(T obj)
	{
		using (ExtentedStringWriter stringWriter = new ExtentedStringWriter(new StringBuilder(), Encoding.UTF8))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			xmlSerializer.Serialize(stringWriter, obj);
			return stringWriter.ToString();
		}
	}
	#endregion
}
