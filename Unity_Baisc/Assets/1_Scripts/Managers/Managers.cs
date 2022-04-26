using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance;
    public static Managers Instance
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
}
