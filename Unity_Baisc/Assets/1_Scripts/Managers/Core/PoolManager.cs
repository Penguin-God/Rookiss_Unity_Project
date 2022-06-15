using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Pool
{
    public GameObject Original { get; private set; }
    public Transform Root { get; private set; }
    Stack<Poolable> _poolStack = new Stack<Poolable>();

    public void Init(GameObject original, int count)
    {
        Original = original;
        Root = new GameObject($"{original.name}_Root").transform;
        for (int i = 0; i < count; i++)
            Push(Create());
    }

    
    Poolable Create()
    {
        GameObject go = Object.Instantiate(Original);
        go.name = Original.name;
        return go.GetOrAddComponent<Poolable>();
    }

    public void Push(Poolable poolable)
    {
        if (poolable == null) return;

        poolable.transform.SetParent(Root);
        poolable.gameObject.SetActive(false);
        poolable.IsUsing = false;
        _poolStack.Push(poolable);
    }

    public Poolable Pop(Transform parent)
    {
        Poolable poolable;
        if (_poolStack.Count > 0) poolable = _poolStack.Pop();
        else poolable = Create();

        ReleaseDontDestoryOnLoad(poolable.transform, parent);
        poolable.gameObject.SetActive(true);
        poolable.transform.SetParent(parent);
        poolable.IsUsing = true;
        return poolable;
    }

    void ReleaseDontDestoryOnLoad(Transform transform, Transform parent)
    {
        if (parent == null)
            transform.SetParent(Managers.Scene.CurrentScene.transform);
    }
}

public class PoolManager
{
    Transform _root;
    Dictionary<string, Pool> _poolByName = new Dictionary<string, Pool>();

    public void Init()
    {
        _root = new GameObject("@Pool_Root").transform;
        Object.DontDestroyOnLoad(_root.gameObject);
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.SetParent(_root);
        _poolByName.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        if (_poolByName.ContainsKey(poolable.gameObject.name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }
        _poolByName[poolable.gameObject.name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_poolByName.ContainsKey(original.name) == false)
            CreatePool(original); 
        
        return _poolByName[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        if (_poolByName.ContainsKey(name) == false)
            return null;
        return _poolByName[name].Original;
    }

    public void Clear()
    {
        foreach (Transform root in _root)
            GameObject.Destroy(root.gameObject);

        _poolByName.Clear();
    }
}
