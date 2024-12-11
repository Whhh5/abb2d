using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

public class ABBEditorLoader : IABBAssetLoader
{
    private Dictionary<int, Object> m_ID2Object = new();
    private int AddObject(Object obj)
    {
        var key = ABBUtil.GetTempKey();
        m_ID2Object.Add(key, obj);
        return key;
    }
    private bool ContainsAsset(int objID)
    {
        var result = m_ID2Object.ContainsKey(objID);
        return result;
    }

    public int LoadAsset<T>(string assetPath)
        where T : Object
    {
        var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
#if UNITY_EDITOR
        if (obj == null)
            return -1;
#endif
        var key = AddObject( obj);
        return key;
    }

    public async UniTask<int> LoadAssetAsync<T>(string assetPath, CancellationTokenSource cancellation)
        where T : Object
    {
        var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
        var delay = ABBUtil.GetRange(1,3);
        await UniTask.DelayFrame(delay, cancellationToken: cancellation.Token);
#if UNITY_EDITOR
        if (obj == null)
            return -1;
#endif
        var key = AddObject(obj);
        return key;
    }

    public void UnloadAsset(int objID)
    {
#if UNITY_EDITOR
        if (!ContainsAsset(objID))
        {
            ABBUtil.LogError($"try unload no exist asset id:{objID}");
            return;
        }
#endif
        m_ID2Object.Remove(objID);
    }

    public Object GetObject(int objID)
    {
#if UNITY_EDITOR
        if(!ContainsAsset(objID))
        {
            ABBUtil.LogError($"try get no exist asset id:{objID}");
            return null;
        }
#endif
        return m_ID2Object[objID];
    }
}

