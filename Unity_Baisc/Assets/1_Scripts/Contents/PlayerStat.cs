using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField] int _exp;
    [SerializeField] int _gold;

    public int Exp { get => _exp; set => _exp = value; }
    public int Gold { get => _gold; set => _gold = value; }

    void Start()
    {
        Level = 1;
        MaxHp = 100;
        Hp = 100;
        Attack = 20;
        Defense = 10;
        MoveSpeed = 5;
        Exp = 10;
        Gold = 10;
    }
}
