using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManager
{
    public event Action OnKeyInput = null;

    public event Action<Define.MouseEvent> OnMouseInput = null;

    public void OnUpdate()
    {
        // UI 버튼이 클릭된 상태면 return
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.anyKey) OnKeyInput?.Invoke();

        if(OnMouseInput != null)
        {
            if (Input.GetMouseButtonDown(0)) OnMouseInput(Define.MouseEvent.Down);
            if (Input.GetMouseButton(0)) OnMouseInput(Define.MouseEvent.Press);
            if (Input.GetMouseButtonUp(0)) OnMouseInput(Define.MouseEvent.Up);
        }
    }
}