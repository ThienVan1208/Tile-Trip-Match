using System.Collections.Generic;
using UnityEngine;
namespace TileAdventure
{
    public class BoardData
    {
        private Dictionary<Vector3Int, TileData> _tilesMap;
        public bool IsEmpty => _tilesMap.Count == 0;
        public BoardData(List<TileData> allTiles)
        {
            _tilesMap = new Dictionary<Vector3Int, TileData>();
            foreach (var tile in allTiles)
            {
                _tilesMap[new Vector3Int(tile.GridX, tile.GridY, tile.Layer)] = tile;
                tile.CoveringTilesCount = 0;
            }
            CalculateInitialOverlaps();
        }
        private void CalculateInitialOverlaps()
        {
            foreach (var tile in _tilesMap.Values)
            {
                List<Vector3Int> underlyingPos = GetUnderlyingPositions(tile);
                foreach (var pos in underlyingPos)
                {
                    if (_tilesMap.TryGetValue(pos, out TileData underlyingTile))
                    {
                        underlyingTile.CoveringTilesCount++;
                    }
                }
            }
        }
        public void RemoveTile(TileData tile)
        {
            Vector3Int key = new Vector3Int(tile.GridX, tile.GridY, tile.Layer);
            if (_tilesMap.ContainsKey(key))
            {
                _tilesMap.Remove(key);
                List<Vector3Int> underlyingPositions = GetUnderlyingPositions(tile);
                foreach (var pos in underlyingPositions)
                {
                    if (_tilesMap.TryGetValue(pos, out TileData underlyingTile))
                    {
                        underlyingTile.CoveringTilesCount--;
                    }
                }
            }
        }
        private List<Vector3Int> GetUnderlyingPositions(TileData tile)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            int currentLayer = tile.Layer;
            if (currentLayer <= 0) return result;
            int lowerLayer = currentLayer - 1;
            int x = tile.GridX;
            int y = tile.GridY;
            bool isCurrentOdd = (currentLayer % 2 != 0);
            if (isCurrentOdd)
            {
                result.Add(new Vector3Int(x, y, lowerLayer));
                result.Add(new Vector3Int(x + 1, y, lowerLayer));
                result.Add(new Vector3Int(x, y + 1, lowerLayer));
                result.Add(new Vector3Int(x + 1, y + 1, lowerLayer));
            }
            else
            {
                result.Add(new Vector3Int(x, y, lowerLayer));
                result.Add(new Vector3Int(x - 1, y, lowerLayer));
                result.Add(new Vector3Int(x, y - 1, lowerLayer));
                result.Add(new Vector3Int(x - 1, y - 1, lowerLayer));
            }
            return result;
        }
    }
}