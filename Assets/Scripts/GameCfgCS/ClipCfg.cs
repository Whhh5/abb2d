// ClipCfg
public class ClipCfg : ICfg
{
	private ClipCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nClipID;
	// 是否循环
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 bIsLoop;
	// 长度
	[Newtonsoft.Json.JsonProperty()] public readonly System.Single fLength;
	// 层级
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nLayer;
	// 资源ID
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetID;
}
