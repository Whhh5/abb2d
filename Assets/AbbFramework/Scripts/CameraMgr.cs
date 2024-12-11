using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class CameraMgr : SingletonMono<CameraMgr>
{
    [SerializeField]
    private Camera m_MainCamera;
    [SerializeField]
    private UniversalAdditionalCameraData m_URPCamera = null;
    [SerializeField]
    private CinemachineVirtualCamera m_VirtualCamera = null;
    private int m_LookAtDataGoID = -1;
    private CameraLookAtData m_LookAtData = null;


    protected override void Start()
    {
        base.Start();
        m_VirtualCamera.ForceCameraPosition(new Vector3(-74.35f, 4.6f, -10), new Quaternion(0, 0, 0, 0));
    }
    protected override void OnDestroy()
    {
        m_LookAtDataGoID
            = -1;
        m_MainCamera = null;
        m_URPCamera = null;
        m_VirtualCamera = null;
        base.OnDestroy();
    }
    public void SetLookAtTran(int goID)
    {
        m_LookAtDataGoID = goID;
        if (m_VirtualCamera != null)
        {
            var goLockAtData = ABBGOMgr.Instance.GetGoCom<CameraLookAtData>(goID);
            m_LookAtData = goLockAtData;
            m_VirtualCamera.LookAt = goLockAtData.lookAtTran;
            m_VirtualCamera.Follow = goLockAtData.followTran;
        }
    }
    public void ClearLookAt()
    {
        m_LookAtData = null;
    }
    public Vector3 GetCameraWorldPos()
    {
        if (m_MainCamera == null)
            return Vector3.zero;
        return m_MainCamera.transform.position;
    }

    private void Update()
    {
        //if (m_LookAtData == null)
        //    return;
        //var tran = m_VirtualCamera.transform;
        //var curPos = tran.transform.position;
        //var toPos = m_LookAtData.followTran.position;
        //var lookAtPos = m_LookAtData.lookAtTran.position;
        //var pos = Vector3.Lerp(curPos, toPos, ABBUtil.GetTimeDelta() * 5);

        //var forword = toPos - lookAtPos;
        //m_VirtualCamera.ForceCameraPosition(pos, Quaternion.Euler(Vector3.zero));

    }
}
