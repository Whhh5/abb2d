using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerEntityData : Entity3DData
{
    private PlayerEntity m_PlayerEntity => m_Entity as PlayerEntity;
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_PlayerEntity;


    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        CameraMgr.Instance.SetLookAtTran(m_GOID);
    }
    public override void OnGODestroy()
    {
        CameraMgr.Instance.ClearLookAt();
        base.OnGODestroy();
    }



}
public class PlayerEntity : Entity3D, IEntity3DCCCom
{
    private PlayerEntityData m_PlayerData = null;
    [SerializeField]
    private CharacterController m_CharacterController = null;
    [SerializeField]
    private Trigger3D m_IsGroundTrigger = null;
    [SerializeField]
    private Transform m_TopPoint = null;
    public override void OnUnload()
    {
        m_PlayerData = null;
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        m_PlayerData = m_EntityData as PlayerEntityData;

        m_CharacterController.Move(m_EntityData.WorldPos - transform.position);
    }
    public CharacterController GetCC()
    {
        return m_CharacterController;
    }
    public override void SetPosition()
    {

    }

    public bool IsGrounded()
    {
        return m_IsGroundTrigger.IsEnter();
    }

    private bool m_IsClimb = false;
    private void OnDrawGizmos()
    {


        //var dir = m_BodyObj.transform.forward;
        //var right = m_BodyObj.transform.right;
        //var up = m_BodyObj.transform.up;
        //var startPoint = m_TopPoint.position;
        //var distance = 1f;
        //var interval = 0.1f;
        //var collision = new List<RaycastHit>(100);
        //for (int x = -5; x <= 5; x++)
        //{
        //    var posX = right * interval * x;
        //    for (int y = -10; y <= 10; y++)
        //    {

        //        var posY = up * interval * y;
        //        var pos = startPoint + posY + posX;

        //        var ray = new Ray(pos, dir);
        //        var hint = Physics.RaycastAll(ray, distance);
        //        collision.AddRange(hint);
        //        Debug.DrawRay(pos, dir * distance, Color.red);
        //    }
        //}
        //// 找出最远的一组点
        //var dis = -1f;
        //RaycastHit hit = default;
        //foreach (var item in collision)
        //{
        //    var dis2 = item.distance;
        //    if (item.distance < dis)
        //        continue;
        //    hit = item;
        //    dis = item.distance;
        //}
        ////
        //if (dis > 0)
        //{
        //    m_IsClimb = true;
        //    Gizmos.DrawSphere(hit.point, 0.3f);
        //}
        //else
        //{
        //    m_IsClimb = false;
        //}
    }
}
