// AnimClipCfg
public class AnimClipCfg : ICfg
{
	private AnimClipCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nSkillID;
	// 参数
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32[] arrParams;

    public int GetID()
    {
        throw new System.NotImplementedException();
    }
}
