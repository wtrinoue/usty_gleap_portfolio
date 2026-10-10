using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatusContainer))]
public class StatusContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "【StatusMatrixの作成方法】\n" +
            "1. Projectの上で右クリック\n" +
            "2. バーの中でCreate->Status->Status Matrix\n" +
            "3. Inspectorで数値を入れる\n" +
            "4. StatusContainerにセットする",
            MessageType.Info
        );
    }
}