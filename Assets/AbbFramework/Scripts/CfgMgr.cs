using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public interface ICfg
{
    
}
[EnumName("包围盒类型")]
public enum EnPhysicsType
{
    Sphere = 1,
    Box = 2,
}
[EnumName("方形包围盒位置类型")]
public enum EnPhysicsBoxCenterType
{
    Center = 1,
    Bottom = 2,
}
[EnumName("方形包围盒类型")]
public enum EnPhysicsBoxType
{
    Once = 1,
    Successive = 2,
}
public class CfgMgr : Singleton<CfgMgr>
{
    //private SkillCfg[] m_ArrSkillCfg = null;
    //private Dictionary<int, SkillCfg> m_DicSkillCfg = new();
    //{
    //    {
    //        1,
    //        new()
    //        {
    //            nSkillID = 1,
    //            arrParams = new int[]
    //            {
    //                (int)EnLoadTarget.Anim_Attack_01,37,15,
    //                    1,
    //                    // 执行时机，伤害值，检测范围类型，参数数量，位置偏移x,y,z，位置类型，检测时间，单位检测大小，包围盒大小x,y,z，旋转x,y,z
    //                        10, 10, (int)EnPhysicsType.Box, 13, 0, 0, 0, (int)EnPhysicsBoxCenterType.Bottom, (int)EnPhysicsBoxType.Successive, 0, 50, 200, 200, 200, 0, 0, 0,
    //                    1,
    //                    // 特效id，执行时机，参数数量，飞行时间，飞行距离
    //                        1, 10, 2, 10, 200,
    //                (int)EnLoadTarget.Anim_Attack_02,33,20,
    //                    1,
    //                        10, 20, (int)EnPhysicsType.Box, 13, 0, 0, 0, (int)EnPhysicsBoxCenterType.Bottom, (int)EnPhysicsBoxType.Once, 0, 200, 200, 200, 200, 0, 0, 0,
    //                    1,
    //                        1, 10, 2, 10, 200,
    //                (int)EnLoadTarget.Anim_Attack_03,47,20,
    //                    1,
    //                        15, 30, (int)EnPhysicsType.Box, 13, 0, 0, 0, (int)EnPhysicsBoxCenterType.Bottom, (int)EnPhysicsBoxType.Successive, 10, 50, 200, 200, 200, 0, 0, 0,
    //                    1,
    //                        1, 15, 2, 10, 200,
    //                (int)EnLoadTarget.Anim_Attack_04,42,20,
    //                    1,
    //                        10, 40, (int)EnPhysicsType.Sphere, 4, 0, 0, 000, 200,
    //                    1,
    //                        1, 10, 2, 10, 200,
    //                (int)EnLoadTarget.Anim_Attack_05,60,30,
    //                    2,
    //                        20, 50, (int)EnPhysicsType.Box, 13, 0, 0, 0, (int)EnPhysicsBoxCenterType.Bottom, (int)EnPhysicsBoxType.Successive, 20, 50, 200, 200, 200, 0, 0, 0,
    //                        25, 50, (int)EnPhysicsType.Box, 13, 0, 0, 0, (int)EnPhysicsBoxCenterType.Bottom, (int)EnPhysicsBoxType.Successive, 30, 50, 200, 200, 400, 0, 0, 0,
    //                    2,
    //                        1, 20, 2, 20, 200,
    //                        1, 25, 2, 30, 400,
    //                (int)EnLoadTarget.Anim_Attack_06,65,50,
    //                    3,
    //                        40, 40, (int)EnPhysicsType.Box, 13, 0, 0, 0, (int)EnPhysicsBoxCenterType.Bottom, (int)EnPhysicsBoxType.Successive, 20, 50, 200, 200, 500, 0, 0, 0,
    //                        45, 50, (int)EnPhysicsType.Box, 13, 0, 0, 0, (int)EnPhysicsBoxCenterType.Bottom, (int)EnPhysicsBoxType.Successive, 25, 50, 200, 200, 700, 0, 0, 0,
    //                        50, 60, (int)EnPhysicsType.Box, 13, 0, 0, 0, (int)EnPhysicsBoxCenterType.Bottom, (int)EnPhysicsBoxType.Successive, 30, 50, 200, 200, 1000, 0, 0, 0,
    //                    3,
    //                        1, 40, 2, 20, 500,
    //                        1, 45, 2, 25, 700,
    //                        1, 50, 2, 30, 1000,
    //            },
    //        }
    //    },
    //};


    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        var type = Instance.GetType();
    }
}
