using System.Collections.Generic;
using UnityEngine;
namespace TileAdventure
{
    [CreateAssetMenu(fileName = "Level_", menuName = "LevelData/LevelData", order = 1)]
    public class LevelDataSO : ScriptableObject
    {
        [Header("Level Info")]
        public int LevelId;
        public int TargetMatches = 15; 
        [Header("Board Layout")]
        public List<TileSpawnInfor> layoutConfigurations = new List<TileSpawnInfor>();
    }
}
