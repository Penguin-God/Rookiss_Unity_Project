using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : BaseController
{
    [SerializeField] PlayerStat _stat;

    protected override void Init()
    {
        _stat = GetComponent<PlayerStat>();
        Managers.Input.OnMouseInput -= OnMouseEvent;
        Managers.Input.OnMouseInput += OnMouseEvent;
    }

    protected override void UpdateMove()
    {
        if(_lockTarget != null)
        {
            _destination = _lockTarget.transform.position;
            float distance = (_destination - transform.position).magnitude;
            if (distance < 1)
            {
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
            Debug.DrawRay(transform.position + Vector3.up, dir.normalized, Color.green);
            if(Physics.Raycast(transform.position + Vector3.up, dir, 1, LayerMask.GetMask("Block")))
            {
                if(Input.GetMouseButton(0) == false)
                    State = CreatureState.Idle;
                return;
            }

            float moveDistance = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDistance;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }

    protected override void UpdateBattle()
    {
        if(_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.3f);
        }
    }

    void OnHitEvent()
    {
        if(_lockTarget != null)
        {
            _lockTarget.GetComponent<Stat>().Hp -= Math.Max(0, _stat.Attack - _lockTarget.GetComponent<Stat>().Defense);
        }

        DecideBattleOrNot();

        void DecideBattleOrNot()
        {
            if (_stopBattle)
                State = CreatureState.Idle;
            else
                State = CreatureState.Battle;
        }
    }

    protected override void AttackHitEvent()
    {
        ToDamage(_stat.Attack);
        DecideBattleOrNot();

        void ToDamage(int damage)
        {
            if (_lockTarget != null)
                _lockTarget.GetComponent<BaseController>().OnDamaged(damage);
        }
    }

    public override void OnDamaged(int damage)
    {
        print("대미지 입음!!");
        _stat.Hp -= Mathf.Max(0, damage - _stat.Defense);
    }

    protected override void DecideBattleOrNot()
    {
        if (_stopBattle)
            State = CreatureState.Idle;
        else
            State = CreatureState.Battle;
    }

    int _targetMask = 1 << (int)Define.Layer.Plane | 1 << (int)Define.Layer.Monster;
    bool _stopBattle = false;
    void OnMouseEvent(Define.MouseEvent mouseEvent)
    {
        switch (State)
        {
            case CreatureState.Idle: OnMouseEvent_IdleRun(mouseEvent); break;
            case CreatureState.Moveing: OnMouseEvent_IdleRun(mouseEvent); break;
            case CreatureState.Battle: if (mouseEvent == Define.MouseEvent.Up) _stopBattle = true; break;
        }
    }

    void OnMouseEvent_IdleRun(Define.MouseEvent mouseEvent)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isRayHit = Physics.Raycast(ray, out RaycastHit hitInfo, 100, _targetMask);

        switch (mouseEvent)
        {
            case Define.MouseEvent.Down:
                {
                    if (isRayHit)
                    {
                        _stopBattle = false;
                        _destination = hitInfo.point;
                        State = CreatureState.Moveing;

                        if (hitInfo.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hitInfo.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;
            case Define.MouseEvent.Press:
                {
                    if (isRayHit)
                        _destination = hitInfo.point;
                }
                break;
            case Define.MouseEvent.Up: _stopBattle = true; break;
        }
    }
}