using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerExcelData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public int nameID;
	[XmlAttribute]
	public string illustPath;
	[XmlAttribute]
	public string battleIconPath;
	[XmlAttribute]
	public string spine;
	[XmlAttribute]
	public string aniIdle;
	[XmlAttribute]
	public string aniIdleSkin;
	[XmlAttribute]
	public string aniWorking;
	[XmlAttribute]
	public string aniWorkingSkin;
	[XmlAttribute]
	public string aniAttack;
	[XmlAttribute]
	public string aniAttackSkin;
	[XmlAttribute]
	public string aniWalk;
	[XmlAttribute]
	public string aniWalkSkin;
	[XmlAttribute]
	public string aniSweat;
	[XmlAttribute]
	public string aniSweatSkin;
	[XmlAttribute]
	public int maxhp;
	[XmlAttribute]
	public int atk;
	[XmlAttribute]
	public string promotion;
	// ...
}

public class PlayerData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public int nameID;
	[XmlAttribute]
	public string illustPath;
	[XmlAttribute]
	public string battleIconPath;
	[XmlAttribute]
	public string spine;
	[XmlAttribute]
	public string aniIdle;
	[XmlAttribute]
	public string aniIdleSkin;
	[XmlAttribute]
	public string aniWorking;
	[XmlAttribute]
	public string aniWorkingSkin;
	[XmlAttribute]
	public string aniAttack;
	[XmlAttribute]
	public string aniAttackSkin;
	[XmlAttribute]
	public string aniWalk;
	[XmlAttribute]
	public string aniWalkSkin;
	[XmlAttribute]
	public string aniSweat;
	[XmlAttribute]
	public string aniSweatSkin;
	[XmlAttribute]
	public int maxhp;
	[XmlAttribute]
	public int atk;
	[XmlArray]
	public List<int> attackTexts = new List<int>();
	[XmlAttribute]
	public string promotion;
}

[Serializable, XmlRoot("ArrayOfPlayerData")]
public class PlayerDataLoader : ILoader<int, PlayerData>
{
	[XmlElement("PlayerData")]
	public List<PlayerData> _characterDatas = new List<PlayerData>();

	public Dictionary<int, PlayerData> MakeDic()
	{
		Dictionary<int, PlayerData> dic = new Dictionary<int, PlayerData>();

		foreach (PlayerData data in _characterDatas)
			dic.Add(data.ID, data);

		return dic;
	}


	public bool Validate()
	{
		return true;
	}
}