
public class SkillCfgEditor : ICfg
{
    public int nSkillID;
    public string strName;
    public int nType;
    public int[] arrParams;

    public int GetID()
    {
        throw new System.NotImplementedException();
    }
}

public class MonsterCfgEditor: ICfg
{
    public System.Int32 nMonsterID;
    public System.String strName;
    public System.Int32[] arrSkillGroup;
    public System.Int32 nAssetCfgID;

    public int GetID()
    {
        throw new System.NotImplementedException();
    }
}

public class AssetCfgEditor: ICfg
{
    public readonly System.Int32 nAssetID;

    public readonly System.String enName;

    public readonly System.String strPath;

    public int GetID()
    {
        throw new System.NotImplementedException();
    }
}