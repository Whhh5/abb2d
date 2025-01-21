using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SkillTypeSelectItemInfoEditor : SkillTypeSelectItemInfo, ISkillTypeEditor
{
    public IOperationInfoEditor _OperationInfoEditor = null;
    public SkillItemInfoEditor _AtkItemDataEditor = null;


    private bool _Foldout = false;
    public void InitEditor()
    {
        var editorType = SkillFactroyEditor.GetOperationDataType(operationType, operationInfo.GetChildType());
        _OperationInfoEditor = EditorUtil.Copy<IOperationInfoEditor>(operationInfo, editorType);
        _OperationInfoEditor.InitEditor();

        _AtkItemDataEditor = EditorUtil.Copy<SkillItemInfoEditor>(atkItemData);
        _AtkItemDataEditor.InitEditor();
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            var labelName = EditorUtil.GetEnumName(operationType);
            EditorGUILayout.LabelField(labelName, GUILayout.Width(200));

            EditorGUILayout.Space(20);
            _OperationInfoEditor.Draw();

            EditorUtil.DrawLinkHor(20);

            if(_Foldout = EditorGUILayout.Foldout(_Foldout, "clip info"))
            {
                _AtkItemDataEditor.Draw();
            }
            EditorGUILayout.EndFadeGroup();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> result)
    {
        var childType = _OperationInfoEditor.GetChildType();


        var gIndex = result.Count;
        result.Add((int)operationType);
        result.Add(childType.Length);
        result.Insert(gIndex, result.Count - gIndex);


        result.AddRange(childType);

        var operationCountIndex = result.Count;
        _OperationInfoEditor.GetStringData(ref result);
        result.Insert(operationCountIndex, result.Count - operationCountIndex);

        var atkItemParamIndex = result.Count;
        _AtkItemDataEditor.GetStringData(ref result);
        result.Insert(atkItemParamIndex, result.Count - atkItemParamIndex);
    }
}