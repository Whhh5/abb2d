using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GOData : IGamePool
{
	public EnLoadTarget loadTarget;
	public GameObject go;
	public void OnPoolDestroy()
	{
		loadTarget = EnLoadTarget.None;
		go = null;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(CustomPoolData userData)
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}
public class ABBGOMgr : Singleton<ABBGOMgr>
{
	private Dictionary<int, GOData> m_GOMap = new();
	public GameObject GetGo(int goID)
	{
		if (!m_GOMap.TryGetValue(goID, out var goData))
			return null;
		return goData.go;
	}
    public T GetGoCom<T>(int goID)
		where T: Component
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
		var goData = GameClassPoolMgr.Instance.Pull<GOData>();
		goData.go = ins;
		goData.loadTarget = target;
        m_GOMap.Add(goID, goData);
        return goID;
    }
	public void DestroyGO(int goID)
	{
		if (!m_GOMap.TryGetValue(goID, out var goData))
			return;
		GameObject.Destroy(goData.go);
		var loadTarget = goData.loadTarget;
		m_GOMap.Remove(goID);
		GameClassPoolMgr.Instance.Push(goData);
        ABBLoadMgr.Instance.Unload(loadTarget);

    }
}

