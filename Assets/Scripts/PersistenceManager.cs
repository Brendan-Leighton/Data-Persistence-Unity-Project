using System;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
    // SINGLETON ACCESS POINT
    public static PersistenceManager instance;

    public string playerName;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
