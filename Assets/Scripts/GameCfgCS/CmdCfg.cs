// CmdCfg
public class CmdCfg : ICfg
{
	private CmdCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nCmdID;
	// 备注
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strDescEditor;
	// 指令登等级
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nLevel;
	// 指令类型
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nType;
	// 指令参数
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrParams;
	// 是否启用根运动
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 bApplyRootMotion;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrLayer;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 bIdleLayerPlay;
	public System.Int32 GetID()
	{
		return nCmdID;
	}
}
