using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    enum CursorType
    {
        None,
        Hand,
        Attack,
    }

    CursorType _cursorType = CursorType.None;
    Texture2D _handCursor;
    Texture2D _attackCursor;

    void Start()
    {
        _handCursor = Managers.Resources.Load<Texture2D>("Textures/Curosr/Hand");
        _attackCursor = Managers.Resources.Load<Texture2D>("Textures/Curosr/Attack");
    }

    void Update()
    {
        UpdateMouseCursor();
    }

    int _targetMask = 1 << (int)Define.Layer.Plane | 1 << (int)Define.Layer.Monster;
    void UpdateMouseCursor()
    {
        if (Input.GetMouseButton(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, _targetMask))
        {
            if (hitInfo.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack)
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
}
