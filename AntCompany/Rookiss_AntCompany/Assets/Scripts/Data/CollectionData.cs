using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public enum CollectionType
{
	Stat,
	Wealth,
	Level,
	Project,
	Battle
}

public class CollectionData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public int nameID;
	[XmlAttribute]
	public CollectionType type;
	[XmlAttribute]
	public string iconPath;
	[XmlAttribute]
	public int reqLevel;
	[XmlAttribute]
	public int leveldif;
	[XmlAttribute]
	public int reqMaxHp;
	[XmlAttribute]
	public int reqWorkAbility;
	[XmlAttribute]
	public int reqLikability;
	[XmlAttribute]
	public int reqLuck;
	[XmlAttribute]
	public int reqStress;
	[XmlAttribute]
	public int reqMoney;
	[XmlAttribute]
	public int reqBlock;
	[XmlAttribute]
	public int reqSalary;
	[XmlAttribute]
	public int projectID;
	[XmlAttribute]
	public int reqCount;
	[XmlAttribute]
	public int difMaxHp;
	[XmlAttribute]
	public int difWorkAbility;
	[XmlAttribute]
	public int difLikability;
	[XmlAttribute]
	public int difLuck;


	// ...
}

[Serializable, XmlRoot("ArrayOfCollectionData")]
public class CollectionDataLoader : ILoader<int, CollectionData>
{
	[XmlElement("CollectionData")]
	public List<CollectionData> _collectionData = new List<CollectionData>();

	public Dictionary<int, CollectionData> MakeDic()
	{
		Dictionary<int, CollectionData> dic = new Dictionary<int, CollectionData>();

		foreach (CollectionData data in _collectionData)
			dic.Add(data.ID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}