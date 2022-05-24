using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    PlayerState _state = PlayerState.Idle;
    Animator _anim;
    NavMeshAgent _nav;
    Vector3 _destination;

    PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;

            switch (_state)
            {
                case PlayerState.Idle: 
                    _anim.SetBool("IsRun", false);
                    _anim.SetBool("IsAttack", false);
                    break;
                case PlayerState.Moveing: 
                    _anim.SetBool("IsRun", true);
                    _anim.SetBool("IsAttack", false);
                    break;
                case PlayerState.Battle: _anim.SetBool("IsAttack", true); break;
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
    }

    void Update()
    {
        switch (State)
        {
            case PlayerState.Idle: break;
            case PlayerState.Moveing: UpdateMove(); break;
            case PlayerState.Battle: StartCoroutine(Co_OnHitEvent()); ; break;
            default: break;
        }
    }

    private void UpdateMove()
    {
        //_anim.SetBool("IsRun", true);
        if(_lockTarget != null)
        {
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

        //_nav.CalculatePath
        
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

    IEnumerator Co_OnHitEvent()
    {
        State = PlayerState.Battle;
        yield return new WaitForSeconds(0.2f);
        print("안녕");
        State = PlayerState.Idle;
        StopAllCoroutines();
    }

    int _targetMask = 1 << (int)Define.Layer.Plane | 1 << (int)Define.Layer.Monster;
    GameObject _lockTarget = null;
    void OnMouseEvent(Define.MouseEvent mouseEvent)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isRayHit = Physics.Raycast(ray, out RaycastHit hitInfo, 100, _targetMask);
        switch (mouseEvent)
        {
            case Define.MouseEvent.Down:
                {
                    if (isRayHit)
                    {
                        _destination = hitInfo.point;
                        State = PlayerState.Moveing;
                    }

                    if (hitInfo.collider.gameObject.layer == (int)Define.Layer.Monster)
                        _lockTarget = hitInfo.collider.gameObject;
                    else
                        _lockTarget = null;
                }
                break;
            case Define.MouseEvent.Press:
                {
                    if (_lockTarget != null)
                        _destination = _lockTarget.transform.position;
                    else if (isRayHit)
                        _destination = hitInfo.point;
                }
                break;
            case Define.MouseEvent.Up: break;
            case Define.MouseEvent.Click: print("Click!!"); break;
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