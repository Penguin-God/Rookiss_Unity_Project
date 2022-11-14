using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtils : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
            GetMoney();
    }

    void GetMoney()
    {
        Managers.Game.Coin += 1000;
        Managers.Game.Dia += 100;
    }
}
