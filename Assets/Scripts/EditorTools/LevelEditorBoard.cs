using System.Collections.Generic;
using UnityEngine;
namespace TileAdventure
{
    [ExecuteAlways]
    public class LevelEditorBoard : MonoBehaviour
    {
        public Sprite ContainerSprite;
        public float CellSize = 1f;
        private Dictionary<Vector3Int, int> _placedTilesData = new Dictionary<Vector3Int, int>();
        private Dictionary<Vector3Int, GameObject> _previewObjects = new Dictionary<Vector3Int, GameObject>();
        public void ClearBoard()
        {
            _placedTilesData.Clear();
            foreach (var go in _previewObjects.Values)
            {
                if (go != null) DestroyImmediate(go);
            }
            _previewObjects.Clear();
        }
        public void LoadLayout(List<TileSpawnInfor> layout, List<Sprite> iconPalette)
        {
            ClearBoard();
            if (layout == null) return;
            foreach (var tile in layout)
            {
                Sprite previewSprite = null;
                if (iconPalette != null && tile.IconId >= 0 && tile.IconId < iconPalette.Count)
                    previewSprite = iconPalette[tile.IconId];
                ToggleTile(tile.GridX, tile.GridY, tile.LayerIndex, tile.IconId, previewSprite, true);
            }
            UpdateAllTileColors();
        }
        public List<TileSpawnInfor> GetTilesData()
        {
            List<TileSpawnInfor> list = new List<TileSpawnInfor>();
            foreach (var kvp in _placedTilesData)
            {
                list.Add(new TileSpawnInfor(kvp.Key.x, kvp.Key.y, kvp.Key.z, kvp.Value));
            }
            return list;
        }
        public void ToggleTile(int x, int y, int layer, int iconId, Sprite sprite, bool isForceAdd = false)
        {
            Vector3Int key = new Vector3Int(x, y, layer);
            if (!isForceAdd && _placedTilesData.ContainsKey(key))
            {
                _placedTilesData.Remove(key);
                if (_previewObjects.ContainsKey(key))
                {
                    DestroyImmediate(_previewObjects[key]);
                    _previewObjects.Remove(key);
                }
            }
            else
            {
                _placedTilesData[key] = iconId;
                if (!_previewObjects.ContainsKey(key))
                {
                    GameObject go = new GameObject($"Tile_Preview_{x}_{y}_{layer}");
                    go.transform.SetParent(this.transform);
                    go.hideFlags = HideFlags.HideAndDontSave; 

                    GameObject containerGo = new GameObject("Container");
                    containerGo.transform.SetParent(go.transform, false);
                    SpriteRenderer containerSr = containerGo.AddComponent<SpriteRenderer>();
                    containerSr.sprite = ContainerSprite;
                    containerSr.sortingOrder = layer * 2;

                    GameObject iconGo = new GameObject("Icon");
                    iconGo.transform.SetParent(go.transform, false);
                    SpriteRenderer iconSr = iconGo.AddComponent<SpriteRenderer>();
                    iconSr.sprite = sprite;
                    iconSr.sortingOrder = layer * 2 + 1;

                    go.transform.position = GetWorldPosition(x, y, layer);
                    _previewObjects[key] = go;
                }
                else
                {
                    Transform iconTrans = _previewObjects[key].transform.Find("Icon");
                    if (iconTrans != null)
                    {
                        iconTrans.GetComponent<SpriteRenderer>().sprite = sprite;
                    }
                }
            }

            if (!isForceAdd)
            {
                UpdateAllTileColors();
            }
        }
        public Vector3 GetWorldPosition(int gridX, int gridY, int layer)
        {
            float offset = (layer % 2 != 0) ? CellSize * 0.5f : 0f;
            return transform.position + new Vector3(gridX * CellSize + offset, gridY * CellSize + offset, -layer);
        }
        private void OnDisable()
        {
            foreach (var go in _previewObjects.Values)
            {
                if (go != null) DestroyImmediate(go);
            }
            _previewObjects.Clear();
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            foreach (var kvp in _placedTilesData)
            {
                Vector3 wPos = GetWorldPosition(kvp.Key.x, kvp.Key.y, kvp.Key.z);
                Gizmos.DrawWireCube(wPos, new Vector3(CellSize, CellSize, 0.1f));
            }
        }

        public void UpdateAllTileColors()
        {
            foreach (var go in _previewObjects.Values)
            {
                var renderers = go.GetComponentsInChildren<SpriteRenderer>();
                foreach (var r in renderers) r.color = Color.white;
            }

            HashSet<Vector3Int> coveredTiles = new HashSet<Vector3Int>();
            foreach (var kvp in _placedTilesData)
            {
                Vector3Int pos = kvp.Key;
                List<Vector3Int> underlying = GetUnderlyingPositions(pos.x, pos.y, pos.z);
                foreach (var u in underlying)
                {
                    coveredTiles.Add(u);
                }
            }

            foreach (var coveredPos in coveredTiles)
            {
                if (_previewObjects.TryGetValue(coveredPos, out GameObject go))
                {
                    var renderers = go.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var r in renderers) r.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                }
            }
        }

        private List<Vector3Int> GetUnderlyingPositions(int x, int y, int layer)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            if (layer <= 0) return result;
            int lowerLayer = layer - 1;
            bool isCurrentOdd = (layer % 2 != 0);
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
