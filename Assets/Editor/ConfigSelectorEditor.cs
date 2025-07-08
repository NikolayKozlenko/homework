#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConfigSelector))]
public class ConfigSelectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ConfigSelector selector = (ConfigSelector)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Access", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Game Settings"))
        {
            GameSettingsWindow.ShowWindow();
        }

        if (selector.currentConfig != null &&
            GUILayout.Button("Edit Current Config"))
        {
            Selection.activeObject = selector.currentConfig;
        }
    }
}
#endif