using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StatusWatcher : EditorWindow
{
    private static readonly StatusMethod[] Methods =
    {
        StatusMethod.Base,
        StatusMethod.Add,
        StatusMethod.Multiply
    };

    [MenuItem("Usty/Status Watcher")]
    private static void Open()
    {
        GetWindow<StatusWatcher>("Status Watcher");
    }

    private void OnEnable()
    {
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    private void OnGUI()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("ゲーム再生中に対象オブジェクトを選択してください。", MessageType.Info);
            return;
        }

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            EditorGUILayout.HelpBox("StatusContainerを持つオブジェクトを選択してください。", MessageType.Info);
            return;
        }

        StatusContainer container = selectedObject.GetComponent<StatusContainer>();
        if (container == null)
        {
            EditorGUILayout.HelpBox("選択中のオブジェクトにStatusContainerがありません。", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField(selectedObject.name, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("BASE", new GUIStyle(EditorStyles.label) { normal = { textColor = Color.red } });
        DrawStatusMatrix(container.statusVector);
        EditorGUILayout.LabelField("BUFF", new GUIStyle(EditorStyles.label) { normal = { textColor = Color.green } });
        DrawStatusMatrix(container.modifierContainer.BuffedStatusVector());
        EditorGUILayout.LabelField("MODIFIED", new GUIStyle(EditorStyles.label) { normal = { textColor = Color.blue } });
        DrawStatusMatrix(container.GetStatus());

        DrawBuffs(container.modifierContainer.Buffs);
        DrawEffects(container.modifierContainer.Effects);
    }

    private void DrawBuffs(IReadOnlyDictionary<ModifierKey, Modifier> buffs)
    {
        DrawModifierList("BUFFS", buffs);
    }

    private void DrawEffects(IReadOnlyDictionary<ModifierKey, Modifier> effects)
    {
        DrawModifierList("EFFECTS", effects);
    }

    private void DrawModifierList(string title, IReadOnlyDictionary<ModifierKey, Modifier> modifiers)
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        if (modifiers.Count == 0)
        {
            EditorGUILayout.LabelField("なし", EditorStyles.miniLabel);
            return;
        }

        foreach (KeyValuePair<ModifierKey, Modifier> pair in modifiers)
        {
            Modifier modifier = pair.Value;
            string definitionName = modifier.Definition != null ? modifier.Definition.name : "不明なModifier";

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(definitionName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("付与元: " + modifier.SourceName, EditorStyles.miniLabel);
            DrawProgressBar(
                "Duration",
                modifier.Definition.duration > 0f
                    ? modifier.RemainingTime / modifier.Definition.duration
                    : 0f,
                Mathf.Max(0f, modifier.RemainingTime),
                modifier.Definition.duration);
            if (modifier.Definition.interval > 0f)
            {
                DrawProgressBar(
                    "次回発動まで",
                    1f - modifier.IntervalTimer / modifier.Definition.interval,
                    Mathf.Max(0f, modifier.IntervalTimer),
                    modifier.Definition.interval);
            }
            else
            {
                EditorGUILayout.LabelField("Interval: なし（常時発動）", EditorStyles.miniLabel);
            }
            DrawModifierValues(modifier.GetModifiers());
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawProgressBar(string label, float progress, float currentTime, float totalTime)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
        EditorGUI.ProgressBar(
            rect,
            Mathf.Clamp01(progress),
            label + ": " + currentTime.ToString("0.0") + " / " + totalTime.ToString("0.0") + "秒");
    }

    private void DrawModifierValues(List<StatusModifier> statusModifiers)
    {
        EditorGUILayout.Space(4);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Category", EditorStyles.boldLabel, GUILayout.MinWidth(80));
        GUILayout.Label("Method", EditorStyles.boldLabel, GUILayout.MinWidth(70));
        GUILayout.Label("Value", EditorStyles.boldLabel, GUILayout.MinWidth(60));
        EditorGUILayout.EndHorizontal();

        if (statusModifiers == null || statusModifiers.Count == 0)
        {
            EditorGUILayout.LabelField("補正なし", EditorStyles.miniLabel);
            return;
        }

        foreach (StatusModifier statusModifier in statusModifiers)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(statusModifier.category.ToString(), GUILayout.MinWidth(80));
            GUILayout.Label(statusModifier.method.ToString(), GUILayout.MinWidth(70));
            GUILayout.Label(statusModifier.value.ToString("0.###"), GUILayout.MinWidth(60));
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawStatusMatrix(StatusVector status)
    {
        if (status == null)
        {
            EditorGUILayout.HelpBox("StatusContainerのステータスが初期化されていません。", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(4);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Category", EditorStyles.boldLabel, GUILayout.MinWidth(80));
        foreach (StatusMethod method in Methods)
        {
            GUILayout.Label(method.ToString(), EditorStyles.boldLabel, GUILayout.MinWidth(70));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.DrawRect(
            GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true)),
            EditorGUIUtility.isProSkin ? new Color(0.35f, 0.35f, 0.35f) : new Color(0.7f, 0.7f, 0.7f));

        for (int categoryIndex = 0; categoryIndex < (int)StatusCategory.Count; categoryIndex++)
        {
            StatusCategory category = (StatusCategory)categoryIndex;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(category.ToString(), GUILayout.MinWidth(80));
            foreach (StatusMethod method in Methods)
            {
                EditorGUILayout.LabelField(status.Get(category, method).ToString("0.###"), GUILayout.MinWidth(70));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.DrawRect(
                GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true)),
                EditorGUIUtility.isProSkin ? new Color(0.35f, 0.35f, 0.35f) : new Color(0.7f, 0.7f, 0.7f));
        }
    }
}
