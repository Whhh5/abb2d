
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttackLinkPhysicsItemEditor : AttackLinkPhysicsItem, ISkillScheduleActionEditor
{
    private IEditorItem m_PhysicsResolveSphereEditor = null;
    public void InitEditor()
    {
        UpdatePhysicsResolveSphereEditor();
    }
    private void UpdatePhysicsResolveSphereEditor()
    {
        switch (physicsType)
        {
            case EnPhysicsType.Sphere:
                m_PhysicsResolveSphereEditor = EditorUtil.Copy<PhysicsResolveSphereEditor>(physicsResolve ?? new PhysicsResolveSphereEditor());
                break;
            case EnPhysicsType.Box:
                m_PhysicsResolveSphereEditor = EditorUtil.Copy<PhysicsResolveBoxEditor>(physicsResolve ?? new PhysicsResolveBoxEditor());
                break;
            default:
                break;
        }
        m_PhysicsResolveSphereEditor?.InitEditor();
    }
    public void Draw()
    {
        EditorGUILayout.BeginVertical();
        {
            var type = (EnPhysicsType)EditorGUILayout.EnumPopup("检测类型", physicsType, GUILayout.Width(300));
            if (type != physicsType)
            {
                switch (type)
                {
                    case EnPhysicsType.Sphere:
                        m_PhysicsResolveSphereEditor = EditorUtil.Copy<PhysicsResolveSphereEditor>(new PhysicsResolveSphere());
                        break;
                    case EnPhysicsType.Box:
                        m_PhysicsResolveSphereEditor = EditorUtil.Copy<PhysicsResolveBoxEditor>(new PhysicsResolveBox());
                        break;
                    default:
                        break;
                }
                physicsType = type;
            }
            atkSchedule = EditorGUILayout.Slider("检测进度", atkSchedule, 0, 1, GUILayout.Width(300));
            atkValue = EditorGUILayout.IntField("伤害值", atkValue, GUILayout.Width(300));

            m_PhysicsResolveSphereEditor?.Draw();
        }
        EditorGUILayout.EndVertical();
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Add(Mathf.RoundToInt(atkSchedule * 100));
        data.Add(Mathf.RoundToInt(atkValue));
        data.Add((int)physicsType);
        data.Insert(index, data.Count - index);

        m_PhysicsResolveSphereEditor.GetStringData(ref data);
    }
}