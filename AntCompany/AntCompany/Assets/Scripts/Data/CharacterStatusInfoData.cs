using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class CharacterStatusInfoData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public int textID;


	// ...
}

[Serializable, XmlRoot("ArrayOfCharacterStatusInfoData")]
public class CharacterStatusInfoDataLoader : ILoader<int, CharacterStatusInfoData>
{
	[XmlElement("CharacterStatusInfoData")]
	public List<CharacterStatusInfoData> _characterStatusInfoData = new List<CharacterStatusInfoData>();

	public Dictionary<int, CharacterStatusInfoData> MakeDic()
	{
		Dictionary<int, CharacterStatusInfoData> dic = new Dictionary<int, CharacterStatusInfoData>();

		foreach (CharacterStatusInfoData data in _characterStatusInfoData)
			dic.Add(data.ID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}
