using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager
{
    GameObject _player;
    public GameObject Player => _player;

    HashSet<GameObject> _monsters = new HashSet<GameObject>();
    public event Action<int> OnSpawn = null;

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resources.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Player:
                _player = go; 
                break;
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                OnSpawn?.Invoke(1);
                break;
        }

        return go;
    }

    Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController baseController = go.GetComponent<BaseController>();
        if (baseController == null) return Define.WorldObject.Unknown;
        return baseController.WorldObjectType;
    }

    public void DeSpawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.Player:
                if(go == _player) _player = null;
                break;
            case Define.WorldObject.Monster:
                if (_monsters.Contains(go))
                {
                    _monsters.Remove(go);
                    OnSpawn?.Invoke(-1);
                }
                break;
        }

        Managers.Resources.Destroy(go);
    }
}
