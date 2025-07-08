using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EditGameConfig", menuName = "Game/Config")]
public class EditGameConfig : ScriptableObject
{
    public string configName = "New Config";
    public int startLevel = 1;
    public float difficulty = 1.0f;
    [TextArea] public string description;

    public string GetFormattedInfo()
    {
        return $"<b>{configName}</b>\n" +
               $"Start Level: {startLevel}\n" +
               $"Difficulty: {difficulty}x\n" +
               $"{description}";
    }
}