using UnityEngine;
using UnityEngine.UI;

/// <summary>
///淡入淡出
/// </summary>
public class Fader : TransitionBase
{
    [SerializeField] private Image faderImage = null;
    [SerializeField] private float fadeTime = 1.0f;

    private float playFadeTime_ = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (State == TransitionState.None)
        {
            return;
        }

        //更新
        Color color = faderImage.color;
        playFadeTime_ += Time.deltaTime;
        if (State == TransitionState.ScreenIn)
        {
            if (fadeTime > 0.0f)
            {
                color.a = Mathf.Max(1.0f - (playFadeTime_ / fadeTime), 0.0f);
            }
            else
            {
                color.a = 0.0f;
            }

            if (color.a == 0.0f)
            {
                faderImage.gameObject.SetActive(false);
            }
        }
        else if (State == TransitionState.ScreenOut)
        {
            if (fadeTime > 0.0f)
            {
                color.a = Mathf.Min((playFadeTime_ / fadeTime), 1.0f);
            }
            else
            {
                color.a = 1.0f;
            }
        }

        faderImage.color = color;
        //结束判定
        if (fadeTime <= 0.0f || (playFadeTime_ / fadeTime) >= 1.0f)
        {
            playFadeTime_ = 0.0f;
            FinishTransition();
        }
    }

    /// <summary>
    ///开始迁移
    /// </summary>
    /// <param name="transitionState"></param>
    protected override void StartTransition(TransitionState transitionState)
    {
        if (transitionState == TransitionState.ScreenOut)
        {
            //淡出
            Color color = faderImage.color;
            color.a = 0.0f;
            faderImage.color = color;
            faderImage.gameObject.SetActive(true);
            playFadeTime_ = 0.0f;
        }
        else
        {
            //淡入
            Color color = faderImage.color;
            color.a = 1.0f;
            faderImage.color = color;
            faderImage.gameObject.SetActive(true);
            playFadeTime_ = 0.0f;
        }
    }
}