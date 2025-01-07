using UnityEngine;
using System.Collections;


public abstract class CustomPoolData : IGamePool
{
    public abstract void OnPoolDestroy();

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
public interface IGamePool
{
    public void PoolConstructor();
    public void OnPoolInit(CustomPoolData userData);
    public void OnPoolEnable();

    public void OnPoolDestroy();

    public void PoolRelease();
}

