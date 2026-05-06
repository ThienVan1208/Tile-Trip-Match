using System;
using UnityEngine;

namespace TileTrip.Tiles.Data
{
    [Serializable]
    public struct TileSpawnData
    {
        public int GridX;
        public int GridY;
        public int LayerIndex;

        public TileSpawnData(int gridX, int gridY, int layerIndex)
        {
            GridX = gridX;
            GridY = gridY;
            LayerIndex = layerIndex;
        }
    }
}
