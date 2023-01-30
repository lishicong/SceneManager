using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SceneHelper : MonoBehaviourSingleton<SceneHelper>
{
    [SerializeField] private List<SceneStackData> sceneNameList = new();

    // 创建新页面，打开一个场景
    public void OpenPage(string pageName, string sceneName)
    {
        SceneStackData sceneStackData = new SceneStackData();
        sceneStackData.AddSceneStack(pageName, sceneName);
        sceneNameList.Add(sceneStackData);
    }

    // 关闭页面
    public void ClosePage(string pageName)
    {
        int len = sceneNameList.Count;
        for (int i = len - 1; i >= 0; i--)
        {
            if (sceneNameList[i].pageName == pageName)
            {
                sceneNameList.RemoveRange(i, len - i);
                break;
            }
        }
    }

    // U3D 打开一个场景
    public void OpenScene(string sceneName)
    {
        SceneStackData sceneStackData = sceneNameList.Last();
        sceneStackData.AddScene(sceneName);
    }

    public string ResumeScene(string pageName)
    {
        string sceneName = "";
        int len = sceneNameList.Count;
        for (int i = len - 1; i >= 0; i--)
        {
            if (sceneNameList[i].pageName == pageName)
            {
                sceneName = sceneNameList[i].scenes.Last().sceneName;
                break;
            }
        }

        return sceneName;
    }

    public SceneType BackScene()
    {
        sceneNameList.Last().scenes.RemoveAt(sceneNameList.Last().scenes.Count - 1);
        string sceneName = sceneNameList.Last().scenes.Last().sceneName;
        return (SceneType) Enum.Parse(typeof(SceneType), sceneName);
    }

    public bool CanBackScene()
    {
        return sceneNameList.Last().scenes.Count > 1;
    }

    public object GetSceneData(string sceneName)
    {
        return sceneNameList.Last().scenes.Last().sceneData;
    }

    public string GetSceneStackInfo()
    {
        StringBuilder sbf = new StringBuilder();
        foreach (var sceneStackData in sceneNameList)
        {
            sbf.Append("NA页面：" + sceneStackData.pageName + "\n");
            sbf.Append("U3D场景：");
            foreach (var scenes in sceneStackData.scenes)
            {
                sbf.Append(scenes.sceneName + " | ");
            }

            sbf.Append("\n-----------------------------------------------\n");
        }

        return sbf.ToString();
    }

    class SceneStackData
    {
        public string pageName;
        public List<SceneData> scenes = new();

        public void AddSceneStack(string page, string scene)
        {
            pageName = page;
            AddScene(scene);
        }

        public void AddScene(string scene)
        {
            SceneData sceneData = new();
            sceneData.sceneName = scene;
            scenes.Add(sceneData);
        }
    }

    class SceneData
    {
        public string sceneName;
        public object sceneData;
    }
}