using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityMonitorEntity: IGamePool
{
    public void StartMonitor(Entity3DData entityData);
    public void StopMonitor();
}
