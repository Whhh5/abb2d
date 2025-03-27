
public interface IEntityBuffParams : IClassPoolDestroy
{


}

public interface IEntityBuffParams<T> : IEntityBuffParams, IClassPool<T>
    where T : class, IClassPoolUserData
{


}