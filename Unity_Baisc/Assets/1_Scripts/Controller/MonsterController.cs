using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    [SerializeField] Stat _stat;
    private NavMeshAgent _nav;
    protected override void Init()
    {
        _stat = GetComponent<Stat>();
        _nav = GetComponent<NavMeshAgent>();
    }


    float _scanRange = 10;
    float _attackRange = 2;

    protected override void UpdateIdle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if(Vector3.Distance(player.transform.position, transform.position) < _scanRange)
        {
            _lockTarget = player;
            State = CreatureState.Moveing;
        }
    }

    protected override void UpdateMove()
    {
        if (_lockTarget != null)
        {
            SetDestination(TargetPosition);
            if (TargetDistance < _attackRange)
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
        Vector3 dir = Destination - transform.position;

        if (dir.magnitude < 0.1f)
            State = CreatureState.Idle;
        else
        {
            _nav.speed = _stat.MoveSpeed;
            _nav.SetDestination(Destination);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }


    protected override void UpdateBattle()
    {
        base.UpdateBattle();
    }

    protected override void AttackHitEvent()
    {
        if (_lockTarget == null)
        {
            State = CreatureState.Idle;
            return;
        }

        TargetController.OnDamaged(_stat.Attack);
        DecideBattleOrNot();
    }

    protected void DecideBattleOrNot()
    {
        if (TargetController.IsDead == false)
        {
            if (TargetDistance < _attackRange)
            {
                State = CreatureState.Battle;
            }
            else
            {
                State = CreatureState.Idle;
            }
        }
    }

    public override void OnDamaged(int damage)
    {
        _stat.Hp -= Mathf.Max(0, damage - _stat.Defense);
    }
}
