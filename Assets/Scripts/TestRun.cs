using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 测试运行
/// </summary>
public class TestRun : MonoBehaviour
{
    [SerializeField] private Button button = null;

    // Start is called before the first frame update
    void Start()
    {
        if (button == null)
        {
            return;
        }

        Debug.Log("LISHICONG TestRun Start");

        List<SceneType> sceneTypeList = new List<SceneType>();
        sceneTypeList.AddRange((SceneType[]) Enum.GetValues(typeof(SceneType)));
        sceneTypeList.Remove(SceneType.None);
        // SceneHelper.Instance.OpenPage("Unity_01_Activity", SceneType.Scene1.ToString());
        foreach (SceneType sceneType in sceneTypeList)
        {
            Button copy = GameObject.Instantiate(button, button.transform.parent);

            if (sceneType != SceneType.Back)
            {
                copy.onClick.AddListener(() =>
                {
                    SceneHelper.Instance.OpenScene(sceneType.ToString());
                    SceneManager.Instance.SetNextScene(sceneType);

                    SendPageStackToNative(); // 打印页面栈信息
                });
                copy.GetComponentInChildren<Text>().text = "打开 " + sceneType.ToString();
            }
            else
            {
                copy.onClick.AddListener(() => { OnNativeBackEvent("u3d click back"); });
                copy.GetComponentInChildren<Text>().text = "返回";
            }
        }

        button.gameObject.SetActive(false);
    }

    public void OnNativeLifeCycle(string jsonStr)
    {
        Debug.Log("LISHICONG TestRun OnNativeLifeCycle jsonStr:" + jsonStr);

        JObject jObj = JObject.Parse(jsonStr);
        JToken lifeCycleNameToken = jObj["lifeCycleName"]; //生命周期名字
        string lifeCycleName = lifeCycleNameToken.ToString();
        JToken pageNameToken = jObj["pageName"];
        string pageName = pageNameToken.ToString();
        JToken sceneNameToken = jObj["sceneName"];
        string sceneName = sceneNameToken.ToString();
        JToken timeToken = jObj["time"];
        long time = long.Parse(timeToken.ToString());

        long time2 = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        Debug.Log("LISHICONG TestRun OnNativeLifeCycle time:" + time);
        Debug.Log("LISHICONG TestRun OnNativeLifeCycle time2:" + time2);
        Debug.Log("LISHICONG TestRun OnNativeLifeCycle time3:" + (time2-time));

        if (lifeCycleName == "onCreate")
        {
            SceneHelper.Instance.OpenPage(pageName, sceneName);
            SceneManager.Instance.SetNextScene((SceneType) Enum.Parse(typeof(SceneType), sceneName));
        }
        else if (lifeCycleName == "finish")
        {
            SceneHelper.Instance.ClosePage(pageName);
        }
        else if (lifeCycleName == "onResume")
        {
            string resumceSceneName = SceneHelper.Instance.ResumeScene(pageName);
            if (SceneManager.Instance.CurrentSceneType.ToString() != resumceSceneName)
            {
                SceneManager.Instance.SetNextScene((SceneType) Enum.Parse(typeof(SceneType), resumceSceneName));
                Debug.Log("LISHICONG TestRun OnNativeLifeCycle onResume:" + resumceSceneName);
            }
        }

        SendPageStackToNative();
    }

    public void OnNativeBackEvent(string jsonStr)
    {
        Debug.Log("LISHICONG TestRun OnNativeBackEvent jsonStr:" + jsonStr);

        bool u3dCanBack = SceneHelper.Instance.CanBackScene();
        if (u3dCanBack)
        {
            SceneManager.Instance.SetNextScene(SceneHelper.Instance.BackScene());
            SendPageStackToNative(); // 打印页面栈信息
        }

        SendNativeBackEvent(u3dCanBack);
    }

    public void SendNativeBackEvent(bool u3dCanBack)
    {
        JObject staff = new JObject();
        staff.Add(new JProperty("isIntercept", u3dCanBack));

        string result = staff.ToString();
        JoCall("OnUnityBackEvent", result);

        Debug.Log("LISHICONG TestRun SendNativeBackEvent result:" + result);
    }

    public void SendPageStackToNative()
    {
        StartCoroutine(DelayToInvoke.DelayToInvokeDo(() =>
        {
            string stackInfo = SceneHelper.Instance.GetSceneStackInfo();
            Debug.Log("LISHICONG TestRun Stack:" + stackInfo);
            // 输出日志到NA屏幕
            JoCall("SendPageStackToNative", stackInfo);
        }, 0.5f));
    }

    private void JoCall(string nativeMethod, object obj)
    {
        AndroidJavaClass jc;
        AndroidJavaObject jo;
        try
        {
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call(nativeMethod, obj);
        }
        catch
        {
            Debug.LogError("LISHICONG TestRun jo call error on method:" + nativeMethod);
        }

        Debug.Log("LISHICONG TestRun jo call on method:" + nativeMethod);
    }
}