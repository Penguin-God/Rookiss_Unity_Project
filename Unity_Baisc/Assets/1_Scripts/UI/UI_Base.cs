using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Object = UnityEngine.Object;

public class UI_Base : MonoBehaviour
{
    Dictionary<Type, Object[]> _objectsByType = new Dictionary<Type, Object[]>();

    protected void Bind<T>(Type type) where T : Object
    {
        string[] names = Enum.GetNames(type); // enum의 이름들을 가져옴. C# 리플렉션의 놀라운 은총, C++은 없음 ㅋㅋ
        Object[] objects = new Object[names.Length];
        _objectsByType.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, true, names[i]);
            else
                objects[i] = Util.FindChild<T>(gameObject, true, names[i]);

            if (objects[i] == null) print("실패!!");
        }
    }

    protected T Get<T>(int index) where T : Object
    {
        if (_objectsByType.TryGetValue(typeof(T), out Object[] objects) == false) return null;

        return objects[index] as T;
    }

    protected Text GetText(int index) => Get<Text>(index);
    protected Button GetButton(int index) => Get<Button>(index);
    protected Image GetImage(int index) => Get<Image>(index);
}
