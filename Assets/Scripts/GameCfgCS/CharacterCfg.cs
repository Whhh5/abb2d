// CharacterCfg
public class CharacterCfg : ICfg
{
	private CharacterCfg() {}
	// id
	[Newtonsoft.Json.JsonProperty()] public readonly System.Int32 nCharacterID;
	public System.Int32 GetID()
	{
		return nCharacterID;
	}
}
