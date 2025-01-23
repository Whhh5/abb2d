using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity3DComData : IClassPool
{
    public void OnCreateGO(int entityID);
    public void OnDestroyGO(int entityID);
}
public interface IEntity3DComData<T> : IEntity3DComData, IClassPool<T>
    where T: class, IClassPoolUserData
{

}


public interface IEntity3DCom
{
    
}
