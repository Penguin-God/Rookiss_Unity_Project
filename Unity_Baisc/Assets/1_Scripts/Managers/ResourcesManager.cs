using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager
{
    public T Load<T> (string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if(prefab == null)
        {
            Debug.LogError($"찾을 수 없는 Resources 경로 {path}");
            return null;
        }

        GameObject go = Object.Instantiate(prefab, parent); // Instantiate 재귀호출 방지로 Object. 붙임
        int index = go.name.IndexOf("(Clone)");
        if (index > 0) go.name = go.name.Substring(0, index);

        return go;
    }

    public void Destroy(GameObject go)
    {
        Object.Destroy(go);
    }
}
