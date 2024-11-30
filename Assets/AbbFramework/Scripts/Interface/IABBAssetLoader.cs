
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IABBAssetLoader
{
    public UniTask<int> LoadAssetAsync(string assetPath, CancellationTokenSource cancellation);
    public int LoadAsset(string assetPath);
    public void UnloadAsset(int obj);
    public Object GetObject(int objID);
}

