using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class GoHomeData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public int nameID;
	[XmlAttribute]
	public string aniPath;
	[XmlAttribute]
	public int difWorkAbility;
	[XmlAttribute]
	public int difLikeability;
	[XmlAttribute]
	public int difLuck;
	[XmlAttribute]
	public int difStress;
	[XmlAttribute]
	public int difMoney;
	[XmlAttribute]
	public int textID; // 집갈 때 뜨는 메시지

	// ...
}

[Serializable, XmlRoot("ArrayOfGoHomeData")]
public class GoHomeDataLoader : ILoader<int, GoHomeData>
{
	[XmlElement("GoHomeData")]
	public List<GoHomeData> _goHomeData = new List<GoHomeData>();

	public Dictionary<int, GoHomeData> MakeDic()
	{
		Dictionary<int, GoHomeData> dic = new Dictionary<int, GoHomeData>();

		foreach (GoHomeData data in _goHomeData)
			dic.Add(data.ID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}