using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance;
    private static Managers Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Managers>();
                if(instance == null)
                {
                    instance = new GameObject("Managers").AddComponent<Managers>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }

            return instance;
        }
    }

    InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    void Update()
    {
        _input.OnUpdate();
    }
}
