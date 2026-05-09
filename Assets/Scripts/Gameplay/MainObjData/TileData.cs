using System;
namespace TileAdventure
{
    public class TileData
    {
        public string Id { get; }
        public int IconId { get; }
        public int GridX { get; }
        public int GridY { get; }
        public int Layer { get; }
        private int _coveringTilesCount;
        public int CoveringTilesCount
        {
            get => _coveringTilesCount;
            set
            {
                _coveringTilesCount = value;
                OnCoveringCountChanged?.Invoke(_coveringTilesCount);
            }
        }
        public bool IsExposed => _coveringTilesCount == 0;
        public event Action<int> OnCoveringCountChanged;
        public TileData(string id, int iconId, int x, int y, int layer)
        {
            Id = id; 
            IconId = iconId; 
            GridX = x; 
            GridY = y; 
            Layer = layer;
        }
    }
}