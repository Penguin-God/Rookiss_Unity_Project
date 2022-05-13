using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    public enum CameraMode
    {
        QuarterView,
    }

    public enum MouseEvent
    {
        Down,
        Press,
        Up,
    }

    public enum UI_Event
    {
        Click,
        Drag,
    }
}
