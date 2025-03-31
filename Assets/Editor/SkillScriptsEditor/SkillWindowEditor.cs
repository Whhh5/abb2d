using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DG.DOTweenEditor;
using DG.Tweening;
using DG.Tweening.Core;
using Newtonsoft.Json;
using TMPro;
using Unity.Entities.UniversalDelegates;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static Codice.Client.BaseCommands.Import.Commit;

public interface ISkillItem
{

}

public class SkillWindowEditor : EditorWindow
{
    [MenuItem("Tools/SkillWindow %^e")]
    public static void OpenWindow()
    {
        var window = EditorWindow.GetWindow<SkillWindowEditor>();
        window.Show();
    }
    private class ItemInfo
    {
        public bool isShow = true;
        public bool isDelect = false;


        private Dictionary<int, Vector2> m_ScrollPos = new();
        public Vector2 GetScrollPos(int index)
        {
            if (!m_ScrollPos.ContainsKey(index))
                m_ScrollPos.Add(index, new());
            return m_ScrollPos[index];
        }
        public void SetScrollPos(int index, Vector2 pos)
        {
            m_ScrollPos[index] = pos;
        }
    }
    private class SkillViewInfo
    {
        public int m_CurSelectID = -1;
        public List<int> m_OpenList = new();
        public Dictionary<int, ItemInfo> m_DicItemInfo = new();

        public void OnEnable()
        {

        }
        public void OnDisable()
        {
            m_CurSelectID = -1;
            m_OpenList.Clear();
            m_DicItemInfo.Clear();
        }
    }
    private Dictionary<int, SkillViewInfo> _MonsterSkillViewInfos = new();
    private SkillViewInfo CurSkillViewInfo => _CurMonsterID > 0
        ? _MonsterSkillViewInfos[_CurMonsterID]
        : new();
    private Dictionary<int, ISkillTypeEditor> m_DicSkilDrawData = new();
    private int _CurMonsterID = -1;
    private bool _IsSimulation = false;
    private MonsterCfg CurMonsterCfg => _CurMonsterID > 0
        ? ExcelUtil.GetCfg<MonsterCfg>(_CurMonsterID)
        : null;

    private void OnEnable()
    {
        LoadCfgData();

    }

    private void OnDisable()
    {
        ClearPlayable();
        CurSkillViewInfo.OnDisable();
        _MonsterSkillViewInfos.Clear();
        m_DicSkilDrawData.Clear();
        _CurMonsterID = -1;
    }

    private void LoadCfgData()
    {
        InitMonsterData();
        InitSkillCfgData();
    }
    private void InitMonsterData()
    {
        var count = ExcelUtil.GetCfgCount<MonsterCfg>();
        for (int i = 0; i < count; i++)
        {
            var cfg = ExcelUtil.GetCfgByIndex<MonsterCfg>(i);
            var data = new SkillViewInfo();
            _MonsterSkillViewInfos.Add(cfg.nMonsterID, data);
            if (cfg.arrSkillGroup != null)
            {
                for (int k = 0; k < cfg.arrSkillGroup.Length; k++)
                {
                    var skillID = cfg.arrSkillGroup[k];
                    data.m_DicItemInfo.Add(skillID, new());
                }
            }

            data.OnEnable();
        }

    }
    private void InitSkillCfgData()
    {
        var count = ExcelUtil.GetCfgCount<SkillCfg>();
        for (int i = 0; i < count; i++)
            AddSkillItem(ExcelUtil.GetCfgByIndex<SkillCfg>(i));
    }
    private void AddSkillItem(SkillCfg skillCfg)
    {
        var key = skillCfg.nSkillID;

        var linkData = SkillFactroyEditor.GetSkillTypeEditor((EnSkillBoxType)skillCfg.nType);
        var data = new AttackLinkSkillDataUserData()
        {
            arrParams = skillCfg.arrParams,
        };
        (linkData as IClassPool).OnPoolInit(data);
        linkData.InitEditor();
        m_DicSkilDrawData.Add(key, linkData);
        CurSkillViewInfo.m_DicItemInfo.Add(key, new());
    }
    private int AddSkillItem(EnSkillBoxType skillType)
    {
        var skillCfg = ExcelUtil.CreateTypeInstance<SkillCfg>();
        ExcelUtil.SetCfgValue(skillCfg, nameof(SkillCfg.nType), (int)skillType);
        var id = ExcelUtil.GetNextIndex<SkillCfg>();
        ExcelUtil.SetCfgValue(skillCfg, nameof(SkillCfg.nSkillID), id);
        AddSkillItem(skillCfg);
        ExcelUtil.AddCfg<SkillCfg>(skillCfg);
        return skillCfg.nSkillID;
    }
    private void SaveSkillCfgData()
    {
        var count = ExcelUtil.GetCfgCount<SkillCfg>();
        for (int i = 0; i < count; i++)
        {
            var item = ExcelUtil.GetCfgByIndex<SkillCfg>(i);
            var key = item.GetID();
            var drawItem = m_DicSkilDrawData[key];
            var listData = new List<int>();
            drawItem.GetStringData(ref listData);

            ExcelUtil.SetCfgValue(item, nameof(item.arrParams), listData.ToArray());

        }
        ExcelUtil.SaveExcel<SkillCfg>();
    }

