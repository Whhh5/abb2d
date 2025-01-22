using UnityEngine;




public enum EnSkillBoxType
{
    None,
    [EditorFieldName("连击")]
    Link,
    [EditorFieldName("随机")]
    Random,
    [EditorFieldName("单一")]
    Singleton,
    [EditorFieldName("循环")]
    Loop,
    [EditorFieldName("条件")]
    Select,
    EnumCount,
}

public class CmdMgr : Singleton<CmdMgr>
{


    public SkillTypePlayableAdapter GetPlayable(PlayableGraphAdapter graph, EnEntityCmd cmd)
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

    public SkillTypePlayableAdapter GetSkillPlayable(int skillID, PlayableGraphAdapter graph)
    {
        var skillCfg = GameSchedule.Instance.GetSkillCfg0(skillID);
        var customData = ClassPoolMgr.Instance.Pull<SkillTypeLinkPlayableAdapterCustomData>();
        customData.arrParams = skillCfg.arrParams;
        SkillTypePlayableAdapter result = (EnSkillBoxType)skillCfg.nType switch
        {
            EnSkillBoxType.Link => graph.Create<SkillTypeLinkPlayableAdapter>(customData),
            EnSkillBoxType.Random => graph.Create<SkillTypeRandomPlayableAdapter>(customData),
            EnSkillBoxType.Singleton => graph.Create<SkillTypeSingletonPlayableAdapter>(customData),
            EnSkillBoxType.Loop => graph.Create<SkillTypeLoopPlayableAdapter>(customData),
            EnSkillBoxType.Select => graph.Create<SkillTypeSelectPlayableAdapter>(customData),
            _ => null
        };
        ClassPoolMgr.Instance.Push(customData);
        return result;
    }
}
