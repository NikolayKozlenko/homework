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

    [MenuItem("Tools/Game Settings")]
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

        // Выбор конфига
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

        // Список всех конфигов
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
        foreach (var config in configs)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(config.configName);

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                selectedConfig = config;
                showConfigEditor = false;
                DestroyImmediate(configEditor);
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Edit", GUILayout.Width(60)))
            {
                selectedConfig = config;
                showConfigEditor = true;
                DestroyImmediate(configEditor);
                configEditor = Editor.CreateEditor(selectedConfig);
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog("Delete Config",
                    $"Are you sure you want to delete '{config.configName}'?",
                    "Delete", "Cancel"))
                {
                    string path = AssetDatabase.GetAssetPath(config);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                    if (selectedConfig == config)
                    {
                        selectedConfig = null;
                        showConfigEditor = false;
                        DestroyImmediate(configEditor);
                    }
                    GUIUtility.ExitGUI(); // Выход из GUI из-за изменения коллекции
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        // Отображение редактора конфига
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

        // Кнопки управления
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
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/EditGameConfig.asset");

        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        selectedConfig = config;

        DestroyImmediate(configEditor);
        configEditor = Editor.CreateEditor(selectedConfig);
    }
}
#endif
