#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public class GameSettingsWindow : EditorWindow
{
    private EditGameConfig selectedConfig;
    private Vector2 scrollPos;
    private Editor configEditor;
    private bool showConfigEditor = false;

    [MenuItem("Window/Game Settings Window")]
    public static void ShowWindow()
    {
        GetWindow<GameSettingsWindow>("Game Settings");
    }

    private void OnDisable()
    {
        DestroyImmediate(configEditor);
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Configuration", EditorStyles.boldLabel);

        var configs = AssetDatabase.FindAssets("t:EditGameConfig")
            .Select(guid => AssetDatabase.LoadAssetAtPath<EditGameConfig>(
                AssetDatabase.GUIDToAssetPath(guid)))
            .ToList();

        EditorGUI.BeginChangeCheck();
        selectedConfig = (EditGameConfig)EditorGUILayout.ObjectField(
            "Selected Config",
            selectedConfig,
            typeof(EditGameConfig),
            false);

        if (EditorGUI.EndChangeCheck())
        {
            showConfigEditor = false;
            DestroyImmediate(configEditor);
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
        foreach (var config in configs)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(config.configName);

            if (GUILayout.Button("Select", GUILayout.Width(80)))
            {
                selectedConfig = config;
                showConfigEditor = false;
                DestroyImmediate(configEditor);
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Edit", GUILayout.Width(80)))
            {
                selectedConfig = config;
                showConfigEditor = true;
                DestroyImmediate(configEditor);
                configEditor = Editor.CreateEditor(selectedConfig);
                GUI.FocusControl(null);
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (showConfigEditor && selectedConfig != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Config Settings", EditorStyles.boldLabel);

            if (configEditor == null)
            {
                configEditor = Editor.CreateEditor(selectedConfig);
            }

            configEditor.OnInspectorGUI();
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Apply to Scene"))
        {
            ApplyToScene();
            showConfigEditor = false;
            DestroyImmediate(configEditor);
        }

        if (GUILayout.Button("Create New"))
        {
            CreateNewConfig();
            showConfigEditor = true;
        }

        if (GUILayout.Button("Save All"))
        {
            AssetDatabase.SaveAssets();
            showConfigEditor = false;
            DestroyImmediate(configEditor);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ApplyToScene()
    {
        if (selectedConfig == null) return;

        var selector = FindObjectOfType<ConfigSelector>();
        if (selector != null)
        {
            selector.ApplyConfig(selectedConfig);
            EditorUtility.SetDirty(selector);
        }
    }

    private void CreateNewConfig()
    {
        var config = ScriptableObject.CreateInstance<EditGameConfig>();
        string path = "Assets/Resources/EditGameConfig_New.asset";
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        selectedConfig = config;

        DestroyImmediate(configEditor);
        configEditor = Editor.CreateEditor(selectedConfig);
    }
}
#endif
