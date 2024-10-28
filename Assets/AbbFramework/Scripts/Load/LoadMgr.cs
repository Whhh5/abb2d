using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadMgr : Singleton<LoadMgr>
{
    //private Dictionary<EnLoadTarget, >
    public async UniTask<Object> LoadAsync(string assetPath)
    {
        //await UniTask.DelayFrame(2);
        //var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        var asset = await Resources.LoadAsync(assetPath);
        return asset;
    }
    public async UniTask<T> LoadAsync<T>(EnLoadTarget loadTarget)
        where T : Object
    {
        var path = LoadConfig.Instance.GetTargetPath(loadTarget);
        var obj = await LoadAsync(path);
        return obj as T;
    }
    public T Load<T>(EnLoadTarget loadTarget)
        where T : Object
    {
        var path = LoadConfig.Instance.GetTargetPath(loadTarget);
        var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        return obj as T;
    }
    public void Unload<T>(T target)
    {
        
    }
}
