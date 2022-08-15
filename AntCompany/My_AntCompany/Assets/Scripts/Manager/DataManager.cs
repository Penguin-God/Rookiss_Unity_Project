using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public interface ILoader<Key, Item>
{
    Dictionary<Key, Item> MakeDic();
    bool Validate();
}

public class DataManager
{
    public StartData Start { get; private set; }
    public SalaryNegotiationData SalaryNegotiation { get; private set; }

	public Dictionary<int, ShopData> Shops { get; private set; }
    public Dictionary<int, TextData> Texts { get; private set; }
	public Dictionary<int, StatData> Stats { get; private set; }
	public Dictionary<int, PlayerData> Players { get; private set; }
	public Dictionary<int, GoHomeData> GoHomes { get; private set; }
	public Dictionary<int, ProjectData> Projects { get; private set; }
    public Dictionary<int, EndingData> Endings { get; private set; }

	public Dictionary<int, CollectionData> Collections { get; private set; }
	public List<CollectionData> StatCollections { get; private set; }
	public List<CollectionData> WealthCollections { get; private set; }
	public List<CollectionData> LevelCollections { get; private set; }
	public List<CollectionData> ProjectCollections { get; private set; }
	public List<CollectionData> BattleCollections { get; private set; }

	public Dictionary<int, DialogueEventData> Dialogues { get; private set; }
    public List<DialogueEventData> InferiorEvents { get; private set; } // 나보다 부하인 NPC가 진행하는 이벤트                                                                    
    public List<DialogueEventData> SuperiorEvents { get; private set; } // 나보다 상사인 NPC가 진행하는 이벤트

    public Dictionary<int, BlockEventData> BlockEvents { get; private set; }

	public void Init()
    {
        Start = LoadSingleXml<StartData>("StartData");
        SalaryNegotiation = LoadSingleXml<SalaryNegotiationData>("SalaryNegotiationData");

		Shops = LoadXml<ShopDataLoader, int, ShopData>("ShopData").MakeDic();
		Texts = LoadXml<TextDataLoader, int, TextData>("TextData").MakeDic();
        Stats = LoadXml<StatDataLoader, int, StatData>("StatData").MakeDic();
        Players = LoadXml<PlayerDataLoader, int, PlayerData>("PlayerData").MakeDic();
        GoHomes = LoadXml<GoHomeDataLoader, int, GoHomeData>("GoHomeData").MakeDic();
        Projects = LoadXml<ProjectDataLoader, int, ProjectData>("ProjectData").MakeDic();
        Endings = LoadXml<EndingDataLoader, int, EndingData>("EndingData").MakeDic();

		// Collection
		var collectionLoader = LoadXml<CollectionDataLoader, int, CollectionData>("CollectionData");
		StatCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Stat).ToList();
		WealthCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Wealth).ToList();
		LevelCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Level).ToList();
		ProjectCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Project).ToList();
		BattleCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Battle).ToList();

		Collections = collectionLoader.MakeDic();

		// Dialogue
		var dialogueLoader = LoadXml<DialogueEventDataLoader, int, DialogueEventData>("DialogueEventData");
        InferiorEvents = dialogueLoader._dialogueEventData.Where(e => e.enemyType == 1).ToList(); 
        SuperiorEvents = dialogueLoader._dialogueEventData.Where(e => e.enemyType == 0).ToList();        
        Dialogues = dialogueLoader.MakeDic();

        var loader = LoadXml<BlockEventDataLoader, int, BlockEventData>("BlockEventData");
        BlockEvents = loader.MakeDic();

    }

	private Item LoadSingleXml<Item>(string name)
	{
		XmlSerializer xs = new XmlSerializer(typeof(Item));
		TextAsset textAsset = Resources.Load<TextAsset>("Data/" + name);
		using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
			return (Item)xs.Deserialize(stream);
	}

	private Loader LoadXml<Loader, Key, Item>(string name) where Loader : ILoader<Key, Item>, new()
    {
        XmlSerializer xs = new XmlSerializer(typeof(Loader));
        TextAsset textAsset = Resources.Load<TextAsset>("Data/" + name);
        using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
            return (Loader)xs.Deserialize(stream);
    }
}
