using UnityEngine;

public class CmdMgr : Singleton<CmdMgr>
{


    public CmdPlayableAdapter GetPlayable(PlayableGraphAdapter graph, EnEntityCmd cmd)
    {
        var cmdCfg = GameSchedule.Instance.GetCmdCfg0((int)cmd);
        switch (cmdCfg.nType)
        {
            case 1:
                var playable = GetSkillPlayable(cmdCfg.arrParams[0],  graph);
                return playable;
            default:
                break;
        }
        return null;
    }

    public CmdPlayableAdapter GetSkillPlayable(int skillID, PlayableGraphAdapter graph)
    {
        var skillCfg = GameSchedule.Instance.GetSkillCfg0(skillID);
        switch (skillCfg.nType)
        {
            case 1:
                {
                    var customData = GameClassPoolMgr.Instance.Pull<AttackCmdPlayableAdapterData>();
                    customData.arrParams = skillCfg.arrParams;
                    var playable = graph.Create<AttackCmdPlayableAdapter>(customData);
                    GameClassPoolMgr.Instance.Push(customData);
                    return playable;
                }
            case 2:
                {
                    var customData = GameClassPoolMgr.Instance.Pull<IdleCmdPlayableAdapterData>();
                    customData.arrClip = skillCfg.arrParams;
                    var playable = graph.Create<IdleCmdPlayableAdapter>(customData);
                    GameClassPoolMgr.Instance.Push(customData);
                    return playable;
                }
            case 3:
                {
                    var customData = GameClassPoolMgr.Instance.Pull<RunCmdPlayableAdapterData>();
                    customData.arrClip = skillCfg.arrParams;
                    var playable = graph.Create<RunCmdPlayableAdapter>(customData);
                    GameClassPoolMgr.Instance.Push(customData);
                    return playable;
                }
            case 4:
                {
                    var customData = GameClassPoolMgr.Instance.Pull<Skill2CmdPlayableAdapterData>();
                    customData.arrParams = skillCfg.arrParams;
                    var playable = graph.Create<Skill2CmdPlayableAdapter>(customData);
                    GameClassPoolMgr.Instance.Push(customData);
                    return playable;
                }
            default:
                break;
        }
        return null;
    }
}
