using System.Collections.Generic;

public enum EnLoadTarget
{

}
public class LoadConfig : Singleton<LoadConfig>
{
    private Dictionary<EnLoadTarget, string> m_DicPrefabPath;

    public string GetTargetPath(EnLoadTarget target)
    {
        if (!m_DicPrefabPath.TryGetValue(target, out var path))
            return null;
        return path;
    }
    public override void Awake()
    {
        base.Awake();

        //LoadMgr.ins
    }
}

