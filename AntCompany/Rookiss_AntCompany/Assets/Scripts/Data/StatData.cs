using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static Define;

public class StatData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public StatType type;
	[XmlAttribute]
	public int nameID;
	[XmlAttribute]
	public int price; // 업그레이드 비용
	[XmlAttribute]
	public int increaseStat; // 업그레이드 변화값
	[XmlAttribute]
	public int difMaxHp;
	[XmlAttribute]
	public int difAtk;
	[XmlAttribute]
	public float difAllAtkPercent;
	[XmlAttribute]
	public float difSalaryPercent;
	[XmlAttribute]
	public float difRevenuePercent;
	[XmlAttribute]
	public float cooltimePercent;
	[XmlAttribute]
	public float successPercent;
}

[Serializable, XmlRoot("ArrayOfStatData")]
public class StatDataLoader : ILoader<int, StatData>
{
	[XmlElement("StatData")]
	public List<StatData> _statData = new List<StatData>();

	public Dictionary<int, StatData> MakeDic()
	{
		Dictionary<int, StatData> dic = new Dictionary<int, StatData>();

		foreach (StatData data in _statData)
			dic.Add(data.ID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}