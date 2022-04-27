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
    ResourcesManager _resources = new ResourcesManager();

    public static InputManager Input => Instance._input;
    public static ResourcesManager Resources => Instance._resources;

    void Update()
    {
        _input.OnUpdate();
    }
}
