using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : UI_Scene
{
    enum GameObjects
    {
        GridPanel,
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject grid = Get<GameObject>((int)GameObjects.GridPanel);
        foreach (Transform item in grid.transform)
            Managers.Resources.Destroy(item.gameObject);

        // 인벤토리에 아이템 UI 생성
        for (int i = 0; i < 6; i++)
        {
            GameObject item = Managers.UI.MakeSubItem<UI_InventoryItem>(parent: grid.transform).gameObject;
            item.GetOrAddComponent<UI_InventoryItem>().SetInfo($"집행검 제 {i + 1}번");
        }
    }
}
