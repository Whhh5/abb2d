// BuffCfg
public class BuffCfg : ICfg
{
	private BuffCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nBuffID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nBuffType;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strDescEditor;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strEnumNameEditor;
	public System.Int32 GetID()
	{
		return nBuffID;
	}
}
