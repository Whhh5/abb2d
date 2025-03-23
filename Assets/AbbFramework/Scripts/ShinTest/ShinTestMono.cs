using UnityEngine;
using System;

[Serializable]
public class ShinTestData
{
    public SphereCollider col;
    public Vector3 posl;
}
public class ShinTestMono : MonoBehaviour
{
    public ShinTestData _LeftHandData;
    public ShinTestData _RightHandData;
    public ShinTestData _LeftFootData;
    public ShinTestData _RightFootData;


    private void Update()
    {
        //var result = Physics.SphereCast(
        //    _LeftHandData.col.transform.position
        //    , _LeftHandData.col.radius
        //    , new Vector3(1, 1, 1)
        //    , out var hit
        //    , _LeftHandData.col.radius
        //    , 1 << (int)EnGameLayer.Terrain
        //    , QueryTriggerInteraction.UseGlobal);
        //DebugDrawMgr.Instance.DrawSphere(_LeftHandData.col.transform.position, _LeftHandData.col.radius, Time.deltaTime);
        //if (result)
        //{
        //    DebugDrawMgr.Instance.DrawSphere(hit.point, 0.2f, 0.1f);
        //}
    }
}
