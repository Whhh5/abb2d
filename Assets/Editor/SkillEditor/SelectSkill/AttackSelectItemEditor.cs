using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;
using static UnityEditor.GenericMenu;

public class AttackSelectItemEditor : AttackSelectItem, IEditorItem
{
    //public EntityPropertyInfoEditor m_PropertyInfoEditor = null;
    public List<AttackSelectItemInfoEditor> itemInfoEditorList = new();

    private List<bool> foldoutList = new();
    public void InitEditor()
    {
        foreach (var item in itemInfo ?? new AttackSelectItemInfo[0])
        {
            var editorData = EditorUtil.Copy<AttackSelectItemInfoEditor>(item);
            itemInfoEditorList.Add(editorData);
        }
        foldoutList = new(new bool[itemInfoEditorList.Count]);
    }
    public void GetStringData(ref List<int> result)
    {
        var gIndex = result.Count;
        result.Add((int)target);
        result.Add(itemInfoEditorList.Count);
        result.Insert(gIndex, result.Count - gIndex);

        //m_PropertyInfoEditor.GetStr(ref result);


        for (int i = 0; i < itemInfoEditorList.Count; i++)
        {
            var index = result.Count;
            var item = itemInfoEditorList[i];
            item.GetStringData(ref result);
            result.Insert(index, result.Count - index);
        }
    }


    public void AddAttackSelectItemInfoEditor(AttackSelectItemInfoEditor infoEditor)
    {
        itemInfoEditorList.Add(infoEditor);
        foldoutList.Add(true);
    }
    public void RemoveAtAttackSelectItemInfoEditor(int index)
    {
        itemInfoEditorList.RemoveAt(index);
        foldoutList.RemoveAt(index);
    }
    private List<(GUIContent title, Func<AttackSelectItemInfoEditor> click)> GetOperationMenu(EnOperationType operationType, string rootMenu)
    {
        return operationType switch
        {
            EnOperationType.Compare => CompareLessInfoEditor.GetOperationCompareMenu(rootMenu),
            _ => new(),
        };
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                target = (EnEntityProperty)EditorGUILayout.EnumPopup(target, GUILayout.Width(100));

                GUILayout.Space(20);
                if (GUILayout.Button("➕", GUILayout.Width(50)))
                {
                    var menu = new GenericMenu();

                    for (var i = EnOperationType.None + 1; i < EnOperationType.EnumCount; i++)
                    {
                        var name = EditorUtil.GetEnumName(i);

                        var menuList = GetOperationMenu(i, name);

                        for (int j = 0; j < menuList.Count; j++)
                        {
                            var (title, click) = menuList[j];
                            menu.AddItem(title, false, () =>
                            {
                                var info = click();
                                AddAttackSelectItemInfoEditor(info);
                            }
                            );
                        }
                    }

                    menu.ShowAsContext();
                }
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < itemInfoEditorList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("❌", GUILayout.Width(50)))
                    {
                        RemoveAtAttackSelectItemInfoEditor(i);
                        continue;
                    }

                    foldoutList[i] = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutList[i], $"{i}");
                    {
                        var editorData = itemInfoEditorList[i];
                        editorData.Draw();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

}