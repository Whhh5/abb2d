using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadMgr : Singleton<LoadMgr>
{
    //private Dictionary<EnLoadTarget, >
    public async UniTask<T> LoadAsync<T>(EnLoadTarget loadTarget)
        where T : Object
    {
        var path = LoadConfig.Instance.GetTargetPath(loadTarget);
        var obj = await Resources.LoadAsync(path);
        return obj as T;
    }
    public void Unload<T>(T target)
    {
        
    }
}
