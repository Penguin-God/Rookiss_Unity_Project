using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : UI_Base
{

    enum GameObjects
    {
        HpBar
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        _hpBar = GetObject((int)GameObjects.HpBar).GetComponent<Slider>();
        _stat = transform.parent.GetComponent<Stat>();
        _hpBar.maxValue = _stat.MaxHp;
    }

    Stat _stat;
    void Update()
    {
        transform.position = transform.parent.position + (Vector3.up * transform.parent.GetComponent<Collider>().bounds.size.y);
        transform.rotation = Camera.main.transform.rotation;
        SetHp(_stat.Hp);
    }

    Slider _hpBar;
    public void SetHp(int newHp)
    {
        _hpBar.value = newHp;
    }
}
