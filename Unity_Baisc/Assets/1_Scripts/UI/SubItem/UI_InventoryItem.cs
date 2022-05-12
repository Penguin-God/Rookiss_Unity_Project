using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryItem : UI_Base
{
    enum GameObjects
    {
        ItemIcon,
        ItemNameText,
    }

    string _name;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvnet(data => print($"나는야 {_name}"));
    }

    public void SetInfo(string name)
    {
        _name = name;
    }
}
