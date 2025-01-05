using UnityEngine;

public class DebugDrawSphereData : DebugDrawData
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_DrawSphere;
}


public class DebugDrawSphere : Entity3D
{

}