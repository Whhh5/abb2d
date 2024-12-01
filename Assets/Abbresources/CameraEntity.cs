using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraEntityData : EntityData
{

}
public class CameraEntity : Entity
{
    [SerializeField]
    private UniversalAdditionalCameraData m_URPCamera = null;
    [SerializeField]
    private Camera m_Camera = null;
    [SerializeField]
    private CinemachineVirtualCamera m_VirtualCamera = null;

    public override void LoadCompeletion()
    {
        base.LoadCompeletion();

        CameraMgr.Instance.SetMainCamera(m_Camera);
        CameraMgr.Instance.SetVirtualCamera(m_VirtualCamera);
        CameraMgr.Instance.SetURPCamera(m_URPCamera);
    }
}
