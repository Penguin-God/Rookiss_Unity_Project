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
    UI_Manager _ui = new UI_Manager();

    public static InputManager Input => Instance._input;
    public static ResourcesManager Resources => Instance._resources;
    public static UI_Manager UI => Instance._ui;

    void Update()
    {
        _input.OnUpdate();
    }
}
