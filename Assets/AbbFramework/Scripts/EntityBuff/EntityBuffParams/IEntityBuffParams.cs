
public interface IEntityBuffParams : IClassPool
{


}

public interface IEntityBuffParams<T> : IEntityBuffParams, IClassPool<T>
    where T : class, IClassPoolUserData
{


}