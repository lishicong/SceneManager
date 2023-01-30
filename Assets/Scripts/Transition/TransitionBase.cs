using UnityEngine;
using UnityEngine.Events;

/// <summary>
///迁移状态
/// </summary>
public enum TransitionState
{
    ScreenOut,
    ScreenIn,
    None
}

/// <summary>
///迁移库
/// </summary>
public abstract class TransitionBase : MonoBehaviour
{
    public TransitionState State { get; private set; } = TransitionState.None;
    private UnityAction callback_ = null;

    /// <summary>
    ///迁移状态设置
    /// </summary>
    /// <param name="transitionState"></param>
    /// <returns></returns>
    public bool SetTransitionState(TransitionState transitionState, UnityAction callback)
    {
        //仅在未迁移时有效
        if (State != TransitionState.None)
        {
            return false;
        }

        //开始迁移
        callback_ = callback;
        State = transitionState;
        StartTransition(State);
        return true;
    }

    /// <summary>
    ///迁移结束
    /// </summary>
    protected void FinishTransition()
    {
        State = TransitionState.None;
        if (callback_ != null)
        {
            UnityAction temp = callback_;
            callback_ = null;
            temp();
        }
    }

    /// <summary>
    ///开始迁移
    /// </summary>
    /// <param name="transitionState"></param>
    protected abstract void StartTransition(TransitionState transitionState);
}