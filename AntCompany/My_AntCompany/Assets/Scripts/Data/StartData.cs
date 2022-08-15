using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class StartData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public int maxHp;
	[XmlAttribute]
	public string maxhpIconPath;
	[XmlAttribute]
	public int atk;
	[XmlAttribute]
	public int money;
	[XmlAttribute]
	public string moneyIconPath;
	[XmlAttribute]
	public int block;
	[XmlAttribute]
	public string blockIconPath;
	[XmlAttribute]
	public int salary;
	[XmlAttribute]
	public float salaryPercent;
	[XmlAttribute]
	public float revenuePercent;
	[XmlAttribute]
	public float cooltimePercent;
	[XmlAttribute]
	public float successPercent;
	[XmlAttribute]
	public int workAbility;
	[XmlAttribute]
	public string workAbilityIconPath;
	[XmlAttribute]
	public int likeAbility;
	[XmlAttribute]
	public string likeAbilityIconPath;
	[XmlAttribute]
	public int stress;
	[XmlAttribute]
	public int increaseStress;
	[XmlAttribute]
	public int maxStress;
	[XmlAttribute]
	public string stressIconPath;
	[XmlAttribute]
	public int luck;
	[XmlAttribute]
	public string luckIconPath;


}