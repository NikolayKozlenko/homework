using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Text _statusText;
    [SerializeField] private Text _fpsText;
    [SerializeField] private Button _generateButton;

    [Header("Config Settings")]
    [SerializeField] private int _levels = 30;
    [SerializeField][Range(10, 100)] private int _enemiesPerLevel = 100;
    [SerializeField][Range(1, 20)] private int _dropsPerEnemy = 3;

    private string ConfigPath => Path.Combine(Application.streamingAssetsPath, "game_config.json");

    private void Start()
    {
        SetupFpsMonitor();
        SetupGenerateButton();
        CheckConfigExists();
    }

    private void SetupFpsMonitor()
    {
        Observable.Interval(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => UpdateFps())
            .AddTo(this);
    }

    private void SetupGenerateButton()
    {
        _generateButton.OnClickAsObservable()
            .Subscribe(_ => StartCoroutine(GenerateConfigCoroutine()))
            .AddTo(this);
    }

    private void UpdateFps()
    {
        float fps = 1.0f / Time.unscaledDeltaTime;
        _fpsText.text = $"FPS: {fps:00}";
        _fpsText.color = GetFpsColor(fps);
    }

    private Color GetFpsColor(float fps)
    {
        return fps < 30 ? Color.red :
               fps < 50 ? Color.yellow :
               Color.green;
    }

    private void CheckConfigExists()
    {
        _statusText.text = File.Exists(ConfigPath) ? "Config ready" : "No config found";
    }

    private System.Collections.IEnumerator GenerateConfigCoroutine()
    {
        _generateButton.interactable = false;
        ResetUI();

        yield return StartCoroutine(CreateConfigFile());

        CompleteGeneration();
        _generateButton.interactable = true;
    }

    private void ResetUI()
    {
        _statusText.text = "Generating...";
        _progressBar.value = 0;
    }

    private System.Collections.IEnumerator CreateConfigFile()
    {
        var config = new GameConfig
        {
            gameTitle = "Dynamic Game Config",
            levels = new GameConfig.Level[_levels]
        };

        for (int i = 0; i < _levels; i++)
        {
            config.levels[i] = CreateLevel(i);
            UpdateProgress(i);
            yield return null;
        }

        SaveConfigToFile(config);
    }

    private GameConfig.Level CreateLevel(int levelIndex)
    {
        return new GameConfig.Level
        {
            id = levelIndex,
            name = $"Level_{levelIndex}",
            enemies = Enumerable.Range(0, _enemiesPerLevel)
                .Select(j => CreateEnemy())
                .ToArray()
        };
    }

    private GameConfig.Enemy CreateEnemy()
    {
        return new GameConfig.Enemy
        {
            type = $"enemy_{UnityEngine.Random.Range(1, 10)}",
            health = UnityEngine.Random.Range(30, 100),
            drops = GenerateRandomDrops()
        };
    }

    private string[] GenerateRandomDrops()
    {
        return Enumerable.Range(0, _dropsPerEnemy)
            .Select(_ => $"item_{UnityEngine.Random.Range(1, 100)}")
            .ToArray();
    }

    private void UpdateProgress(int currentLevel)
    {
        _progressBar.value = (float)currentLevel / _levels;
        _statusText.text = $"Generating... {_progressBar.value:P0}";
    }

    private void SaveConfigToFile(GameConfig config)
    {
        Directory.CreateDirectory(Application.streamingAssetsPath);
        File.WriteAllText(ConfigPath, JsonUtility.ToJson(config, true));
    }

    private void CompleteGeneration()
    {
        _statusText.text = "Generation complete!";
        _progressBar.value = 1f;
    }
}
