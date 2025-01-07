using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

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

        public  void OnPoolInit(CustomPoolData userData)
        {
            m_TokenSource = new();
        }

        public void OnPoolDestroy()
        {
            m_TokenSource.Cancel();
            m_TokenSource = null;
            m_Status = EnLoadStatus.Start;
            m_ObjID = -1;
            m_RefCount = 0;
        }

        public void PoolConstructor()
        {
        }

        public void OnPoolEnable()
        {
        }

        public void PoolRelease()
        {
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
    private Dictionary<int, LoadData> m_LoadDataCache = new();
    private Dictionary<EnLoaderType, IABBAssetLoader> m_LoaderList = new();
    private CancellationTokenSource m_LoadTokenSource = new();
    private Dictionary<int, LoadConfigItem> m_DicPrefabPath = new();

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
    private bool ContainsLoadData(int assetID)
    {
        if (!m_LoadDataCache.ContainsKey(assetID))
            return false;
        return true;
    }
    private IABBAssetLoader GetLoader(EnLoaderType loadType)
    {
        return m_LoaderList[loadType];
    }
    private IABBAssetLoader GetLoader()
    {
#if UNITY_EDITOR
        //return new ABBEditorLoader();
        return GetLoader(EnLoaderType.Editor);
#else
        return null;
#endif
    }
    private LoadData GetLoadData(int assetID)
    {
        if (!m_LoadDataCache.TryGetValue(assetID, out var loadData))
        {
            loadData = GameClassPoolMgr.Instance.Pull<LoadData>();
            m_LoadDataCache.Add(assetID, loadData);
        }
        return loadData;
    }
    private void RemoveLoadData(int assetID)
    {
        var loadData = GetLoadData(assetID);
        GameClassPoolMgr.Instance.Push(loadData);
        m_LoadDataCache.Remove(assetID);
    }
    private async UniTask<T> LoadAsync<T>(string assPath)
        where T : Object
    {
        var objID = await LoadAsync<T>(assPath, m_LoadTokenSource);
        var asset = GetLoader().GetObject(objID);
        return asset as T;
    }
    public async UniTask<T> LoadAsync<T>(EnLoadTarget loadTarget)
        where T : Object
    {
        var assCfg = GameSchedule.Instance.GetAssetCfg0((int)loadTarget);
        var objID = await LoadAsync<T>(assCfg.strPath, m_LoadTokenSource);
        var asset = GetLoader().GetObject(objID);
        return asset as T;
    }
    private async UniTask<int> LoadAsync<T>(string assPath, CancellationTokenSource tokenSource)
        where T : Object
    {
        var loader = GetLoader();
        var objID = await loader.LoadAssetAsync<T>(assPath, tokenSource);
        return objID;
    }
    public T Load<T>(int assetID)
           where T : Object
    {
        var assetCfg = GameSchedule.Instance.GetAssetCfg0(assetID);
        var loadData = GetLoadData(assetID);
        if (loadData.IsStatus(EnLoadStatus.Start))
        {
            loadData.SetLoadStatus(EnLoadStatus.Loading);
            var loader = GetLoader();
            var path = assetCfg.strPath;
            var objID = loader.LoadAsset<T>(path);
            if (objID < 0)
            {
                loadData.SetLoadStatus(EnLoadStatus.Failed);
                ABBUtil.LogError($"load failed, assetID: {assetID}, path: {path}");
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
        Unload((int)loadTarget);
    }
    public void Unload(int assetID)
    {
        if (!ContainsLoadData(assetID))
        {
            ABBUtil.LogError("unload no exist");
            return;
        }
        var loadData = GetLoadData(assetID);
        loadData.CancelRef();
        if (loadData.GetRefCount() == 0)
        {
            if (loadData.IsStatus(EnLoadStatus.Success))
            {
                var objID = loadData.GetObjID();
                // 对资源进行处理, 卸载资源，卸载 assetbundle
                var loader = GetLoader();
                loader.UnloadAsset(objID);
            }
            RemoveLoadData(assetID);
        }
    }
}
