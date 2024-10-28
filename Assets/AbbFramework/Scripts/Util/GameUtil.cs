using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public class GameUtil : Singleton<GameUtil>
{
    public static T PullClass<T>()
        where T : class, IGamePool, new()
    {
        var result = new T();
        result.Constructor();
        result.OnPull();
        return result;
    }
    public static void PushClass<T>(T classData)
        where T : class, IGamePool
    {
        classData.OnPush();
        classData.Release();
    }
    public async UniTask<T> LoadGameobjectAsync<T>(EnLoadTarget loadTarget)
        where T : GameObjPool
    {
        var obj = await LoadMgr.Instance.LoadAsync<GameObject>(loadTarget);
        var ins = GameObject.Instantiate(obj);
        var com = ins.GetComponent<T>();
        return com;
    }
    public void UnloadGameobject<T>(T target)
        where T : GameObjPool
    {
        //LoadMgr.Instance.Unload(target);
        GameObject.Destroy(target.gameObject);
    }
}

