using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

enum PlayerState
{
    Idle,
    Moveing,
    Die,
    Battle,
}

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;
    [SerializeField] PlayerState _state = PlayerState.Idle;
    Animator _anim;
    NavMeshAgent _nav;
    Vector3 _destination;

    PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;
            //print($"Chaged : {Enum.GetName(typeof(PlayerState), _state)}");
            switch (_state)
            {
                case PlayerState.Idle:
                    _anim.CrossFade("WAIT", 0.2f);
                    break;
                case PlayerState.Moveing:
                    _anim.CrossFade("RUN", 0.1f);
                    break;
                case PlayerState.Battle:
                    _anim.CrossFade("Attack", 0.1f);
                    break;
            }
        }
    }

    void Start()
    {
        _anim = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _stat = GetComponent<PlayerStat>();
        Managers.Input.OnMouseInput -= OnMouseEvent;
        Managers.Input.OnMouseInput += OnMouseEvent;

        Managers.UI.MakeWorldSapce_UI<UI_HpBar>(transform);
    }

    void Update()
    {
        switch (State)
        {
            case PlayerState.Idle: break;
            case PlayerState.Moveing: UpdateMove(); break;
            case PlayerState.Battle: UpdateBattle(); break;
            default: break;
        }
    }

    [SerializeField] GameObject _lockTarget = null;
    private void UpdateMove()
    {
        if(_lockTarget != null)
        {
            _destination = _lockTarget.transform.position;
            float distance = (_destination - transform.position).magnitude;
            if (distance < 1)
            {
                State = PlayerState.Battle;
                return;
            }
        }
        MoveToDestination();
    }

    void MoveToDestination()
    {
        Vector3 dir = _destination - transform.position;

        if (dir.magnitude < 0.1f)
            State = PlayerState.Idle;
        else
        {
            float moveDistance = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            _nav.Move(dir.normalized * moveDistance);

            Debug.DrawRay(transform.position + Vector3.up, dir.normalized, Color.green);
            if(Physics.Raycast(transform.position + Vector3.up, dir, 1, LayerMask.GetMask("Block")))
            {
                if(Input.GetMouseButton(0) == false)
                    State = PlayerState.Idle;
                return;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }

    void UpdateBattle()
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
            print(_stat.Attack - _lockTarget.GetComponent<Stat>().Defense);
        }

        if (_stopSkill)
            State = PlayerState.Idle;
        else
            State = PlayerState.Battle;
    }

    int _targetMask = 1 << (int)Define.Layer.Plane | 1 << (int)Define.Layer.Monster;
    bool _stopSkill = false;
    void OnMouseEvent(Define.MouseEvent mouseEvent)
    {
        switch (_state)
        {
            case PlayerState.Idle: OnMouseEvent_IdleRun(mouseEvent); break;
            case PlayerState.Moveing: OnMouseEvent_IdleRun(mouseEvent); break;
            case PlayerState.Battle: if (mouseEvent == Define.MouseEvent.Up) _stopSkill = true; break;
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
                        _stopSkill = false;
                        _destination = hitInfo.point;
                        State = PlayerState.Moveing;

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
            case Define.MouseEvent.Up: _stopSkill = true; break;
        }
    }

    // 이제는 안쓰지만 아까우니까 남겨둔 코드
    //void OnKeyboard()
    //{
    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        // Quaternion.LookRotation(Vector3) : 인자값에 들어간 Vector3좌표를 바라보는 쿼터니언 return
    //        // transform.Translate()는 이동 방향이 자기 기준이라 이렇게 방향을 바꿔줄 거면 그냥 transform.position에 직빵으로 더하는게 나음
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
    //        transform.position += Vector3.forward * Time.deltaTime * speed;
    //    }
    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
    //        transform.position += Vector3.back * Time.deltaTime * speed;
    //    }
    //    if (Input.GetKey(KeyCode.D))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
    //        transform.position += Vector3.right * Time.deltaTime * speed;
    //    }
    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
    //        transform.position += Vector3.left * Time.deltaTime * speed;
    //    }
    //}
}