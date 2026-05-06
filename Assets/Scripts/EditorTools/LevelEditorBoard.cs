using System.Collections.Generic;
using UnityEngine;
using TileTrip.Tiles.Data;

namespace TileTrip.EditorTools
{
    /// <summary>
    /// Quản lý lưới 3D, vẽ Gizmos và làm data provider để TileLevelEditorWindow lưu xuống SO.
    /// (Chạy trong môi trường thiết kế level)
    /// </summary>
    [ExecuteAlways]
    public class LevelEditorBoard : MonoBehaviour
    {
        public float CellSize = 1f;

        // Dictionary theo dõi các ô đang đặt trên grid: Key = (X, Y, Layer)
        private Dictionary<Vector3Int, bool> _placedTiles = new Dictionary<Vector3Int, bool>();

        public void ClearBoard()
        {
            _placedTiles.Clear();
        }

        public void LoadLayout(List<TileSpawnData> layout)
        {
            ClearBoard();
            if (layout == null) return;
            foreach (var tile in layout)
            {
                _placedTiles[new Vector3Int(tile.GridX, tile.GridY, tile.LayerIndex)] = true;
            }
        }

        public List<TileSpawnData> GetTilesData()
        {
            List<TileSpawnData> list = new List<TileSpawnData>();
            foreach (var kvp in _placedTiles)
            {
                if (kvp.Value)
                {
                    list.Add(new TileSpawnData(kvp.Key.x, kvp.Key.y, kvp.Key.z));
                }
            }
            return list;
        }

        public void ToggleTile(int x, int y, int layer)
        {
            Vector3Int key = new Vector3Int(x, y, layer);
            if (_placedTiles.ContainsKey(key))
            {
                _placedTiles.Remove(key); // Xóa nếu đã có
            }
            else
            {
                _placedTiles[key] = true; // Thêm nếu chưa có
            }
        }

        public bool HasTile(int x, int y, int layer)
        {
            return _placedTiles.ContainsKey(new Vector3Int(x, y, layer));
        }

        public Vector3 GetWorldPosition(int gridX, int gridY, int layer)
        {
            // Lớp lẻ (1, 3, 5) sẽ dịch chuyển 1/2 ô so với lớp chẵn (0, 2, 4)
            float offset = (layer % 2 != 0) ? CellSize * 0.5f : 0f;
            float worldX = transform.position.x + (gridX * CellSize) + offset;
            float worldY = transform.position.y + (gridY * CellSize) + offset;
            float worldZ = transform.position.z - layer; // Z âm để đè lên trên hiển thị (nếu 2D)

            return new Vector3(worldX, worldY, worldZ);
        }

        private void OnDrawGizmos()
        {
            // Vẽ lưới (wireframe) và các ô (solid/wire) ở chế độ Editor
            Gizmos.color = Color.white;

            foreach (var kvp in _placedTiles)
            {
                if (kvp.Value)
                {
                    Vector3Int pos = kvp.Key;
                    Vector3 worldPos = GetWorldPosition(pos.x, pos.y, pos.z);

                    // Gradient màu theo layer index để dễ phân biệt
                    Gizmos.color = Color.Lerp(Color.yellow, Color.cyan, pos.z / 5f);
                    Gizmos.DrawCube(worldPos, new Vector3(CellSize * 0.9f, CellSize * 0.9f, 0.1f));
                    
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(worldPos, new Vector3(CellSize, CellSize, 0.1f));
                }
            }
        }
    }
}
