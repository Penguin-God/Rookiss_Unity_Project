using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HpBar : UI_Base
{
    enum GameObjets
    {
        HpBar
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjets));
    }


    void Update()
    {
        transform.position = transform.parent.position + (Vector3.up * transform.parent.GetComponent<Collider>().bounds.size.y);

    }
}
