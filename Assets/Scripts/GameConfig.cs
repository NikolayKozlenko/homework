using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameConfig 
{
    public string gameTitle;
    public Level[] levels;

    [System.Serializable]
    public class Level
    {
        public int id;
        public string name;
        public Enemy[] enemies;
    }

    [System.Serializable]
    public class Enemy
    {
        public string type;
        public int health;
        public string[] drops;
    }
}
