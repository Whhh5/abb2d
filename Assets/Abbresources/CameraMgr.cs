using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class CameraMgr : Singleton<CameraMgr>
{
    private int m_CameraDataID = -1;
    private Camera m_MainCamera;
    private UniversalAdditionalCameraData m_URPCamera = null;
    private CinemachineVirtualCamera m_VirtualCamera = null;
    private int m_LookAtDataGoID = -1;

    public override void Destroy()
    {
        m_CameraDataID
            = m_LookAtDataGoID
            = -1;
        m_MainCamera = null;
        m_URPCamera = null;
        m_VirtualCamera = null;
        base.Destroy();
    }

    public void CreateCamera()
    {
        m_CameraDataID = EntityMgr.Instance.CreateEntityData<CameraEntityData>(EnLoadTarget.Pre_CameraEntity);
        EntityMgr.Instance.LoadEntity(m_CameraDataID);
    }
    public void SetMainCamera(Camera camera)
    {
        m_MainCamera = camera;
        m_URPCamera = camera.GetComponent<UniversalAdditionalCameraData>();

        SetLookAtTran(m_LookAtDataGoID);
    }
    public void SetVirtualCamera(CinemachineVirtualCamera virtualCamera)
    {
        m_VirtualCamera = virtualCamera;
    }
    public void SetURPCamera(UniversalAdditionalCameraData urpCamera)
    {
        m_URPCamera = urpCamera;
    }
    public void SetLookAtTran(int goID)
    {
        m_LookAtDataGoID = goID;
        if(m_VirtualCamera != null)
        {
            var goLockAtData = ABBGOMgr.Instance.GetGoCom<CameraLookAtData>(goID);
            m_VirtualCamera.Follow = goLockAtData.followTran;
            m_VirtualCamera.LookAt = goLockAtData.lookAtTran;
        }
    }
    public Vector3 GetCameraWorldPos()
    {
        if (m_MainCamera == null)
            return Vector3.zero;
        return m_MainCamera.transform.position;
    }

}
