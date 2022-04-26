using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager
{
    public event Action OnKeyInput = null;

    public void OnUpdate()
    {
        if (!Input.anyKey) return;

        OnKeyInput?.Invoke();
    }
}