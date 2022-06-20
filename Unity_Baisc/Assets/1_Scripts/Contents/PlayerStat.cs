using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField] int _exp;
    [SerializeField] int _needExp;
    [SerializeField] int _gold;

    public int Gold { get => _gold; set => _gold = value; }

    void Start()
    {
        Level = 1;
        SetStat();

        Defense = 10;
        MoveSpeed = 5;
        Gold = 10;
    }

    void SetStat()
    {
        MaxHp = Managers.Data.StatByLevel[Level].hp;
        Hp = MaxHp;
        Attack = Managers.Data.StatByLevel[Level].attack;
        _needExp = Managers.Data.StatByLevel[Level].exp;
        print(_needExp);
        _exp = 0;
    }

    public void AddExp(int newExp)
    {
        _exp += newExp;
        if (_exp > _needExp) LevelUp();
    }
    void LevelUp()
    {
        int tempExp = _exp;
        while (tempExp >= _needExp)
        {
            Level++;
            tempExp -= _needExp;
            if (Managers.Data.StatByLevel.ContainsKey(Level) == false) break;

            SetStat();
        }

        _exp = tempExp;
    }

    protected override void OnDead(Stat stat)
    {
        print("응애 나 죽음");
    }
}
