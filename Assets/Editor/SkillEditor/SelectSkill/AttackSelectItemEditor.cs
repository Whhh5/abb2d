using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




public class AttackSelectItemEditor : AttackSelectItem, IEditorItem
{
    //public EntityPropertyInfoEditor m_PropertyInfoEditor = null;
    public List<AttackSelectItemInfoEditor> m_ItemInfoEditorList = new();

    public void InitEditor()
    {
        foreach (var item in itemInfo ?? new AttackSelectItemInfo[0])
        {
            var editorData = EditorUtil.Copy<AttackSelectItemInfoEditor>(item);
            m_ItemInfoEditorList.Add(editorData);
        }
    }
    public void GetStringData(ref List<int> result)
    {
        var gIndex = result.Count;
        result.Add((int)target);
        result.Add(m_ItemInfoEditorList.Count);
        result.Insert(gIndex, result.Count - gIndex);

        //m_PropertyInfoEditor.GetStr(ref result);


        for (int i = 0; i < m_ItemInfoEditorList.Count; i++)
        {
            var index = result.Count;
            var item = m_ItemInfoEditorList[i];
            item.GetStringData(ref result);
            result.Insert(index, result.Count - index);
        }
    }

    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            target = (EnEntityProperty)EditorGUILayout.EnumPopup(target);

            EditorGUILayout.BeginHorizontal();
            {
                for (int i = 0; i < m_ItemInfoEditorList.Count; i++)
                {
                    var editorData = m_ItemInfoEditorList[i];
                    editorData.Draw();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
}