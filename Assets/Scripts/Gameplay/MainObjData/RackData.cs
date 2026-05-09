using System.Collections.Generic;
namespace TileAdventure
{
    public class RackData
    {
        private const int MAX_CAPACITY = Constant.MAX_RACK_CAPACITY;
        private List<TileData> _tiles = new List<TileData>();
        public List<TileData> Tiles => _tiles;
        public bool IsFull => _tiles.Count >= MAX_CAPACITY;
        public int TryAddTile(TileData tile)
        {
            if (IsFull) return -1;
            int insertIndex = _tiles.Count;
            for (int i = 0; i < _tiles.Count; i++)
            {
                if (_tiles[i].IconId == tile.IconId)
                {
                    insertIndex = i + 1;
                    while (insertIndex < _tiles.Count && _tiles[insertIndex].IconId == tile.IconId)
                    {
                        insertIndex++;
                    }
                    break;
                }
            }
            _tiles.Insert(insertIndex, tile);
            return insertIndex;
        }
        public List<TileData> CheckForMatch()
        {
            if (_tiles.Count < Constant.TILE_MATCH_NUMBER) return null;
            for (int i = 0; i <= _tiles.Count - Constant.TILE_MATCH_NUMBER; i++)
            {
                if (_tiles[i].IconId == _tiles[i + 1].IconId && _tiles[i].IconId == _tiles[i + 2].IconId)
                {
                    var matchedTiles = new List<TileData> { _tiles[i], _tiles[i + 1], _tiles[i + 2] };
                    _tiles.RemoveRange(i, Constant.TILE_MATCH_NUMBER);
                    return matchedTiles;
                }
            }
            return null;
        }
    }
}