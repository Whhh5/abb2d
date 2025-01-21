public interface IPhysicsParams
{
    public EnPhysicsType GetPhysicsType();
    public int[] GetPhysicsParams();
}



public interface IPhysicsColliderCallbackCustomData
{

}


public interface IPhysicsResolve
{
    public void SetParams(ref int[] arrParams, int startIndex);
    public void Execute(int entityID, int layer, PhysicsColliderCallback callback, IPhysicsColliderCallbackCustomData cusomData);
}