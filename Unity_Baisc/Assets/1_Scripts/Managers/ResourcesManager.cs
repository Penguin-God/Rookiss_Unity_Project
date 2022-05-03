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

        return Object.Instantiate(prefab, parent); // Instantiate 재귀호출 방지로 Object. 붙임
    }
}
