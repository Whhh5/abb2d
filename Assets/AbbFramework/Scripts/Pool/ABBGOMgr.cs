using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GOData : IGamePool
{
    private EnLoadTarget m_LoadTarget;
    private GameObject m_GO;
    public void OnPoolDestroy()
    {
        m_LoadTarget = EnLoadTarget.None;
        m_GO = null;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit<T>(ref T userData) where T : struct, IPoolUserData
    {
        if (userData is not GODataUserData data)
            return;
        m_GO = data.go;
        m_LoadTarget = data.target;
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }

    public EnLoadTarget GetLoadTarget()
    {
        return m_LoadTarget;
    }
    public GameObject GetGameObject()
    {
        return m_GO;
    }
}
public class ABBGOMgr : Singleton<ABBGOMgr>
{
    private Dictionary<int, GOData> m_GOMap = new();
    private GODataUserData m_CreateGOUserData = new();
    public GameObject GetGo(int goID)
    {
        if (!m_GOMap.TryGetValue(goID, out var goData))
            return null;
        return goData.GetGameObject();
    }
    public T GetGoCom<T>(int goID)
        where T : Component
    {
        var go = GetGo(goID);
        var com = go.GetComponent<T>();
        return com;
    }
    public async UniTask<int> CreateGOAsync(EnLoadTarget target, Transform parent = null)
    {
        var obj = await ABBLoadMgr.Instance.LoadAsync<GameObject>(target);
#if UNITY_EDITOR
        if (obj == null)
            return -1;
#endif
        var goID = ABBUtil.GetTempKey();
        var ins = GameObject.Instantiate(obj, parent);

        m_CreateGOUserData.go = ins;
        m_CreateGOUserData.target = target;

        var goData = ClassPoolMgr.Instance.Pull<GOData, GODataUserData>(ref m_CreateGOUserData);

        m_GOMap.Add(goID, goData);
        return goID;
    }
    public void DestroyGO(int goID)
    {
        if (!m_GOMap.TryGetValue(goID, out var goData))
            return;
        GameObject.Destroy(goData.GetGameObject());
        var loadTarget = goData.GetLoadTarget();
        m_GOMap.Remove(goID);
        ClassPoolMgr.Instance.Push(goData);
        ABBLoadMgr.Instance.Unload(loadTarget);

    }
}

