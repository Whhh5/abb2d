using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ABBLoadMgr : Singleton<ABBLoadMgr>
{
    public enum EnLoaderType
    {
        Editor,
    }
    public class LoadData : IGamePool
    {
        private int m_ObjID = -1;
        private int m_RefCount = 0;
        public EnLoadStatus m_Status = EnLoadStatus.Start;
        private CancellationTokenSource m_TokenSource = null;

        public void OnPoolGet()
        {
            m_TokenSource = new();
        }

        public void OnPoolRecycle()
        {
            m_TokenSource.Cancel();
            m_TokenSource = null;
            m_Status = EnLoadStatus.Start;
            m_ObjID = -1;
            m_RefCount = 0;
        }

        public void SetObjID(int objID)
        {
            m_ObjID = objID;
        }
        public int GetObjID()
        {
            return m_ObjID;
        }
        public T GetObj<T>()
            where T : Object
        {
            m_RefCount++;
            var loader = Instance.GetLoader();
            var obj = loader.GetObject(m_ObjID);
            return obj as T;
        }
        public void SetLoadStatus(EnLoadStatus loadStatus)
        {
            m_Status = loadStatus;
        }
        public bool IsLoadFinish()
        {
            return IsStatused(EnLoadStatus.Finish);
        }
        public bool IsStatused(EnLoadStatus loadStatus)
        {
            return m_Status >= loadStatus;
        }
        public bool IsStatus(EnLoadStatus loadStatus)
        {
            return loadStatus == m_Status;
        }
        public CancellationTokenSource GetTokenSource()
        {
            return m_TokenSource;
        }
        public void CancelRef()
        {
            m_RefCount--;
        }
        public int GetRefCount()
        {
            return m_RefCount;
        }
    }
    private Dictionary<EnLoadTarget, LoadData> m_LoadCache = new();
    private Dictionary<EnLoaderType, IABBAssetLoader> m_LoaderList = new();
    private CancellationTokenSource m_LoadTokenSource = new();
    private Dictionary<EnLoadTarget, LoadConfigItem> m_DicPrefabPath = new();

    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();
    }
    public override void Destroy()
    {
        base.Destroy();
        UnregisterLoader<ABBEditorLoader>(EnLoaderType.Editor);
    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();
        RegisterLoader<ABBEditorLoader>(EnLoaderType.Editor);

        var str = await LoadAsync<TextAsset>(GlobalConfig.LoadConfigRecordsJson);
        var target = JsonConvert.DeserializeObject<List<LoadConfigItem>>(str.text);
        foreach (var item in target)
        {
            m_DicPrefabPath.Add(item.LoadTarget, item);
        }
    }
    private void RegisterLoader<T>(EnLoaderType loadType)
        where T : IABBAssetLoader, new()
    {
        m_LoaderList.Add(loadType, new T());
    }
    private void UnregisterLoader<T>(EnLoaderType loadType)
        where T : IABBAssetLoader, new()
    {
        m_LoaderList.Remove(loadType);
    }
    private string GetAssetPath(EnLoadTarget target)
    {
        if (!m_DicPrefabPath.TryGetValue(target, out var data))
            return null;
        return data.Path;
    }
    private IABBAssetLoader GetLoader(EnLoaderType loadType)
    {
        return m_LoaderList[loadType];
    }
    private IABBAssetLoader GetLoader()
    {
#if UNITY_EDITOR
        return GetLoader(EnLoaderType.Editor);
#else
        return null;
#endif
    }
    private LoadData GetLoadData(EnLoadTarget loadTarget)
    {
        if (!m_LoadCache.TryGetValue(loadTarget, out var loadData))
        {
            loadData = GameClassPoolMgr.Instance.Pull<LoadData>();
            m_LoadCache.Add(loadTarget, loadData);
        }
        return loadData;
    }
    private bool ContainsLoadData(EnLoadTarget loadTarget)
    {
        var result = m_LoadCache.ContainsKey(loadTarget);
        return result;
    }
    private void RemoveLoadData(EnLoadTarget loadTarget)
    {
        var loadData = GetLoadData(loadTarget);
        GameClassPoolMgr.Instance.Push(loadData);
        m_LoadCache.Remove(loadTarget);
    }
    private async UniTask<T> LoadAsync<T>(string assPath)
        where T : Object
    {
        var objID = await LoadAsync<T>(assPath, m_LoadTokenSource);
        var asset = GetLoader().GetObject(objID);
        return asset as T;
    }
    private async UniTask<int> LoadAsync<T>(string assPath, CancellationTokenSource tokenSource)
        where T : Object
    {
        var loader = GetLoader();
        var objID = await loader.LoadAssetAsync(assPath, tokenSource);
        return objID;
    }
    public async UniTask<T> LoadAsync<T>(EnLoadTarget loadTarget)
        where T : Object
    {
        var loadData = GetLoadData(loadTarget);
        if (loadData.IsStatus(EnLoadStatus.Start))
        {
            loadData.SetLoadStatus(EnLoadStatus.Loading);
            var path = GetAssetPath(loadTarget);
            var objID = await LoadAsync<Object>(path, loadData.GetTokenSource());
            if (objID < 0)
            {
                loadData.SetLoadStatus(EnLoadStatus.Failed);
                ABBUtil.LogError($"load failed, target: {loadTarget}, path: {path}");
            }
            else
            {
                loadData.SetLoadStatus(EnLoadStatus.Success);
                loadData.SetObjID(objID);
            }
        }
        if (!loadData.IsLoadFinish())
            await UniTask.WaitUntil(loadData.IsLoadFinish);
        if (!loadData.IsStatus(EnLoadStatus.Success))
            return null;
        var obj = loadData.GetObj<T>();
        return obj;
    }
    public T Load<T>(EnLoadTarget loadTarget)
           where T : Object
    {
        var loadData = GetLoadData(loadTarget);
        if (loadData.IsStatus(EnLoadStatus.Start))
        {
            loadData.SetLoadStatus(EnLoadStatus.Loading);
            var path = GetAssetPath(loadTarget);
            var loader = GetLoader();
            var objID = loader.LoadAsset(path);
            if (objID < 0)
            {
                loadData.SetLoadStatus(EnLoadStatus.Failed);
                ABBUtil.LogError($"load failed, target: {loadTarget}, path: {path}");
            }
            else
            {
                loadData.SetLoadStatus(EnLoadStatus.Success);
                loadData.SetObjID(objID);
            }
        }
        if (!loadData.IsStatus(EnLoadStatus.Success))
            return null;
        var result = loadData.GetObj<T>();
        return result;
    }
    public void Unload(EnLoadTarget loadTarget)
    {
        if(!ContainsLoadData(loadTarget))
        {
            ABBUtil.LogError("unload no exist");
            return;
        }
        var loadData = GetLoadData(loadTarget);
        loadData.CancelRef();
        if(loadData.GetRefCount() == 0)
        {
            if(loadData.IsStatus(EnLoadStatus.Success))
            {
                var objID = loadData.GetObjID();
                // 对资源进行处理, 卸载资源，卸载 assetbundle
                var loader = GetLoader();
                loader.UnloadAsset(objID);
            }
            RemoveLoadData(loadTarget);
        }
    }
}
