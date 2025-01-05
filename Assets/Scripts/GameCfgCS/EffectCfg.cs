// EffectCfg
public class EffectCfg : ICfg
{
	private EffectCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nCmdID;
	// 指令类型
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetID;
}
