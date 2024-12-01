using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;



public class MapMgr : Singleton<MapMgr>
{

    public void CreateMap(MapDataSO mapData)
    {
        for (int row = 0; row < mapData.GetRowCount(); row++)
        {
            for (int col = 0; col < mapData.GetColCount(); col++)
            {
                var pos = mapData.GetMapPos(row, col);
                var chunkEntityID = EntityMgr.Instance.CreateEntityData<MapChunkEntityData>(EnLoadTarget.Pre_MapChunk_Default);
                var ChunkEntityData = EntityMgr.Instance.GetEntityData(chunkEntityID);
                ChunkEntityData.SetPosition(pos);
                EntityMgr.Instance.LoadEntity(chunkEntityID);
            }
        }
    }
}
