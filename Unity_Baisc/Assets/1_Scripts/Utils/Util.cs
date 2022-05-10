using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static T GetOrAddComponent<T>(GameObject go) where T : Component 
    {
        T component = go.GetComponent<T>();
        if (component == null) component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject parent, bool recursive = false, string name = "")
    {
        Transform tf = FindChild<Transform>(parent, recursive, name);
        if (tf != null) return tf.gameObject;
        else return null;
    }

    // Unity GameObject 자식 중에 원하는 타입, 이름을 가진 오브젝트의 컴포넌트로 있는 T를 반환함
    // recursive가 true면 모든 자식을 검사하고 아니면 직계 자손만 검사함
    // 이름이 없으면 타입만 비교함
    public static T FindChild<T>(GameObject parent, bool recursive = false, string findChildName = null) where T : UnityEngine.Object
    {
        if (parent == null) return null;

        if (recursive == false)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Transform tf = parent.transform.GetChild(i);
                if (string.IsNullOrEmpty(findChildName) || tf.name == findChildName)
                {
                    T component = tf.GetComponent<T>();
                    if (component != null) return component;
                }
            }
        }
        else
        {
            foreach (T component in parent.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(findChildName) || component.name == findChildName)
                    return component;
            }
        }

        return null;
    }
}
