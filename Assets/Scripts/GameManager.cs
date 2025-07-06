using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using Unity.VisualScripting;
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
    [SerializeField][Range(10, 100)] private int _enemiesPerLevel = 50;
    [SerializeField][Range(1, 20)] private int _dropsPerEnemy = 5;

    private string ConfigPath => Path.Combine(Application.streamingAssetsPath, "game_config.json");
    private CancellationTokenSource _cts;
    private System.Random _random = new System.Random();

    private void Start()
    {
        SetupFpsMonitor();
        SetupGenerateButton();
        CheckConfigExists();
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
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
            .Subscribe(_ => GenerateConfig().Forget())
            .AddTo(this);
    }

    private async UniTaskVoid GenerateConfig()
    {
        _generateButton.interactable = false;
        _cts = new CancellationTokenSource();
        ResetUI();

        try
        {
            var config = await GenerateConfigDataAsync(_cts.Token);
            await SaveConfigAsync(config, _cts.Token);
            CompleteGeneration();
        }
        catch (OperationCanceledException)
        {
            _statusText.text = "Generation canceled";
        }
        catch (Exception e)
        {
            _statusText.text = $"Error: {e.Message}";
            Debug.LogError(e);
        }
        finally
        {
            _generateButton.interactable = true;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private async UniTask<GameConfig> GenerateConfigDataAsync(CancellationToken ct)
    {
        var config = new GameConfig
        {
            gameTitle = "Generated Config",
            levels = new GameConfig.Level[_levels]
        };

        var progress = new Progress<float>(p =>
        {
            _progressBar.value = p;
            _statusText.text = $"Generating... {p:P0}";
        });

        await Task.Run(() =>
        {
            for (int i = 0; i < _levels; i++)
            {
                ct.ThrowIfCancellationRequested();
                config.levels[i] = CreateLevel(i);
                ((IProgress<float>)progress).Report((float)i / _levels);
            }
            return config;
        }, ct);

        return config;
    }

    private GameConfig.Level CreateLevel(int levelIndex)
    {
        return new GameConfig.Level
        {
            id = levelIndex,
            name = $"Level_{levelIndex}",
            enemies = Enumerable.Range(0, _enemiesPerLevel)
                .Select(_ => CreateEnemy())
                .ToArray()
        };
    }

    private GameConfig.Enemy CreateEnemy()
    {
        return new GameConfig.Enemy
        {
            type = $"enemy_{_random.Next(1, 10)}", // Используем System.Random вместо UnityEngine.Random
            health = _random.Next(30, 100),
            drops = GenerateRandomDrops()
        };
    }

    private string[] GenerateRandomDrops()
    {
        return Enumerable.Range(0, _dropsPerEnemy)
            .Select(_ => $"item_{_random.Next(1, 100)}")
            .ToArray();
    }

    private async UniTask SaveConfigAsync(GameConfig config, CancellationToken ct)
    {
        string json = JsonUtility.ToJson(config, true);

        await UniTask.SwitchToThreadPool();
        Directory.CreateDirectory(Application.streamingAssetsPath);
        await File.WriteAllTextAsync(ConfigPath, json, ct);
        await UniTask.SwitchToMainThread();
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

    private void ResetUI()
    {
        _statusText.text = "Generating...";
        _progressBar.value = 0;
    }

    private void CompleteGeneration()
    {
        _statusText.text = "Generation complete!";
        _progressBar.value = 1f;

    }
}