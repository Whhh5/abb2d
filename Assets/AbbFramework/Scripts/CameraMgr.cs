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
    private Camera _UICamera;
    [SerializeField]
    private UniversalAdditionalCameraData m_URPCamera = null;
    [SerializeField]
    private CinemachineVirtualCamera m_VirtualCamera = null;

    private int m_LookAtEntityID = -1;


    public UniversalAdditionalCameraData GetURPCamera()
    {
        return m_URPCamera;
    }
    protected override void Start()
    {
        base.Start();
        m_VirtualCamera.ForceCameraPosition(new Vector3(-74.35f, 4.6f, -10), new Quaternion(0, 0, 0, 0));
    }
    protected override void OnDestroy()
    {
        m_LookAtEntityID
            = -1;
        m_MainCamera = null;
        m_URPCamera = null;
        m_VirtualCamera = null;
        base.OnDestroy();
    }
    public void SetLookAtTran(int entityID)
    {
        m_LookAtEntityID = entityID;
    }
    public void SetVirtualCameraTrackTran(Transform follow, Transform lookAt)
    {
        m_VirtualCamera.LookAt = lookAt;
        m_VirtualCamera.Follow = follow;
    }
    public void ClearLookAt()
    {
        m_VirtualCamera.LookAt = null;
        m_VirtualCamera.Follow = null;
    }
    public Camera GetMainCamera()
    {
        return m_MainCamera;
    }
    public Camera GetUICamera()
    {
        return _UICamera;
    }
    public Vector3 GetCameraWorldPos()
    {
        if (m_MainCamera == null)
            return Vector3.zero;
        return m_MainCamera.transform.position;
    }
    public Vector3 GetCameraForward()
    {
        return m_MainCamera.transform.forward;
    }
    public Vector3 GetCameraRight()
    {
        return m_MainCamera.transform.right;
    }
    public Vector3 GetCameraUp()
    {
        return m_MainCamera.transform.up;
    }
}
