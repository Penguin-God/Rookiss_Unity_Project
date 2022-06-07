using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class ProjectData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public int projectName;
	[XmlAttribute]
	public string iconPath;
	[XmlAttribute]
	public string aniPath;
	[XmlAttribute]
	public int coolTime;
	[XmlAttribute]
	public int reqAbility;
	[XmlAttribute]
	public int reqLikability;
	[XmlAttribute]
	public int reqLuck;
	[XmlAttribute]
	public int difWorkAbility;
	[XmlAttribute]
	public int difLikeability;
	[XmlAttribute]
	public int difLuck;
	[XmlAttribute]
	public int difStress;
	[XmlAttribute]
	public int difBlock;
	[XmlAttribute]
	public int difMoney;


	// ...
}

[Serializable, XmlRoot("ArrayOfProjectData")]
public class ProjectDataLoader : ILoader<int, ProjectData>
{
	[XmlElement("ProjectData")]
	public List<ProjectData> _projectData = new List<ProjectData>();

	public Dictionary<int, ProjectData> MakeDic()
	{
		Dictionary<int, ProjectData> dic = new Dictionary<int, ProjectData>();

		foreach (ProjectData data in _projectData)
			dic.Add(data.ID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}