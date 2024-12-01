using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public abstract class Singleton
{
    public virtual EnManagerFuncType FuncType => EnManagerFuncType.AwakeAsync | EnManagerFuncType.OnEnableAsync;
    public virtual async UniTask AwakeAsync()
    {
        await UniTask.DelayFrame(1);
    }
    public virtual async UniTask OnEnableAsync()
    {
        await UniTask.DelayFrame(1);
    }
    public virtual void OnDisable()
    {

    }
    public virtual void Destroy()
    {

    }
    public virtual void Update()
    {

    }
}
public abstract class Singleton<T> : Singleton
    where T : class, new()
{
    public static T Instance = new();
}

