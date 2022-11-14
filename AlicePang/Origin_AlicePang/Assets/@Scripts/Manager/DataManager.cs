using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using System.Linq;

public interface ILoader<Key, Item>
{
    Dictionary<Key, Item> MakeDic();
    bool Validate();
}

public class DataManager
{
	public Dictionary<string, TextData> Texts { get; private set; }
	public Dictionary<int, MonsterData> Monsters { get; private set; }
	public Dictionary<int, WeaponData> Weapons { get; private set; }
	//public Dictionary<int, RespawnData> Respawns { get; private set; }
	public Dictionary<int, StageData> Stages { get; private set; }
	public Dictionary<int, BossData> Bosses { get; private set; }
	public Dictionary<int, ChapterResourceData> ChapterResources { get; private set; }

	public void Init()
	{
		LoadJson<TextDataLoader, string, TextData>("TextData", (loader) => { Texts = loader.MakeDic(); });
		LoadJson<MonsterDataLoader, int, MonsterData>("MonsterData", (loader) => { Monsters = loader.MakeDic(); });
		LoadJson<WeaponDataLoader, int, WeaponData>("WeaponData", (loader) => { Weapons = loader.MakeDic(); });
		//LoadJson<RespawnDataLoader, int, RespawnData>("RespawnData", (loader) => { Respawns = loader.MakeDic(); });
		LoadJson<StageDataLoader, int, StageData>("StageData", (loader) => { Stages = loader.MakeDic(); });
		LoadJson<BossDataLoader, int, BossData>("BossData", (loader) => { Bosses = loader.MakeDic(); });
		LoadJson<ChapterResourceDataLoader, int, ChapterResourceData>("ChapterResourceData", (loader) => { ChapterResources = loader.MakeDic(); });
	}

	public bool Loaded()
	{
		if (Texts == null)
			return false;
		if (Monsters == null)
			return false;
		if (Weapons == null)
			return false;
		if (Stages == null)
			return false;
		if (Bosses == null)
			return false;
		if (ChapterResources == null)
			return false;

		return true;
	}

	void LoadSingleJson<Item>(string key, Action<Item> callback)
	{
		Managers.Resource.LoadAsync<TextAsset>(key, (textAsset) =>
		{
			//Item item = JsonConvert.DeserializeObject<Item>(textAsset.text);
			Item item = JsonUtility.FromJson<Item>(textAsset.text);
			callback?.Invoke(item);
		});
	}

	void LoadJson<Loader, Key, Value>(string key, Action<Loader> callback) where Loader : ILoader<Key, Value>
	{
		Managers.Resource.LoadAsync<TextAsset>(key, (textAsset) =>
		{
			//Loader loader = JsonConvert.DeserializeObject<Loader>(textAsset.text);
			Loader loader = JsonUtility.FromJson<Loader>(textAsset.text);
			callback?.Invoke(loader);
		});
	}

	void LoadSingleXml<Item>(string key, Action<Item> callback)
	{
		Managers.Resource.LoadAsync<TextAsset>(key, (textAsset) =>
		{
			XmlSerializer xs = new XmlSerializer(typeof(Item));
			using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
			{
				callback?.Invoke((Item)xs.Deserialize(stream));
			}
		});
	}

	void LoadXml<Loader, Key, Item>(string key, Action<Loader> callback) where Loader : ILoader<Key, Item>, new()
	{
		Managers.Resource.LoadAsync<TextAsset>(key, (textAsset) =>
		{
			XmlSerializer xs = new XmlSerializer(typeof(Loader));
			using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
			{
				callback?.Invoke((Loader)xs.Deserialize(stream));
			}
		});
	}
}
