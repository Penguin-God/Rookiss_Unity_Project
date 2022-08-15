using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public enum EndingType
{
	Level,
	Stress,
}

public class EndingData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public int nameID;
	[XmlAttribute]
	public EndingType type;
	[XmlAttribute]
	public int value;
	[XmlAttribute]
	public string aniPath;
	[XmlAttribute]
	public string illustPath;

	// ...
}

[Serializable, XmlRoot("ArrayOfEndingData")]
public class EndingDataLoader : ILoader<int, EndingData>
{
	[XmlElement("EndingData")]
	public List<EndingData> _endingData = new List<EndingData>();

	public Dictionary<int, EndingData> MakeDic()
	{
		Dictionary<int, EndingData> dic = new Dictionary<int, EndingData>();

		foreach (EndingData data in _endingData)
			dic.Add(data.ID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}