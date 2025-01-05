using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;


public class ABBInputMgr : Singleton<ABBInputMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private Vector3 m_LastMoursePos = Vector3.zero;

    private Dictionary<KeyCode, HashSet<UnityAction>> m_OnKeyDownList = new();
    private Dictionary<KeyCode, HashSet<UnityAction>> m_OnKeyUpList = new();
    private Dictionary<KeyCode, HashSet<UnityAction>> m_OnKeyList = new();
    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.L))
        {

        }
        foreach (var item in m_OnKeyDownList)
            if (Input.GetKeyDown(item.Key))
                foreach (var action in item.Value)
                    action.Invoke();
        foreach (var item in m_OnKeyList)
            if (Input.GetKey(item.Key))
                foreach (var action in item.Value)
                    action.Invoke();
        foreach (var item in m_OnKeyUpList)
            if (Input.GetKeyUp(item.Key))
                foreach (var action in item.Value)
                    action.Invoke();
    }
    public void AddListanerDown(KeyCode keyCode, UnityAction action)
    {
        if (!m_OnKeyDownList.TryGetValue(keyCode, out var actionList))
        {
            actionList = new();
            m_OnKeyDownList.Add(keyCode, actionList);
        }
        actionList.Add(action);
    }
    public void RemoveListanerDown(KeyCode keyCode, UnityAction action)
    {
        if (!m_OnKeyDownList.TryGetValue(keyCode, out var actionList))
            return;
        if (!actionList.Remove(action))
            return;
        if (actionList.Count > 0)
            return;
        m_OnKeyDownList.Remove(keyCode);
    }
    public void AddListaner(KeyCode keyCode, UnityAction action)
    {
        if (!m_OnKeyList.TryGetValue(keyCode, out var actionList))
        {
            actionList = new();
            m_OnKeyList.Add(keyCode, actionList);
        }
        actionList.Add(action);
    }
    public void RemoveListaner(KeyCode keyCode, UnityAction action)
    {
        if (!m_OnKeyList.TryGetValue(keyCode, out var actionList))
            return;
        if (!actionList.Remove(action))
            return;
        if (actionList.Count > 0)
            return;
        m_OnKeyList.Remove(keyCode);
    }
    public void AddListanerUp(KeyCode keyCode, UnityAction action)
    {
        if (!m_OnKeyUpList.TryGetValue(keyCode, out var actionList))
        {
            actionList = new();
            m_OnKeyUpList.Add(keyCode, actionList);
        }
        actionList.Add(action);
    }
    public void RemoveListanerUp(KeyCode keyCode, UnityAction action)
    {
        if (!m_OnKeyUpList.TryGetValue(keyCode, out var actionList))
            return;
        if (!actionList.Remove(action))
            return;
        if (actionList.Count > 0)
            return;
        m_OnKeyUpList.Remove(keyCode);
    }
}
