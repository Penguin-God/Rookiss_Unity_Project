using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInput : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1)) new AbilityUseCase().TryStatUpgrade(Define.StatType.MaxHp);
        else if (Input.GetKey(KeyCode.Alpha2)) new AbilityUseCase().TryStatUpgrade(Define.StatType.WorkAbility);
        else if (Input.GetKey(KeyCode.Alpha3)) new AbilityUseCase().TryStatUpgrade(Define.StatType.Likeability);
        else if (Input.GetKey(KeyCode.Alpha4)) new AbilityUseCase().TryStatUpgrade(Define.StatType.Stress);
    }
}
