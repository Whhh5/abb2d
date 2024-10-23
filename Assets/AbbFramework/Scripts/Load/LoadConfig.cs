using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

public class LoadConfig : Singleton<LoadConfig>
{
    private Dictionary<EnLoadTarget, LoadConfigItem> m_DicPrefabPath = new();

    public string GetTargetPath(EnLoadTarget target)
    {
        if (!m_DicPrefabPath.TryGetValue(target, out var data))
            return null;
        return data.Path;
    }
    public override async UniTask AwakeAsync()
    {
        await base.AwakeAsync();

        var obj = await LoadMgr.Instance.LoadAsync(GlobalConfig.LoadConfigRecordsJson.Replace(".txt", ""));
        var str = obj as TextAsset;
        var target = JsonConvert.DeserializeObject<List<LoadConfigItem>>(str.text);
        foreach (var item in target)
        {
            m_DicPrefabPath.Add(item.LoadTarget, item);
        }
    }
}

