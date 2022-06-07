using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class BlockEventExcelData
{
	[XmlAttribute]
	public int enemyID;
	[XmlAttribute]
	public int answerID;
	[XmlAttribute]
	public int success;
	[XmlAttribute]
	public int resultID;
	[XmlAttribute]
	public int isDead;
	[XmlAttribute]
	public int difWorkAbility;
	[XmlAttribute]
	public int difLikability;
	[XmlAttribute]
	public int difLuck;
	[XmlAttribute]
	public int difBlock;

}

public class BlockEventData
{
	[XmlAttribute]
	public int enemyID;
	[XmlArray]
	public List<BlockEventAnsData> ansData = new List<BlockEventAnsData>();
}

public class BlockEventAnsData
{
	[XmlAttribute]
	public int enemyID;
	[XmlAttribute]
	public int answerID;
	[XmlAttribute]
	public int success;
	[XmlAttribute]
	public int resultID;
	[XmlAttribute]
	public int isDead;
	[XmlAttribute]
	public int difWorkAbility;
	[XmlAttribute]
	public int difLikability;
	[XmlAttribute]
	public int difLuck;
	[XmlAttribute]
	public int difBlock;
}

[Serializable, XmlRoot("ArrayOfBlockEventData")]
public class BlockEventDataLoader : ILoader<int, BlockEventData>
{
	[XmlElement("BlockEventData")]
	public List<BlockEventData> _blockEventData = new List<BlockEventData>();

	public Dictionary<int, BlockEventData> MakeDic()
	{
		Dictionary<int, BlockEventData> dic = new Dictionary<int, BlockEventData>();

		foreach (BlockEventData data in _blockEventData)
			dic.Add(data.enemyID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}