using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class Managers : MonoBehaviour
{
    public static Managers s_instance = null;
    public static Managers Instance { get { return s_instance; } }

	DataManager _data = new DataManager();
	GameManagerEx _game = new GameManagerEx();
    ObjectManager _object = new ObjectManager();
	ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance?._data; } }
    public static GameManagerEx Game { get { return Instance?._game; } }
    public static ObjectManager Object { get { return Instance?._object; } }
    public static UIManager UI { get { return Instance?._ui; } }
	public static ResourceManager Resource { get { return Instance?._resource; } }
	public static SceneManagerEx Scene { get {  return Instance?._scene; } }
	public static SoundManager Sound { get { return Instance?._sound; } }

	private void Awake()
    {
        //Init();
	}

	public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            s_instance = Utils.GetOrAddComponent<Managers>(go);
            DontDestroyOnLoad(go);

            s_instance._data.Init();
            s_instance._sound.Init();
            s_instance._game.Init();
        }
    }

	public static void Clear()
	{
        s_instance._sound.Clear();
		s_instance._ui.Clear();
        s_instance._object.Clear();
	}
}
