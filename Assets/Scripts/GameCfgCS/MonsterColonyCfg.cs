// MonsterColonyCfg
public class MonsterColonyCfg : ICfg
{
	private MonsterColonyCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nColonyID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrMonsterID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Single fCreateIntervalTime;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nMaxCount;
	// AssetCfg
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nAssetID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Single[] v3CreateLocalPos;
	// 100
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nRangeRadius;
	public System.Int32 GetID()
	{
		return nColonyID;
	}
}
