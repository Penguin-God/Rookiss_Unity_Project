using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tests : MonoBehaviour
{
    // �ݸ��� �浹
    // 1 �� �� �ϳ� ������ٵ� ���� (isKinematic flase)
    // 2 �� �� isTrigger false 
    void OnCollisionEnter(Collision collision)
    {
        print($"�ȳ� �浹 : ����{name}��� ��");
    }

    // Ʈ���� �浹
    // 1 �� �� �ϳ� ������ٵ� ���� (isKinematic ���� ��� X)
    // 2 �� �� �ϳ��� isTrigger true
    void OnTriggerEnter(Collider other)
    {
        print($"�ȳ� Ʈ���� : ����{name}��� ��");
    }
}
