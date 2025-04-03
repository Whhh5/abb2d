// TestCfg
public class TestCfg : ICfg
{
	private TestCfg() {}
	// 表内唯一id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nTestID;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nLevel;
	// 
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nType;
	// int
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nIntValue;
	// 一个int数组
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrIntValue;
	// 编辑器使用字段
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strDescEditor;
	public System.Int32 GetID()
	{
		return nTestID;
	}
	public (System.Int32 nLevel,System.Int32 nType) GetID_1()
	{
		return (nLevel,nType);;
	}
}
