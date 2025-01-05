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
			m_DicEffectCfg0.Add(cfg.nCmdID, cfg);
		}
		for (int i = 0; i < m_AssetCfg.Length; i++)
		{
			var cfg = m_AssetCfg[i];
			m_DicAssetCfg0.Add(cfg.nAssetID, cfg);
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
	public EffectCfg GetEffectCfg0(System.Int32 nCmdID)
	{
		return m_DicEffectCfg0[nCmdID];
	}
	public AssetCfg GetAssetCfg0(System.Int32 nAssetID)
	{
		return m_DicAssetCfg0[nAssetID];
	}
}
