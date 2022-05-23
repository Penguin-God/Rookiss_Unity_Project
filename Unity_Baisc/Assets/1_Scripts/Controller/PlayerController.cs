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

enum CursorType
{
    None,
    Hand,
    Attack,
}

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;
    PlayerState _state = PlayerState.Idle;
    CursorType _cursorType = CursorType.None;
    Animator _anim;
    NavMeshAgent _nav;
    Vector3 _destination;

    Texture2D _handCursor;
    Texture2D _attackCursor;
    void Start()
    {
        _anim = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _stat = GetComponent<PlayerStat>();
        Managers.Input.OnMouseInput -= OnMouseEvent;
        Managers.Input.OnMouseInput += OnMouseEvent;
        _handCursor = Managers.Resources.Load<Texture2D>("Textures/Curosr/Hand");
        _attackCursor = Managers.Resources.Load<Texture2D>("Textures/Curosr/Attack");
    }

    void Update()
    {
        switch (_state)
        {   
            case PlayerState.Idle: UpdateIdle(); break;
            case PlayerState.Moveing: UpdateMove(); break;
            default: break;
        }

        UpdateMouseCursor();
    }

    void UpdateMouseCursor()
    {
        if (Input.GetMouseButton(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, _targetMask))
        {
            if (hitInfo.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if(_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attackCursor, new Vector2(_attackCursor.width / 5, 0), CursorMode.Auto);
                    _cursorType = CursorType.Attack;
                }
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_handCursor, new Vector2(_handCursor.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }
    }

    private void UpdateIdle()
    {
        _anim.SetBool("IsRun", false);
    }

    private void UpdateMove()
    {
        _anim.SetBool("IsRun", true);
        MoveToDestination();
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
                        _state = PlayerState.Moveing;
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
            case Define.MouseEvent.Up: _lockTarget = null; break;
            case Define.MouseEvent.Click:
                break;
            default:
                break;
        }


    }

    void MoveToDestination()
    {
        Vector3 dir = _destination - transform.position;

        //_nav.CalculatePath
        
        if (dir.magnitude < 0.1f)
            _state = PlayerState.Idle;
        else
        {
            float moveDistance = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            _nav.Move(dir.normalized * moveDistance);

            Debug.DrawRay(transform.position + Vector3.up, dir.normalized, Color.green);
            if(Physics.Raycast(transform.position + Vector3.up, dir, 1, LayerMask.GetMask("Block")))
            {
                if(Input.GetMouseButton(0) == false)
                    _state = PlayerState.Idle;
                return;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
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