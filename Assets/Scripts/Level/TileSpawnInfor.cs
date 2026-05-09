using System;
namespace TileAdventure
{
    [Serializable]
    public struct TileSpawnInfor
    {
        public int GridX;
        public int GridY;
        public int LayerIndex;
        public int IconId; 
        public TileSpawnInfor(int gridX, int gridY, int layerIndex, int iconId = 0)
        {
            GridX = gridX;
            GridY = gridY;
            LayerIndex = layerIndex;
            IconId = iconId;
        }
    }
}
