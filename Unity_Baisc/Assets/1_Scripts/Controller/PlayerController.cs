using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;

    [SerializeField] bool _isMoveToDestination;
    Vector3 _destination;

    void Start()
    {
        Managers.Input.OnKeyInput -= OnKeyboard;
        Managers.Input.OnKeyInput += OnKeyboard;
        Managers.Input.OnMouseInput -= MouseDownAction;
        Managers.Input.OnMouseInput += MouseDownAction;
    }

    void Update()
    {
        MoveToDestination();
    }

    void OnKeyboard()
    {
        // TODO : _isMoveToDestination = false; 부분 리펙토링
        if (Input.GetKey(KeyCode.W))
        {
            // Quaternion.LookRotation(Vector3) : 인자값에 들어간 Vector3좌표를 바라보는 쿼터니언 return
            // transform.Translate()는 이동 방향이 자기 기준이라 이렇게 방향을 바꿔줄 거면 그냥 transform.position에 직빵으로 더하는게 나음
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            transform.position += Vector3.forward * Time.deltaTime * speed;
            _isMoveToDestination = false;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * speed;
            _isMoveToDestination = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * speed;
            _isMoveToDestination = false;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * speed;
            _isMoveToDestination = false;
        }
    }

    void MouseDownAction(Define.MouseEvent mouseEvent)
    {
        if (mouseEvent != Define.MouseEvent.Down) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 100, LayerMask.GetMask("Plane")))
        {
            _destination = hitInfo.point;
            _isMoveToDestination = true;
        }
    }

    void MoveToDestination()
    {
        if (_isMoveToDestination)
        {
            Vector3 dir = _destination - transform.position;

            if (dir.magnitude < 0.001f) 
                _isMoveToDestination = false;
            else
            {
                float moveDistance = Mathf.Clamp(speed * Time.deltaTime, 0, dir.magnitude);
                transform.position += dir.normalized * moveDistance;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
            }
        }
    }
}