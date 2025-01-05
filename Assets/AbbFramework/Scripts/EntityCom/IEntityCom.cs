using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity3DComData : IGamePool
{
    public void AddCom(Entity3DData entity3DData);
    public void RemomveCom();
    public void OnCreateGO(Entity3D entity);
    public void OnDestroyGO();
}

public interface IEntity3DCom
{
    
}
