using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tests : MonoBehaviour
{
    // 콜리젼 충돌
    // 1 둘 중 하나 리지드바디 소유 (isKinematic flase)
    // 2 둘 다 isTrigger false 
    void OnCollisionEnter(Collision collision)
    {
        print($"안녕 충돌 : 나는{name}라고 해");
    }

    // 트리거 충돌
    // 1 둘 중 하나 리지드바디 소유 (isKinematic 여부 상관 X)
    // 2 둘 중 하나라도 isTrigger true
    void OnTriggerEnter(Collider other)
    {
        print($"안녕 트리거 : 나는{name}라고 해");
    }
}
