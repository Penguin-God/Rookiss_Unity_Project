using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class DialogueEventExcelData
{
	[XmlAttribute]
	public int questionID;
	[XmlAttribute]
	public int answerID;
	[XmlAttribute]
	public int resultID;
	[XmlAttribute]
	public int difWorkAbility;
	[XmlAttribute]
	public int difLikability;
	[XmlAttribute]
	public int difLuck;
	[XmlAttribute]
	public int difStress;
	[XmlAttribute]
	public int difMoney;
	[XmlAttribute]
	public int difBlock;
	[XmlAttribute]
	public int enemyType;
}

public class DialogueEventData
{
	[XmlAttribute]
	public int questionID;
	[XmlAttribute]
	public int enemyType; // 상사한테?
	[XmlArray]
	public List<DialogueAnsData> answers = new List<DialogueAnsData>();
}

public class DialogueAnsData
{
	[XmlAttribute]
	public int answerID;
	[XmlAttribute]
	public int resultID;
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
	public int difBlock;
}

[Serializable, XmlRoot("ArrayOfDialogueEventData")]
public class DialogueEventDataLoader : ILoader<int, DialogueEventData>
{
	[XmlElement("DialogueEventData")]
	public List<DialogueEventData> _dialogueEventData = new List<DialogueEventData>();

	public Dictionary<int, DialogueEventData> MakeDic()
	{
		Dictionary<int, DialogueEventData> dic = new Dictionary<int, DialogueEventData>();

		foreach (DialogueEventData data in _dialogueEventData)
			dic.Add(data.questionID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}