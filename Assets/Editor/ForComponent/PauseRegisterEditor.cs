using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PauseRegister))]
public class PauseRegisterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "【Pause機能の使い方】\n" +
            "1. PauseしたいMonoBehaviourにIPausableを実装\n" +
            "2. Updateに書くものを、OnCustomUpdateに書く\n" +
            "3. PauseRegisterをアタッチ\n" +
            "4. IPausableが実装されたコンポーネントをPauseRegisterにセット",
            MessageType.Info
        );
    }
}
