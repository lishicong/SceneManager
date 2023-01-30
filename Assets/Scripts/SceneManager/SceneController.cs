using System;
using System.Collections;
using UnityEngine;
/// <summary>
///场景控制
/// </summary>
public class SceneController : MonoBehaviour
{
    public bool IsInitialized { get; private set; } = false;
    public bool IsDestroyed { get; private set; } = false;
    /// <summary>
    ///屏幕退出开始
    /// </summary>
    public virtual void StartedSceneChange() { }
    /// <summary>
    ///屏幕进入完成
    /// </summary>
    public virtual void FinishedSceneChange() { }
    /// <summary>
    ///场景初始化
    /// </summary>
    public void Initialize()
    {
        StartCoroutine(WaitingForTheEndCoroutine(OnInitializeCoroutine(), () => {
            IsInitialized = true;
        }));
    }
    /// <summary>
    ///场景删除
    /// </summary>
    public void Destory()
    {
        StartCoroutine(WaitingForTheEndCoroutine(OnDestroyCoroutine(), () => {
            IsDestroyed = true;
        }));
    }
    /// <summary>
    //场景初始化协程
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator OnInitializeCoroutine()
    {
        yield return null;
    }
    /// <summary>
    ///场景删除协程
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator OnDestroyCoroutine()
    {
        yield return null;
    }
    /// <summary>
    //等待协程完成
    /// </summary>
    /// <param name="enumerator"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator WaitingForTheEndCoroutine(IEnumerator enumerator, Action callback)
    {
        while (!Boot.IsInitialize) { yield return null; }
        yield return StartCoroutine(enumerator);
        if (callback != null) { callback(); }
    }
}