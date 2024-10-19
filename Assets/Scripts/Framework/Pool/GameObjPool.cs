using UnityEngine;
using System.Collections;

public abstract class GameObjPool : MonoBehaviour, IGamePool
{
    public EnClassType ClassType => EnClassType.GameObjPool;
}

