using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T m_Instance = null;

    public T Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<T>();
                
                if (!m_Instance)
                {
                    GameObject go = new GameObject("Generated " + typeof(T).ToString() + " Singleton");
                    m_Instance = go.AddComponent<T>();
                }
            }

            return m_Instance;
        }
    }
}
