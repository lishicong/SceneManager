using System;
using UnityEngine;

/// <summary>
// 单例
/// </summary>
//<typeparam name=“T”>要成为单体的类的类型</typeparam>
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance_;

    public static T Instance
    {
        get
        {
            if (instance_ == null)
            {
                Type t = typeof(T);
                instance_ = (T) FindObjectOfType(t);
                if (instance_ == null)
                {
                    Debug.LogError(t + " 没有附着的GameObject");
                }
            }

            return instance_;
        }
    }

    public static bool IsExist()
    {
        return (instance_ != null);
    }

    protected virtual void Awake()
    {
        if (Instance != this)
        {
            Type t = typeof(T);
            Debug.LogError(t + " 已附着");
            return;
        }
    }
}