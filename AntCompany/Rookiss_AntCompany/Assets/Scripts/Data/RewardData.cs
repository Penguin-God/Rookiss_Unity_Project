using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class RewardData
{
	[XmlAttribute]
	public string ID;
	[XmlAttribute]
	public string nameID;
	[XmlAttribute]
	public string iconPath;

	// ...
}

[Serializable, XmlRoot("ArrayOfRewardData")]
public class RewardDataLoader : ILoader<string, RewardData>
{
	[XmlElement("RewardData")]
	public List<RewardData> _rewardData = new List<RewardData>();

	public Dictionary<string, RewardData> MakeDic()
	{
		Dictionary<string, RewardData> dic = new Dictionary<string, RewardData>();

		foreach (RewardData data in _rewardData)
			dic.Add(data.ID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}
