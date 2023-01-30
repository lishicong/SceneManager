using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景类型
/// </summary>
public enum SceneType
{
    None,
    Scene1,
    Scene2,
    Scene3,
    Back,
}

/// <summary>
/// 场景切换
/// </summary>
public class SceneManager : MonoBehaviourSingleton<SceneManager>
{
    /// <summary>
    /// 场景导入状态
    /// </summary>
    private enum Phase
    {
        None,
        ScreenOutWait,
        DestroyWait,
        LoadSceneWait,
        InitializeWait,
        ScreenInWait,
    }

    public bool IsInitialized { get; private set; } = false;
    public bool IsChanging { get; private set; } = false;
    public SceneController ActiveScene { get; private set; } = null;
    public TransitionEffectType EffectType { get; set; } = TransitionEffectType.None;
    public SceneType CurrentSceneType { get; private set; } = SceneType.None;
    public SceneType NextSceneType { get; private set; } = SceneType.None;

    [SerializeField] private bool isScenePrefab = false;
    [SerializeField] private List<string> sceneNameList = new List<string>();
    [SerializeField] private List<GameObject> scenePrefabList = new List<GameObject>();

    private Phase phase_ = Phase.None;
    private System.Object passData_ = null;

    //Awake()是在脚本对象实例化时被调用的，而Start()是在对象的第一帧时被调用的，而且是在Update()之前
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        // 从启动时的场景中查找场景控制器
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject rootObject in rootObjects)
        {
            ActiveScene = rootObject.GetComponent<SceneController>();
            if (ActiveScene != null)
            {
                Debug.Log("找到场景控制器ActiveScene");
                break;
            }
            else
            {
                Debug.LogError("未找到场景控制器ActiveScene");
            }
        }

        SceneType nextSceneType = SceneType.None;
        if (isScenePrefab)
        {
            // 查找具有相同预制件名称的场景
            foreach (SceneType sceneType in Enum.GetValues(typeof(SceneType)))
            {
                GameObject prefab = scenePrefabList[(int) sceneType];
                if (prefab != null && prefab.name == ActiveScene.name)
                {
                    nextSceneType = sceneType;
                    break;
                }
            }
        }
        else
        {
            // 查找场景名称相同的场景
            foreach (SceneType sceneType in Enum.GetValues(typeof(SceneType)))
            {
                if (sceneNameList[(int) sceneType] == scene.name)
                {
                    nextSceneType = sceneType;
                    break;
                }
            }
        }

        if (nextSceneType != SceneType.None)
        {
            // 场景初始化
            SetNextScene(nextSceneType, TransitionEffectType.None);
        }
        else
        {
            // 场景导入失败
            Debug.Log("正在播放未在SceneManager中注册的场景");
        }
    }

    /// <summary>
    /// 切换目标场景设置
    /// </summary>
    /// <param name="sceneType"></param>
    /// <param name="transitionEffectType"></param>
    /// <returns></returns>
    public void SetNextScene(SceneType sceneType, TransitionEffectType transitionEffectType = TransitionEffectType.None)
    {
        // 切换中不变更切换目的地
        if (IsChanging)
        {
            return;
        }

        // 无场景指定无效
        if (sceneType == SceneType.None)
        {
            return;
        }

        Debug.Log("LISHICONG SceneManager SetNextScene:" + sceneType);
        // 场景过渡开始
        StartCoroutine(ChangeScene(sceneType, transitionEffectType));
    }

    /// <summary>
    /// 设置要传递给其他场景的数据
    /// </summary>
    public void SetPassData(System.Object data)
    {
        passData_ = data;
    }

    /// <summary>
    /// 获取另一场景保存的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetPassData<T>()
    {
        if (passData_ != null && (passData_ is T))
        {
            return (T) passData_;
        }

        return default(T);
    }

    /// <summary>
    /// 获取场景预制件
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetPrefab(SceneType type)
    {
        if ((int) type < scenePrefabList.Count)
        {
            return scenePrefabList[(int) type];
        }

        return null;
    }

    /// <summary>
    /// 获取场景名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetSceneName(SceneType type)
    {
        if ((int) type < sceneNameList.Count)
        {
            return sceneNameList[(int) type];
        }

        return "";
    }

    /// <summary>
    /// 场景导入
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator LoadScene(SceneType sceneType)
    {
        // 导入下一个场景
        yield return null;
        AsyncOperation asyncOperation =
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GetSceneName(sceneType));
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        // 初始化下一个场景的SceneController
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject rootObject in rootObjects)
        {
            ActiveScene = rootObject.GetComponent<SceneController>();
            if (ActiveScene != null)
            {
                break;
            }
        }
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeScene(SceneType nextSceneType, TransitionEffectType transitionEffectType)
    {
        // 切换开始
        IsChanging = true;

        EffectType = transitionEffectType;
        NextSceneType = nextSceneType;

        if (CurrentSceneType != SceneType.None)
        {
            // 屏蔽
            phase_ = Phase.ScreenOutWait;
            TransitionManager.Instance.StartTransition(transitionEffectType, TransitionState.ScreenOut,
                () => { phase_ = Phase.DestroyWait; });
            if (ActiveScene != null)
            {
                ActiveScene.StartedSceneChange();
            }

            while (phase_ == Phase.ScreenOutWait)
            {
                yield return null;
            }

            // 等待场景删除处理
            if (ActiveScene != null)
            {
                // 等待删除
                ActiveScene.Destory();
                while (!ActiveScene.IsDestroyed)
                {
                    yield return null;
                }

                // 删除
                if (isScenePrefab)
                {
                    Destroy(ActiveScene.gameObject);
                }

                ActiveScene = null;
            }

            // 导入下一个场景
            phase_ = Phase.LoadSceneWait;
            if (isScenePrefab)
            {
                // 预制装配式装入
                GameObject resources = GetPrefab(NextSceneType);
                if (resources != null)
                {
                    GameObject scene = GameObject.Instantiate(resources);
                    ActiveScene = scene.GetComponent<SceneController>();
                }
                else
                {
                    Debug.LogError(String.Format("{0} 场景不存在", NextSceneType));
                    yield break;
                }
            }
            else
            {
                // 场景导入
                yield return LoadScene(NextSceneType);
            }
        }

        CurrentSceneType = NextSceneType;
        NextSceneType = SceneType.None;

        // 等待初始化
        if (ActiveScene != null)
        {
            phase_ = Phase.InitializeWait;
            ActiveScene.Initialize();
            while (!ActiveScene.IsInitialized)
            {
                yield return null;
            }
        }

        // 屏幕输入
        phase_ = Phase.ScreenInWait;
        TransitionManager.Instance.StartTransition(EffectType, TransitionState.ScreenIn,
            () => { phase_ = Phase.None; });
        while (phase_ == Phase.ScreenInWait)
        {
            yield return null;
        }

        // 屏幕进入完成
        if (ActiveScene != null)
        {
            ActiveScene.FinishedSceneChange();
        }

        // 切换完成
        IsChanging = false;
    }
}