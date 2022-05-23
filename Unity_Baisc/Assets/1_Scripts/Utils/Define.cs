using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Layer
    {
        Plane = 6,
        Wall = 7,
        Bolck = 8,
        Monster = 9,
    }
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
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
        Click,
    }

    public enum UI_Event
    {
        Click,
        Drag,
    }
}
