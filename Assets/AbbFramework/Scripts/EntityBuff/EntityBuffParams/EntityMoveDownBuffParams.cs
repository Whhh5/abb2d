using UnityEngine;

public class EntityMoveDownBuffParams : IEntityBuffParams
{
    public float value;

    public void OnPoolDestroy()
    {
        value = -1;
    }

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