    private void SaveMonsterCfgData()
    {
        var monsterCfgCount = ExcelUtil.GetCfgCount<MonsterCfg>();
        for (int i = 0; i < monsterCfgCount; i++)
        {
            var monsterCfg = ExcelUtil.GetCfgByIndex<MonsterCfg>(i);
            var monsterViewInfo = _MonsterSkillViewInfos[monsterCfg.nMonsterID];

            if (monsterCfg.arrSkillGroup == null)
                ExcelUtil.SetCfgValue(monsterCfg, nameof(monsterCfg.arrSkillGroup), new int[0]);
            foreach (var item in monsterViewInfo.m_DicItemInfo)
            {
                var skillID = item.Key;
                if (item.Value.isDelect)
                {
                    if (monsterCfg.arrSkillGroup.Contains(skillID))
                    {
                        var list = monsterCfg.arrSkillGroup.ToList();
                        list.Remove(skillID);
                        ExcelUtil.SetCfgValue(monsterCfg, nameof(monsterCfg.arrSkillGroup), list.ToArray());
                    }
                }
                else
                {
                    if (!monsterCfg.arrSkillGroup.Contains(skillID))
                    {
                        var list = monsterCfg.arrSkillGroup.ToList();
                        list.Add(skillID);
                        ExcelUtil.SetCfgValue(monsterCfg, nameof(monsterCfg.arrSkillGroup), list.ToArray());
                    }
                }
            }
        }
        ExcelUtil.SaveExcel<MonsterCfg>();
    }

    private Vector2 m_ScrollPos1 = Vector2.zero;
    private Vector2 m_ScrollPos2 = Vector2.zero;
    private Vector2 m_ScrollPos3 = Vector2.zero;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            var style = GuiStyleUtil.CreateLayoutBoxBackgroud(new Color32(255 / 8, 255 / 8, 255 / 8, 255 / 8));

