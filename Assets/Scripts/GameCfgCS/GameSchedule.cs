public partial class GameSchedule
{
	private ClipCfg[] m_ClipCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, ClipCfg> m_DicClipCfg0 = new();
	private SkillCfg[] m_SkillCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, SkillCfg> m_DicSkillCfg0 = new();
	private AnimLayerCfg[] m_AnimLayerCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, AnimLayerCfg> m_DicAnimLayerCfg0 = new();
	private CmdCfg[] m_CmdCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, CmdCfg> m_DicCmdCfg0 = new();
	private CmdTyoeCfg[] m_CmdTyoeCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, CmdTyoeCfg> m_DicCmdTyoeCfg0 = new();
	private CmdLevelCfg[] m_CmdLevelCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, CmdLevelCfg> m_DicCmdLevelCfg0 = new();
	private EffectCfg[] m_EffectCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, EffectCfg> m_DicEffectCfg0 = new();
	private AssetCfg[] m_AssetCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, AssetCfg> m_DicAssetCfg0 = new();
	private CharacterCfg[] m_CharacterCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, CharacterCfg> m_DicCharacterCfg0 = new();
	private MonsterCfg[] m_MonsterCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, MonsterCfg> m_DicMonsterCfg0 = new();
	private MonsterColonyCfg[] m_MonsterColonyCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, MonsterColonyCfg> m_DicMonsterColonyCfg0 = new();
	private AIModuleCfg[] m_AIModuleCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, AIModuleCfg> m_DicAIModuleCfg0 = new();
	private AICfg[] m_AICfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, AICfg> m_DicAICfg0 = new();
	private MonsterControllerCfg[] m_MonsterControllerCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, MonsterControllerCfg> m_DicMonsterControllerCfg0 = new();
	private BuffCfg[] m_BuffCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, BuffCfg> m_DicBuffCfg0 = new();
	private BuffTypeCfg[] m_BuffTypeCfg = null;
	private System.Collections.Generic.Dictionary<System.Int32, BuffTypeCfg> m_DicBuffTypeCfg0 = new();
	public void Initialization()
	{
		for (int i = 0; i < m_ClipCfg.Length; i++)
		{
			var cfg = m_ClipCfg[i];
			m_DicClipCfg0.Add(cfg.nClipID, cfg);
		}
		for (int i = 0; i < m_SkillCfg.Length; i++)
		{
			var cfg = m_SkillCfg[i];
			m_DicSkillCfg0.Add(cfg.nSkillID, cfg);
		}
		for (int i = 0; i < m_AnimLayerCfg.Length; i++)
		{
			var cfg = m_AnimLayerCfg[i];
			m_DicAnimLayerCfg0.Add(cfg.nLayer, cfg);
		}
		for (int i = 0; i < m_CmdCfg.Length; i++)
		{
			var cfg = m_CmdCfg[i];
			m_DicCmdCfg0.Add(cfg.nCmdID, cfg);
		}
		for (int i = 0; i < m_CmdTyoeCfg.Length; i++)
		{
			var cfg = m_CmdTyoeCfg[i];
			m_DicCmdTyoeCfg0.Add(cfg.nCmdID, cfg);
		}
		for (int i = 0; i < m_CmdLevelCfg.Length; i++)
		{
			var cfg = m_CmdLevelCfg[i];
			m_DicCmdLevelCfg0.Add(cfg.nLevelID, cfg);
		}
		for (int i = 0; i < m_EffectCfg.Length; i++)
		{
			var cfg = m_EffectCfg[i];
			m_DicEffectCfg0.Add(cfg.nEffectID, cfg);
		}
		for (int i = 0; i < m_AssetCfg.Length; i++)
		{
			var cfg = m_AssetCfg[i];
			m_DicAssetCfg0.Add(cfg.nAssetID, cfg);
		}
		for (int i = 0; i < m_CharacterCfg.Length; i++)
		{
			var cfg = m_CharacterCfg[i];
			m_DicCharacterCfg0.Add(cfg.nCharacterID, cfg);
		}
		for (int i = 0; i < m_MonsterCfg.Length; i++)
		{
			var cfg = m_MonsterCfg[i];
			m_DicMonsterCfg0.Add(cfg.nMonsterID, cfg);
		}
		for (int i = 0; i < m_MonsterColonyCfg.Length; i++)
		{
			var cfg = m_MonsterColonyCfg[i];
			m_DicMonsterColonyCfg0.Add(cfg.nColonyID, cfg);
		}
		for (int i = 0; i < m_AIModuleCfg.Length; i++)
		{
			var cfg = m_AIModuleCfg[i];
			m_DicAIModuleCfg0.Add(cfg.nModuleID, cfg);
		}
		for (int i = 0; i < m_AICfg.Length; i++)
		{
			var cfg = m_AICfg[i];
			m_DicAICfg0.Add(cfg.nAIID, cfg);
		}
		for (int i = 0; i < m_MonsterControllerCfg.Length; i++)
		{
			var cfg = m_MonsterControllerCfg[i];
			m_DicMonsterControllerCfg0.Add(cfg.nControllerID, cfg);
		}
		for (int i = 0; i < m_BuffCfg.Length; i++)
		{
			var cfg = m_BuffCfg[i];
			m_DicBuffCfg0.Add(cfg.nBuffID, cfg);
		}
		for (int i = 0; i < m_BuffTypeCfg.Length; i++)
		{
			var cfg = m_BuffTypeCfg[i];
			m_DicBuffTypeCfg0.Add(cfg.nTypeID, cfg);
		}
	}
	public ClipCfg GetClipCfg0(System.Int32 nClipID)
	{
		return m_DicClipCfg0[nClipID];
	}
	public SkillCfg GetSkillCfg0(System.Int32 nSkillID)
	{
		return m_DicSkillCfg0[nSkillID];
	}
	public AnimLayerCfg GetAnimLayerCfg0(System.Int32 nLayer)
	{
		return m_DicAnimLayerCfg0[nLayer];
	}
	public CmdCfg GetCmdCfg0(System.Int32 nCmdID)
	{
		return m_DicCmdCfg0[nCmdID];
	}
	public CmdTyoeCfg GetCmdTyoeCfg0(System.Int32 nCmdID)
	{
		return m_DicCmdTyoeCfg0[nCmdID];
	}
	public CmdLevelCfg GetCmdLevelCfg0(System.Int32 nLevelID)
	{
		return m_DicCmdLevelCfg0[nLevelID];
	}
	public EffectCfg GetEffectCfg0(System.Int32 nEffectID)
	{
		return m_DicEffectCfg0[nEffectID];
	}
	public AssetCfg GetAssetCfg0(System.Int32 nAssetID)
	{
		return m_DicAssetCfg0[nAssetID];
	}
	public CharacterCfg GetCharacterCfg0(System.Int32 nCharacterID)
	{
		return m_DicCharacterCfg0[nCharacterID];
	}
	public MonsterCfg GetMonsterCfg0(System.Int32 nMonsterID)
	{
		return m_DicMonsterCfg0[nMonsterID];
	}
	public MonsterColonyCfg GetMonsterColonyCfg0(System.Int32 nColonyID)
	{
		return m_DicMonsterColonyCfg0[nColonyID];
	}
	public AIModuleCfg GetAIModuleCfg0(System.Int32 nModuleID)
	{
		return m_DicAIModuleCfg0[nModuleID];
	}
	public AICfg GetAICfg0(System.Int32 nAIID)
	{
		return m_DicAICfg0[nAIID];
	}
	public MonsterControllerCfg GetMonsterControllerCfg0(System.Int32 nControllerID)
	{
		return m_DicMonsterControllerCfg0[nControllerID];
	}
	public BuffCfg GetBuffCfg0(System.Int32 nBuffID)
	{
		return m_DicBuffCfg0[nBuffID];
	}
	public BuffTypeCfg GetBuffTypeCfg0(System.Int32 nTypeID)
	{
		return m_DicBuffTypeCfg0[nTypeID];
	}
}
