using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField] int  _level;
    [SerializeField] int  _maxHp;
    [SerializeField] int  _hp;
    [SerializeField] int  _attack;
    [SerializeField] int  _defense;
    [SerializeField] float  _moveSpeed;

    public int Level { get => _level; set => _level = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int Hp { get => _hp; set => _hp = value; }
    public int Attack { get => _attack; set => _attack = value; }
    public int Defense { get => _defense; set => _defense = value; }
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    void Start()
    {
        Level = 1;
        MaxHp = 100;
        Hp = 100;
        Attack = 100;
        Attack = 20;
        Defense = 10;
        MoveSpeed = 5;
    }
}
