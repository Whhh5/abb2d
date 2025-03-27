// CfgTemplete
public class CfgTemplete : ICfg
{
	private CfgTemplete() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nColonyID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrMonsterID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Single fCreateIntervalTime;

    public int GetID()
    {
        throw new System.NotImplementedException();
    }
}
