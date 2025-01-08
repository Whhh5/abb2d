using UnityEngine;
using System.Collections;


public interface IPoolUserData
{
    //public static PoolNaNUserData NaN;
}
public interface IGamePool
{
    public void PoolConstructor();
    public void OnPoolInit<T>(ref T userData) where T : struct, IPoolUserData;
    public void OnPoolEnable();

    public void OnPoolDestroy();

    public void PoolRelease();
}

