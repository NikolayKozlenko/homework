using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConfigSelector : MonoBehaviour
{
    public EditGameConfig currentConfig;
    [SerializeField] private Text configDisplayText;

    public void ApplyConfig(EditGameConfig config)
    {
        currentConfig = config;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (configDisplayText != null && currentConfig != null)
        {
            configDisplayText.text = currentConfig.GetFormattedInfo();
        }
    }
}    

