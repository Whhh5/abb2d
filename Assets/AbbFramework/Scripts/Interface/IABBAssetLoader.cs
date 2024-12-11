
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IABBAssetLoader
{
    public UniTask<int> LoadAssetAsync<T>(string assetPath, CancellationTokenSource cancellation)
        where T: Object;
    public int LoadAsset<T>(string assetPath)
        where T: Object;
    public void UnloadAsset(int obj);
    public Object GetObject(int objID);
}

