// SkillCfg
public class SkillCfg : ICfg
{
	private SkillCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nSkillID;
	// 名字
	[Newtonsoft.Json.JsonProperty()] public readonly System.String strName;
	// 类型
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nType;
	// 参数
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrParams;
}
