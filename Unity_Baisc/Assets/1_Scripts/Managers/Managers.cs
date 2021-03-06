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
                    instance = new GameObject("Managers").AddComponent<Managers>();

                DontDestroyOnLoad(instance.gameObject);
                instance.Init();
            }

            return instance;
        }
    }

    #region Contents

    GameManager _game = new GameManager();
    public static GameManager Game => Instance._game;

    #endregion Contents


    #region Core

    DataManger _data = new DataManger();
    InputManager _input = new InputManager();
    ResourcesManager _resources = new ResourcesManager();
    UI_Manager _ui = new UI_Manager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    PoolManager _pool = new PoolManager();

    #endregion Core

    public static DataManger Data => Instance._data;
    public static InputManager Input => Instance._input;
    public static ResourcesManager Resources => Instance._resources;
    public static UI_Manager UI => Instance._ui;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static PoolManager Pool => Instance._pool;

    [ContextMenu("Test Init")]
    void Init()
    {
        Data.Init();
        Sound.Init();
        Pool.Init();
    }

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        UI.Clear();
        Scene.Clear();
        Pool.Clear();
    }

    void Update()
    {
        _input.OnUpdate();
    }
}
