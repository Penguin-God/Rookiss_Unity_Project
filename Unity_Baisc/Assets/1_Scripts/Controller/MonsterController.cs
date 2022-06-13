using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    [SerializeField] Stat _stat;
    protected override void Init()
    {
        _stat = GetComponent<Stat>();
    }


    float _scanRange = 10;
    float _attackRange = 2;

    protected override void UpdateIdle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(distance < _scanRange)
        {
            _lockTarget = player;
            State = CreatureState.Moveing;
        }
    }

    protected override void UpdateMove()
    {
        if (_lockTarget != null)
        {
            _destination = _lockTarget.transform.position;
            float distance = (_destination - transform.position).magnitude;
            if (distance < _attackRange)
            {
                _nav.SetDestination(transform.position);
                State = CreatureState.Battle;
                return;
            }
        }
        MoveToDestination();
    }

    void MoveToDestination()
    {
        Vector3 dir = _destination - transform.position;

        if (dir.magnitude < 0.1f)
            State = CreatureState.Idle;
        else
        {
            _nav.speed = _stat.MoveSpeed;
            _nav.SetDestination(_destination);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }

    protected override void UpdateBattle()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.3f);
        }
    }

    void OnHitEvent()
    {
        print("Hit Monsetr");
        if(_lockTarget == null)
        {
            State = CreatureState.Idle;
            return;
        }

        _lockTarget.GetComponent<PlayerStat>().Hp -= Mathf.Max(0, _stat.Attack - _lockTarget.GetComponent<Stat>().Defense);
        if (_lockTarget.GetComponent<PlayerStat>().Hp > 0)
        {
            if((_lockTarget.transform.position - transform.position).magnitude < _attackRange)
            {
                State = CreatureState.Battle;
            }
            else
            {
                State = CreatureState.Idle;
            }
        }
    }
}
