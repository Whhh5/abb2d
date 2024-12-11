using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Entity3DAnimComData : IEntity3DComData
{
    private int m_EntityID = -1;
    private Entity3DData m_Entity3D = null;
    private PlayableGraphAdapter m_PlayableGraph = null;

    public void RemomveCom()
    {
        m_Entity3D = null;
        m_EntityID = -1;
    }
    public void AddCom(Entity3DData entity3DData)
    {
        m_EntityID = entity3DData.EntityID;
        m_Entity3D = entity3DData;
    }

    public void OnDestroyGO()
    {
        m_CreateAddList.Clear();
        var listKey = new List<EnLoadTarget>(m_ClipAdapterDic.Count);
        foreach (var item in m_ClipAdapterDic) listKey.Add(item.Key);
        foreach (var item in listKey) RemoveAnim(item);
        m_ClipAdapterDic.Clear();
        PlayableGraphAdapter.OnDestroy(m_PlayableGraph);
        m_PlayableGraph = null;
    }
    public void OnCreateGO(Entity3D entity3D)
    {
        var animCom = entity3D.gameObject.GetComponent<Animator>();
        m_PlayableGraph = PlayableGraphAdapter.Create(m_EntityID, animCom);
        foreach (var item in m_CreateAddList)
            AddAnim(item);
        m_CreateAddList.Clear();
    }

    private HashSet<EnLoadTarget> m_CreateAddList = new();
    private Dictionary<EnLoadTarget, PlayableAdapter> m_ClipAdapterDic = new();
    public PlayableAdapter AddAnim(EnLoadTarget clipTarget)
    {
        if (!m_Entity3D.IsLoadSuccess)
        {
            m_CreateAddList.Add(clipTarget);
            return null;
        }
        var animAdapter = PlayableClipAdapter.Create(m_EntityID, m_PlayableGraph, clipTarget);
        var layer = AnimMgr.Instance.GetAnimLayer(clipTarget);
        var isLoop = AnimMgr.Instance.GetAnimIsLoop(clipTarget);
        m_PlayableGraph.Connect(layer, animAdapter);
        //m_ClipAdapterDic.Add(clipTarget, animAdapter);
        return animAdapter;
    }
    public void RemoveAnim(PlayableAdapter adapter)
    {
        if (!m_PlayableGraph.ContainsConnect(adapter))
            return;
        m_PlayableGraph.DisConnect(adapter);
    }

    public void RemoveAnim(EnLoadTarget clipTarget)
    {
        if (!m_ClipAdapterDic.TryGetValue(clipTarget, out var adapter))
            return;
        m_ClipAdapterDic.Remove(clipTarget);
        m_PlayableGraph.DisConnect(adapter);
    }

}
