using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ComponentRegisterWindow : EditorWindow
{
    private ComponentRegister componentRegister;
    private Vector2 scrollPosition;

    [MenuItem("Usty/Component Register")]
    private static void Open()
    {
        GetWindow<ComponentRegisterWindow>("Component Register");
    }

    private void OnEnable()
    {
        LoadComponentRegister();
    }

    private void LoadComponentRegister()
    {
        MonoScript windowScript = MonoScript.FromScriptableObject(this);
        string windowPath = AssetDatabase.GetAssetPath(windowScript);
        string windowDirectory = Path.GetDirectoryName(windowPath);
        if (string.IsNullOrEmpty(windowDirectory))
        {
            return;
        }

        windowDirectory = windowDirectory.Replace('\\', '/');
        string defaultAssetPath = windowDirectory + "/ComponentRegister.asset";
        componentRegister = AssetDatabase.LoadAssetAtPath<ComponentRegister>(defaultAssetPath);
        if (componentRegister != null)
        {
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:ComponentRegister", new[] { windowDirectory });

        if (guids.Length == 0)
        {
            guids = AssetDatabase.FindAssets("t:ComponentRegister");
        }

        if (guids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            componentRegister = AssetDatabase.LoadAssetAtPath<ComponentRegister>(assetPath);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        componentRegister = (ComponentRegister)EditorGUILayout.ObjectField(
            componentRegister,
            typeof(ComponentRegister),
            false,
            GUILayout.MinWidth(160));

        if (GUILayout.Button("Create Register", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            CreateComponentRegister();
        }

        EditorGUILayout.EndHorizontal();

        if (componentRegister == null)
        {
            EditorGUILayout.HelpBox(
                "ComponentRegister アセットを選択するか、Create Register から作成してください。",
                MessageType.Info);
            return;
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "スクリプトを Inspector にドラッグすると、選択中の GameObject または Prefab に追加できます。",
            MessageType.Info);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        foreach (ComponentCategory category in Enum.GetValues(typeof(ComponentCategory)))
        {
            DrawCategory(category);
        }
        EditorGUILayout.EndScrollView();
    }

    private void CreateComponentRegister()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Component Register",
            "ComponentRegister",
            "asset",
            "ComponentRegister アセットの保存先を選択してください。");

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        ComponentRegister newRegister = CreateInstance<ComponentRegister>();
        AssetDatabase.CreateAsset(newRegister, path);
        AssetDatabase.SaveAssets();
        componentRegister = newRegister;
        EditorGUIUtility.PingObject(componentRegister);
    }

    private void DrawCategory(ComponentCategory category)
    {
        List<ComponentScriptEntry> entries = GetScriptsForCategory(category);
        EditorGUILayout.LabelField(category.ToString(), EditorStyles.boldLabel);
        Rect lineRect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(lineRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));

        if (entries.Count == 0)
        {
            EditorGUILayout.LabelField("（登録なし）", EditorStyles.miniLabel);
            EditorGUILayout.Space(6);
            return;
        }

        foreach (ComponentScriptEntry entry in entries)
        {
            DrawScriptItem(entry);
        }

        EditorGUILayout.Space(6);
    }

    private List<ComponentScriptEntry> GetScriptsForCategory(ComponentCategory category)
    {
        List<ComponentScriptEntry> entries = new List<ComponentScriptEntry>();
        HashSet<MonoScript> seenScripts = new HashSet<MonoScript>();

        foreach (ComponentRegisterScript registeredScript in componentRegister.scripts)
        {
            if (registeredScript == null ||
                registeredScript.category != category ||
                !IsAttachableScript(registeredScript.script) ||
                !seenScripts.Add(registeredScript.script))
            {
                continue;
            }

            entries.Add(new ComponentScriptEntry(registeredScript.script));
        }

        return entries;
    }

    private static bool IsAttachableScript(MonoScript script)
    {
        if (script == null)
        {
            return false;
        }

        Type scriptType = script.GetClass();
        return scriptType != null &&
               typeof(MonoBehaviour).IsAssignableFrom(scriptType) &&
               !scriptType.IsAbstract &&
               !scriptType.ContainsGenericParameters;
    }

    private static void DrawScriptItem(ComponentScriptEntry entry)
    {
        Rect itemRect = GUILayoutUtility.GetRect(1, 34, GUILayout.ExpandWidth(true));
        GUI.Box(itemRect, GUIContent.none, EditorStyles.helpBox);

        Texture icon = AssetPreview.GetMiniThumbnail(entry.script);
        Rect iconRect = new Rect(itemRect.x + 5, itemRect.y + 5, 24, 24);
        GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
        GUI.Label(
            new Rect(itemRect.x + 36, itemRect.y + 3, itemRect.width - 42, 28),
            entry.script.name,
            EditorStyles.label);

        Event currentEvent = Event.current;
        if (!itemRect.Contains(currentEvent.mousePosition))
        {
            return;
        }

        if (currentEvent.type == EventType.MouseDown &&
            currentEvent.button == 0 &&
            currentEvent.clickCount >= 2)
        {
            Selection.activeObject = entry.script;
            EditorGUIUtility.PingObject(entry.script);
            currentEvent.Use();
        }
        else if (currentEvent.type == EventType.MouseDrag && currentEvent.button == 0)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new UnityEngine.Object[] { entry.script };
            DragAndDrop.StartDrag(entry.script.name);
            currentEvent.Use();
        }
    }

    private struct ComponentScriptEntry
    {
        public readonly MonoScript script;

        public ComponentScriptEntry(MonoScript script)
        {
            this.script = script;
        }
    }
}
