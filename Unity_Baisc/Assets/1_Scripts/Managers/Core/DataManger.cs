using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDic();
}

public class DataManger
{
    Dictionary<int, Data.Stat> _statByLevel = new Dictionary<int, Data.Stat>();
    IReadOnlyDictionary<int, Data.Stat> StatByLevel => _statByLevel;

    public void Init()
    {
        _statByLevel = LoadJson<Data.StatData, int, Data.Stat>("PlayerStats").MakeDic();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resources.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
