using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityMonitorEntity: IClassPool
{
    public void StartMonitor(Entity3DData entityData);
    public void StopMonitor();
}
public interface IEntityMonitorEntity<T> : IEntityMonitorEntity, IClassPool<T>
    where T : class, IClassPoolUserData

{
}