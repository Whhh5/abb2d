// AICfg
public class AICfg : ICfg
{
	private AICfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAIID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrAIModuleID;
}
