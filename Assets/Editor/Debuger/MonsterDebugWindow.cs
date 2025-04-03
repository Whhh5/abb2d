using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

public class MonsterDebugWindow : EditorWindow
{

    private void OnEnable()
    {
        _MonsterEntityData.Clear();
    }
    private void OnGUI()
    {
        DrawMonsterInfo();
        GUILayout.Space(10);
        DrawMonsterColonyInfo();
    }

    private int _MonsterColonyID = 1;
    private Vector3 _MonsterColonyWorldPos;
    private void DrawMonsterColonyInfo()
    {
        var colonyRect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(colonyRect, new Color(1, 1, 1, 0.2f));
        {
            _MonsterColonyID = EditorGUILayout.IntField(_MonsterColonyID, GUILayout.Width(50));
            _MonsterColonyWorldPos = EditorGUILayout.Vector3Field("", _MonsterColonyWorldPos, GUILayout.Width(200));
            if (GUILayout.Button("create"))
            {
                MonsterColonyMgr.Instance.CreateMonsterColony(_MonsterColonyID, _MonsterColonyWorldPos);
            }
        }
        EditorGUILayout.EndVertical();
    }


    private List<int> _MonsterEntityData = new();
    private int _MonsterID = 2;
    private Vector3 _TargetPos = Vector3.zero;
    private int _ExecuteEntityID = -1;
    private EnEntityCmd _Cmd = EnEntityCmd.Idle;
    private EnEntityControllerType _ControllerType = EnEntityControllerType.None;
    private void DrawMonsterInfo()
    {
        var verRect = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(verRect, new Color(1, 1, 1, 0.2f));
        {
            EditorGUILayout.BeginHorizontal();
            {
                _MonsterID = EditorGUILayout.IntField(_MonsterID, GUILayout.Width(100));
                _TargetPos = EditorGUILayout.Vector3Field("", _TargetPos, GUILayout.Width(200));
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Create"))
            {
                var entityData = MonsterMgr.Instance.CreateMonster(_MonsterID, _TargetPos);
                _MonsterEntityData.Add(entityData);
            }

            GUILayout.Space(30);

            EditorGUILayout.BeginHorizontal();
            {
                _ExecuteEntityID = EditorGUILayout.IntField(_ExecuteEntityID, GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                _Cmd = (EnEntityCmd)EditorGUILayout.EnumPopup(_Cmd, GUILayout.Width(100));
                if (GUILayout.Button("Execute", GUILayout.Width(100)))
                {
                    Entity3DMgr.Instance.AddEntityCmd(_ExecuteEntityID, _Cmd);
                }
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Controller"))
            {
                PlayerMgr.Instance.SetControllerPlayerID(_ExecuteEntityID);
            }
            EditorGUILayout.BeginHorizontal();
            {
                _ControllerType = (EnEntityControllerType)EditorGUILayout.EnumPopup(_ControllerType, GUILayout.Width(100));
                if (GUILayout.Button("Controller Type", GUILayout.Width(100)))
                {
                    //Entity3DMgr.Instance.SetEntityControllerType(_ExecuteEntityID, _ControllerType);
                    if (_ControllerType == EnEntityControllerType.AI)
                    {
                        if (!Entity3DMgr.Instance.ContainsEntityCom<EntityAIComData>(_ExecuteEntityID))
                        {
                            Entity3DMgr.Instance.AddEntityCom<EntityAIComData>(_ExecuteEntityID);
                        }
                    }
                    else
                    {
                        if (Entity3DMgr.Instance.ContainsEntityCom<EntityAIComData>(_ExecuteEntityID))
                        {
                            Entity3DMgr.Instance.RemoveEntityCom<EntityAIComData>(_ExecuteEntityID);
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginVertical();
            {
                var count = Application.isPlaying ? _MonsterEntityData.Count : 0;
                for (int i = 0; i < count; i++)
                {
                    var entityID = _MonsterEntityData[i];
                    var entity = Entity3DMgr.Instance.GetEntity3DData(entityID);

                    var go = ABBGOMgr.Instance.GetGo(entity.GOID);

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button($"{entityID}", GUILayout.Width(50)))
                        {
                            _ExecuteEntityID = entityID;
                        }
                        EditorGUILayout.ObjectField(go, typeof(GameObject), true, GUILayout.Width(100));
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

}
