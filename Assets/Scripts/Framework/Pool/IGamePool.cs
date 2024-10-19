using UnityEngine;
using System.Collections;

public interface IGamePool
{
    public EnClassType ClassType { get; }
    public bool Constructor()
    {
        return true;
    }

    public bool OnPull()
    {
        return true;
    }

    public bool OnPush()
    {
        return true;
    }

    public bool Release()
    {
        return true;
    }
}

