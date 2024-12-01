using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABBInputMgr : Singleton<ABBInputMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private Vector3 m_LastMoursePos = Vector3.zero;
    public override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0))
        {
            m_LastMoursePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_LastMoursePos = Vector3.zero;
        }
        if (Input.GetMouseButton(0))
        {
            var delta = Input.mousePosition - m_LastMoursePos;
            PlayerMgr.Instance.IncrementMovePlayer(new Vector3(delta.x, 0, 0) * Time.deltaTime);
            m_LastMoursePos = Input.mousePosition;
        }
    }
}
