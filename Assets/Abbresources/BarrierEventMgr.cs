using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BarrierEventMgr : Singleton<BarrierEventMgr>, IABBEventExecute
{
    private Dictionary<int, BarrierEntityData> m_BarrierMap = new();
    private Dictionary<EnBarrierType, BarrierEntityData> m_BarrierDataMap = new();
    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }

    public void EventExecute(EnABBEvent enEvent, int sourceID, int type, object userData)
    {
        
    }
    public void CreateBarrier(MapDataSO mapDataSO)
    {
        var rowCount = mapDataSO.barrierRowCount;
        var colCount = mapDataSO.barrierColCount;

        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (int colIndex = 0; colIndex < colCount; colIndex++)
            {
                var worldPos = mapDataSO.GetBarrierPos(rowIndex, colIndex);
                var barrierDataID = EntityMgr.Instance.CreateEntityData<BarrierEntity1Data>(EnLoadTarget.Pre_BarrierEntity1);
                var barrData = EntityMgr.Instance.GetEntityData<BarrierEntity1Data>(barrierDataID);
                barrData.SetBarrierID(barrierDataID);
                barrData.SetPosition(worldPos);
                var num = Random.Range(1, 100);
                barrData.SetNum(num);
                m_BarrierMap.Add(barrierDataID, barrData);
                EntityMgr.Instance.LoadEntity(barrierDataID);
            }
        }
    }
    public void DestroyBarrier(int barrierID)
    {
        if (!m_BarrierMap.TryGetValue(barrierID, out var barrierData))
            return;
        m_BarrierMap.Remove(barrierID);
        EntityMgr.Instance.UnloadEntity(barrierData.EntityID);
        EntityMgr.Instance.RecycleEntityData(barrierData.EntityID);
    }

}
