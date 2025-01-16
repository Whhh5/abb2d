using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class AttackSelectItemInfoEditor : AttackSelectItemInfo, IEditorItem
{
    private IOperationInfoEditor _OperationInfoEditor = null;
    public AttackLinkItemDataEditor _AtkItemDataEditor = null;


    private bool _Foldout = false;
    public void InitEditor()
    {
        var editorType = OperationFactoryEditor.GetOperationDataType(operationType, operationInfo.GetChildType());
        _OperationInfoEditor = EditorUtil.Copy<IOperationInfoEditor>(operationInfo, editorType);

        _AtkItemDataEditor = EditorUtil.Copy<AttackLinkItemDataEditor>(_AtkItemDataEditor);
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