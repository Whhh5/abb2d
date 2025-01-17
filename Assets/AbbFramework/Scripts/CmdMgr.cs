using UnityEngine;




public enum EnSkillBoxType
{
    None,
    Link,
    Random,
    Singleton,
    Loop,
    Select,
    EnumCount,
}

public class CmdMgr : Singleton<CmdMgr>
{


    public CmdPlayableAdapter GetPlayable(PlayableGraphAdapter graph, EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        switch (cmdCfg.nType)
        {
            case 1:
                var playable = GetSkillPlayable(cmdCfg.arrParams[0], graph);
                return playable;
            default:
                break;
        }
        return null;
    }

    public CmdPlayableAdapter GetSkillPlayable(int skillID, PlayableGraphAdapter graph)
    {
        var skillCfg = GameSchedule.Instance.GetSkillCfg0(skillID);
        var customData = ClassPoolMgr.Instance.Pull<AttackCmdPlayableAdapterData>();
        customData.arrParams = skillCfg.arrParams;
        CmdPlayableAdapter result = (EnSkillBoxType)skillCfg.nType switch
        {
            EnSkillBoxType.Link => graph.Create<AttackCmdPlayableAdapter>(customData),
            EnSkillBoxType.Random => graph.Create<IdleCmdPlayableAdapter>(customData),
            EnSkillBoxType.Singleton => graph.Create<RunCmdPlayableAdapter>(customData),
            EnSkillBoxType.Loop => graph.Create<Skill2CmdPlayableAdapter>(customData),
            EnSkillBoxType.Select => graph.Create<JumpDownCmdPlayableAdapter>(customData),
            _ => null
        };
        ClassPoolMgr.Instance.Push(customData);
        return result;
    }
}
