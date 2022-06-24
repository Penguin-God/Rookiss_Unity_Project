using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : BaseController
{
    [SerializeField] PlayerStat _stat;
    public override bool IsDead => _stat.Hp <= 0;

    protected override void Init()
    {
        _stat = GetComponent<PlayerStat>();
        WorldObjectType = Define.WorldObject.Player;
        Managers.Input.OnMouseInput -= OnMouseEvent;
        Managers.Input.OnMouseInput += OnMouseEvent;
    }

    protected override void UpdateMove()
    {
        if(_lockTarget != null)
        {
            SetDestination(TargetPosition);
            if ((Destination - transform.position).magnitude < 1)
            {
                State = CreatureState.Battle;
                return;
            }
        }
        MoveToDestination();

        void MoveToDestination()
        {
            Vector3 dir = Destination - transform.position;
            dir.y = 0;

            if (dir.magnitude < 0.1f)
                State = CreatureState.Idle;
            else
            {
                Debug.DrawRay(transform.position + Vector3.up, dir.normalized, Color.green);
                if (Physics.Raycast(transform.position + Vector3.up, dir, 1, LayerMask.GetMask("Block")))
                {
                    if (Input.GetMouseButton(0) == false)
                        State = CreatureState.Idle;
                    return;
                }

                float moveDistance = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
                transform.position += dir.normalized * moveDistance;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
            }
        }
    }




    protected override void UpdateBattle()
    {
        base.UpdateBattle();
    }
    protected override void AttackHitEvent()
    {
        if(_lockTarget == null)
        {
            State = CreatureState.Idle;
            return;
        }
        AttackLockTarget(_stat);
        DecideBattleOrNot();
    }

    bool _stopBattle = false;
    protected void DecideBattleOrNot()
    {
        if (_stopBattle || _lockTarget == null)
            State = CreatureState.Idle;
        else
            State = CreatureState.Battle;
    }

    int _targetMask = 1 << (int)Define.Layer.Plane | 1 << (int)Define.Layer.Monster;
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
                        SetDestination(hitInfo.point);
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
                        SetDestination(hitInfo.point);
                }
                break;
            case Define.MouseEvent.Up: _stopBattle = true; break;
        }
    }
}