            EditorGUILayout.BeginVertical(style, GUILayout.Width(220));
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                {
                    if (_CurMonsterID > 0)
                    {
                        var monsterCfg = ExcelUtil.GetCfg<MonsterCfg>(_CurMonsterID);
                        EditorGUILayout.LabelField($"{monsterCfg.nMonsterID}:{monsterCfg.strName}", GUILayout.Width(100));
                    }
                    if (GUILayout.Button("Monster"))
                    {
                        ShowSelectMonsterMenu();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                {
                    if (GUILayout.Button("保存"))
                    {
                        SaveSkillCfgData();
                        SaveMonsterCfgData();
                    }
                    if (GUILayout.Button("重载"))
                    {
                        OnDisable();
                        OnEnable();
                    }
                }
                EditorGUILayout.EndHorizontal();


                if (_CurMonsterID > 0)
                {
                    var btnContent = new GUIContent()
                    {
                        text = _IsSimulation ? "取消预览" : "启用预览",
                    };
                    var btnStyle = GuiStyleUtil.CreateLayoutBoxBackgroud(new Color32(255, 255, 255, 255));
                    var btnRect = GUILayoutUtility.GetRect(1, 999, 1, 40, GUILayout.ExpandWidth(true));
                    EditorGUI.DrawRect(btnRect, _IsSimulation ? Color.green : Color.gray);
                    if (GUI.Button(btnRect, btnContent, btnStyle))
                    {
                        _IsSimulation = !_IsSimulation;
                        if (_IsSimulation)
                            CreatePlayable();
                        else
                            ClearPlayable();
                    }
                    GUILayout.Label("skill list");
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                    {
                        if (GUILayout.Button("添加"))
                        {
                            ShowAddSkillMenu();
                        }
                        if (GUILayout.Button("新建"))
                        {
                            ShowCreateSkillMenu();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    DrawSkillSelectView();
                }
            }
            EditorGUILayout.EndVertical();

            if (_CurMonsterID > 0)
            {
                EditorGUILayout.BeginVertical();
                {
                    DrawOpenSkillNavigation();

                    GUILayout.Space(20);

                    DrawSkillView();
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndHorizontal();
        //this.Repaint();
    }









    private void ShowAddSkillMenu()
    {
        var menu = new GenericMenu();
        var skillCount = ExcelUtil.GetCfgCount<SkillCfg>();
        for (int i = 0; i < skillCount; i++)
        {
            var item = ExcelUtil.GetCfgByIndex<SkillCfg>(i);
            var skillID = item.nSkillID;
            if (CurSkillViewInfo.m_DicItemInfo.ContainsKey(skillID))
                continue;
            var skillName = item.strName;
            var guiContent = new GUIContent()
            {
                text = $"{skillID}: {skillName}",
            };
            menu.AddItem(guiContent, false, () =>
            {
                CurSkillViewInfo.m_DicItemInfo.Add(skillID, new());
            });
        }
        menu.ShowAsContext();
    }
    private void ShowSelectMonsterMenu()
    {
        var menu = new GenericMenu();
        var count = ExcelUtil.GetCfgCount<MonsterCfg>();
        for (int i = 0; i < count; i++)
        {
            var monsterCfg = ExcelUtil.GetCfgByIndex<MonsterCfg>(i);
            var monsterID = monsterCfg.nMonsterID;
            var guiContent = new GUIContent()
            {
                text = $"{monsterID}: {monsterCfg.strName}",
            };
            menu.AddItem(guiContent, monsterID == _CurMonsterID, () =>
            {
                if (_CurMonsterID == monsterID)
                    return;
                _CurMonsterID = monsterID;
                if (_IsSimulation)
                {
                    ClearPlayable();
                    CreatePlayable();
                }

            });
        }
        menu.ShowAsContext();
    }
    private void ShowCreateSkillMenu()
    {
        var contents = new List<GUIContent>();
        var map = new Dictionary<string, EnSkillBoxType>();
        var menu = new GenericMenu();
        for (var i = EnSkillBoxType.None + 1; i < EnSkillBoxType.EnumCount; i++)
        {
            var boxType = i;
            var key = EditorUtil.GetEnumName(boxType);
            menu.AddItem(new() { text = key }, false, () =>
            {
                var skillID = AddSkillItem(boxType);

                CurSkillViewInfo.m_CurSelectID = skillID;
                CurSkillViewInfo.m_OpenList.Add(skillID);
                if (_IsSimulation)
                    InitSimulationData();
            });
        }

        menu.ShowAsContext();
    }

    private void DrawSkillSelectView()
    {
        m_ScrollPos1 = EditorGUILayout.BeginScrollView(m_ScrollPos1, GUILayout.Width(200));
        {
            foreach (var item in CurSkillViewInfo.m_DicItemInfo)
            {
                var key = item.Key;
                var itemInfo = item.Value;

                if (!ExcelUtil.Contains<SkillCfg>(key))
                {
                    GUILayout.Label($"error id:{key}");
                    itemInfo.isDelect = true;
                    continue;
                }
                var cfg = ExcelUtil.GetCfg<SkillCfg>(key);
                EditorGUILayout.BeginHorizontal();
                {
                    if (itemInfo.isDelect)
                    {
                        GUILayout.Label(cfg.strName);
                    }
                    else
                    {
                        if (GUILayout.Button(cfg.strName))
                        {
                            CurSkillViewInfo.m_CurSelectID = key;
                            m_ScrollPos3 = Vector3.zero;
                            if (!CurSkillViewInfo.m_OpenList.Contains(key))
                                CurSkillViewInfo.m_OpenList.Add(key);
                            if (_IsSimulation)
                                InitSimulationData();
                        }
                    }
                    if (GUILayout.Button(itemInfo.isDelect ? "❌" : "✅", GUILayout.Width(50)))
                    {
                        itemInfo.isDelect = !itemInfo.isDelect;
                        if (itemInfo.isDelect)
                        {
                            CurSkillViewInfo.m_OpenList.Remove(key);
                            if (CurSkillViewInfo.m_CurSelectID == key)
                            {
                                CurSkillViewInfo.m_CurSelectID = -1;
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawOpenSkillNavigation()
    {
        m_ScrollPos2 = EditorGUILayout.BeginScrollView(m_ScrollPos2, GUILayout.Height(30));
        {
            EditorGUILayout.BeginHorizontal();
            {
                for (var i = 0; i < CurSkillViewInfo.m_OpenList.Count; i++)
                {
                    var skillID = CurSkillViewInfo.m_OpenList[i];
                    var skillCfg = ExcelUtil.GetCfg<SkillCfg>(skillID);

                    var content = new GUIContent()
                    {
                        text = skillCfg.strName
                    };
                    var btnStyle = GuiStyleUtil.CreateButtonBackground();
                    if (CurSkillViewInfo.m_CurSelectID == skillID)
                    {
                        btnStyle.normal = btnStyle.active;
                        content.image = btnStyle.active.background;
                    }
                    if (GuiStyleUtil.DrawButton(content, null, () =>
                    {
                        if (skillID == CurSkillViewInfo.m_CurSelectID)
                        {
                            CurSkillViewInfo.m_CurSelectID = -1;
                        }
                        CurSkillViewInfo.m_OpenList.RemoveAt(i);
                        i--;
                    }))
                    {
                        CurSkillViewInfo.m_CurSelectID = skillID;
                        m_ScrollPos3 = Vector3.zero;

                        if (_IsSimulation)
                            InitSimulationData();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawSkillView()
    {
        if (_IsSimulation)
        {
            DrawSimulationView();
        }
        DrawSkillData();
    }

    private void DrawSkillData()
    {
        m_ScrollPos3 = EditorGUILayout.BeginScrollView(m_ScrollPos3);
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (m_DicSkilDrawData.TryGetValue(CurSkillViewInfo.m_CurSelectID, out var linkData))
                {
                    var cfg = ExcelUtil.GetCfg<SkillCfg>(CurSkillViewInfo.m_CurSelectID);
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            var labelContent = new GUIContent()
                            {
                                text = $"{cfg.nSkillID}",
                                image = EditorLoad.LoadTexture2D(EnEditorRes.btn_red),
                            };
                            EditorGUILayout.LabelField(labelContent, GUILayout.Width(30));
                            var strName = EditorGUILayout.TextField(cfg.strName, GUILayout.Width(200));
                            if (strName != cfg.strName)
                                ExcelUtil.SetCfgValue(cfg, nameof(cfg.strName), strName);
                        }
                        EditorGUILayout.EndHorizontal();

                        if (_IsSimulation && linkData is ISkillSimulationEditor simulationEditor)
                        {
                            simulationEditor.UpdateSimulation(_LastUpdateTime);
                        }
                        else
                        {
                            linkData.Draw();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

    }



    private float _CurSliderValue = 0;
    private bool _IsPlaying = false;
    private Tweener _TweenCore = null;

    private void DrawSimulationView()
    {

        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.ObjectField(_SimulationObj, typeof(GameObject), true, GUILayout.Width(100));
                EditorGUILayout.ObjectField(_PrefabObj, typeof(GameObject), true, GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                var style2 = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                };
                var labelRect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, GUILayout.Width(30), GUILayout.Height(20));

                GUI.Label(labelRect, "0", style2);

                var rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(20));

                var height = 1f;
                var lineRect = new Rect()
                {
                    position = rect.position + new Vector2(0, (rect.height - height) / 2),
                    size = new Vector2(rect.width, height),
                };
                EditorGUI.DrawRect(lineRect, Color.gray);

                var labelRect2 = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, GUILayout.Width(30), GUILayout.Height(20));
                GUI.Label(labelRect2, _MaxTime.ToString("0.0"), style2);

                var rectSlider = _LastUpdateTime / _MaxTime;
                var weight = 30f;
                var height2 = 15;
                var hitRect = new Rect()
                {
                    center = new Vector2(rect.position.x + rect.width * rectSlider - weight * 0.5f, rect.position.y + (rect.height - height2) / 2),
                    size = new Vector2(weight, height2),
                };
                EditorGUI.DrawRect(hitRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
                var style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.green },
                };
                GUI.Label(hitRect, $"{_LastUpdateTime:0.0}", style);
            }
            EditorGUILayout.EndHorizontal();

            {
                var rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(50));
                _CurSliderValue = GuiStyleUtil.DrawSlider(_CurSliderValue, rect);
            }

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            {

                var playRect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.button, GUILayout.Width(100), GUILayout.Height(30));

                var title = _IsPlaying ? "Stop" : "Pause";
                var bgColor = _IsPlaying ? Color.yellow : Color.grey;
                var txtColor = _IsPlaying ? Color.blue : Color.white;
                if (GuiStyleUtil.DrawButton(playRect, bgColor, title, txtColor))
                {
                    _IsPlaying = !_IsPlaying;
                    KillDotween();
                    if (_IsPlaying)
                    {
                        var start = _CurSliderValue;
                        var end = 1f;
                        var time = _MaxTime * (1 - start);
                        var lastTime = start;

                        _TweenCore = DOTween.To(() => start, value =>
                        {
                            var deltaTime = value * _MaxTime - lastTime;
                            lastTime = value * _MaxTime;
                            _CurSliderValue = value;
                            //SceneView.RepaintAll();
                            this.Repaint();
                        }, end, time)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                KillDotween();
                                _IsPlaying = false;
                            });
                        DOTweenEditorPreview.PrepareTweenForPreview(_TweenCore, false);
                        DOTweenEditorPreview.Start();

                    }
                }

                GUILayout.Space(10);

                var rect2 = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.button, GUILayout.Width(100), GUILayout.Height(30));
                if (GuiStyleUtil.DrawButton(rect2, Color.gray, "Reset", Color.white))
                {
                    _CurSliderValue = 0;
                    _IsPlaying = false;
                    _PrefabObj.transform.localPosition = Vector3.zero;
                    KillDotween();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();


        UpdateGraphTime();
    }


    private PlayableGraph _Graph;
    public static float _MaxTime = 10f;
    public static float _LastUpdateTime = 0;
    public static GameObject _SimulationObj = null;
    public static GameObject _PrefabObj = null;

    private void KillDotween()
    {
        if (_TweenCore == null)
            return;
        _TweenCore.Pause();
        _TweenCore.Kill();
        DOTweenEditorPreview.Stop();
        _TweenCore = null;
    }
    private void ClearPlayable()
    {
        if (_Graph.IsValid())
        {
            _Graph.Destroy();
        }
        if (_SimulationObj != null)
        {
            GameObject.DestroyImmediate(_SimulationObj);
            _SimulationObj = null;
            _PrefabObj = null;
        }
    }


    private void UpdateGraphTime()
    {
        if (!_Graph.IsValid())
            return;

        var curTime = _CurSliderValue * _MaxTime;
        var deltaTime = curTime - _LastUpdateTime;
        _Graph.Evaluate(deltaTime);
        _LastUpdateTime = curTime;


        if (SceneView.lastActiveSceneView != null)
        {
            var offset = SceneView.lastActiveSceneView.rotation * Vector3.forward;
            SceneView.lastActiveSceneView.pivot = _PrefabObj.transform.position + offset * 2 + Vector3.up * 1;
        }
    }

    private void InitSimulationData()
    {
        if (!m_DicSkilDrawData.TryGetValue(CurSkillViewInfo.m_CurSelectID, out var data))
            return;
        if (data is not ISkillSimulationEditor editor)
            return;
        _MaxTime = editor.GetMaxSimulationTime();
        editor.InitSimulation(ref _Graph);
    }
    private void CreatePlayable()
    {
        var curAssetID = CurMonsterCfg.nAssetCfgID;
        var assetInfo = ExcelUtil.GetCfg<AssetCfg>(curAssetID);
        if (assetInfo == null)
        {
            Debug.LogError($"monster id: [ {_CurMonsterID} ], asset no exist assetcfg id: [ {curAssetID} ]");
            return;
        }

        _SimulationObj = new GameObject("Simulation");

        var prefabAss = AssetDatabase.LoadAssetAtPath<GameObject>(assetInfo.strPath);

        _PrefabObj = GameObject.Instantiate(prefabAss, _SimulationObj.transform);
        var animator = _PrefabObj.GetComponent<Animator>();

        //var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Resources/Anim/Rest_run.FBX");
        //AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[0]);

        _Graph = PlayableGraph.Create("Simulation");
        _Graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        var output = AnimationPlayableOutput.Create(_Graph, "", animator);
        //var clipPlayable = AnimationClipPlayable.Create(_Graph, clip);

        //output.SetSourcePlayable(clipPlayable, 0);
        _Graph.Play();

        InitSimulationData();
    }
}
