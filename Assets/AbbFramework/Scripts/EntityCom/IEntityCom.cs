using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity3DComData : IGamePool
{
    public void OnCreateGO(Entity3D entity);
    public void OnDestroyGO();
}

public interface IEntity3DCom
{
    
}
