using System.Collections.Generic;
using UnityEngine;

namespace TileTrip.Tiles.Data
{
    [CreateAssetMenu(fileName = "Level_", menuName = "TileTrip/LevelData", order = 1)]
    public class LevelDataSO : ScriptableObject
    {
        [Header("Level Info")]
        public int LevelId;
        public int TargetMatches = 15; 
        
        [Header("Board Layout")]
        public List<TileSpawnData> layoutConfigurations = new List<TileSpawnData>();
    }
}
