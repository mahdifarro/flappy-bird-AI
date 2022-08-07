using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levels", menuName = "ScriptableObjects/Levels", order = 1)]
public class Levels : ScriptableObject
{
    [Serializable]
    public class Level
    {
        public float levelSpeed; // in unit/s
        public int obstaclesCount;
        public float obstaclesOffset; // in unit
        public float restTime;
        public float changeDirecrionChance;
    }

    public List<Level> levelsList;
}

