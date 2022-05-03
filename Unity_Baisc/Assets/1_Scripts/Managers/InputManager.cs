using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager
{
    public event Action OnKeyInput = null;

    public event Action<Define.MouseEvent> OnMouseInput = null;

    public void OnUpdate()
    {
        if (Input.anyKey) OnKeyInput?.Invoke();

        if(OnMouseInput != null)
        {
            if (Input.GetMouseButtonDown(0)) OnMouseInput(Define.MouseEvent.Down);
            if (Input.GetMouseButton(0)) OnMouseInput(Define.MouseEvent.Press);
            if (Input.GetMouseButtonUp(0)) OnMouseInput(Define.MouseEvent.Up);
        }
    }
}