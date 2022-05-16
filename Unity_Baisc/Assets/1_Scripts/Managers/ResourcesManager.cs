using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager
{
    public T Load<T> (string path) where T : Object
    {
        if(typeof(T) == typeof(GameObject))
        {
            int index = path.LastIndexOf('/');
            string name = "";
            if (index > 0) name = path.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null) return go as T;
        }
        
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if(original == null)
        {
            Debug.LogError($"찾을 수 없는 Resources 경로 {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent); // Instantiate 재귀호출 방지로 Object. 붙임
        go.name = original.name;

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null) return;
        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
