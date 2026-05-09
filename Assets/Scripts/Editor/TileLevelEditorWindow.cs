using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace TileAdventure
{
    public class TileLevelEditorWindow : EditorWindow
    {
        private LevelDataSO _currentLevelData;
        private LevelDatabaseSO _levelDatabase;
        private LevelEditorBoard _board;

        private int _currentLayer = 0;
        private int _gridWidth = Constant.DEFAULT_GRID_SIZE;
        private int _gridHeight = Constant.DEFAULT_GRID_SIZE;
        private float _cellSize = Constant.EDITOR_CELL_SIZE;
        private Vector2 _scrollPosition;
        
        private TileIconPaletteSO _iconPaletteSO;
        private int _selectedIconIndex = 0;
        private Sprite _containerSprite;

        [MenuItem("Tile Adventure/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<TileLevelEditorWindow>("Level Editor");
        }

        private void OnEnable()
        {
            AutoFindReferences();
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void AutoFindReferences()
        {
            _board = Object.FindFirstObjectByType<LevelEditorBoard>();
            
            string[] dbGuids = AssetDatabase.FindAssets("t:LevelDatabaseSO");
            if (dbGuids.Length > 0)
            {
                _levelDatabase = AssetDatabase.LoadAssetAtPath<LevelDatabaseSO>(AssetDatabase.GUIDToAssetPath(dbGuids[0]));
            }

            string[] paletteGuids = AssetDatabase.FindAssets("t:TileIconPaletteSO");
            if (paletteGuids.Length > 0)
            {
                _iconPaletteSO = AssetDatabase.LoadAssetAtPath<TileIconPaletteSO>(AssetDatabase.GUIDToAssetPath(paletteGuids[0]));
            }

            if (_board != null && _board.ContainerSprite != null)
            {
                _containerSprite = _board.ContainerSprite;
            }
        }

        private void OnUndoRedo()
        {
            if (_currentLevelData != null && _board != null)
            {
                var iconList = _iconPaletteSO != null ? _iconPaletteSO.IconList : null;
                _board.LoadLayout(_currentLevelData.layoutConfigurations, iconList);
                SceneView.RepaintAll();
            }
            Repaint();
        }

        private void OnGUI()
        {
            DrawTopPanel();
            
            if (_currentLevelData == null)
            {
                EditorGUILayout.HelpBox("Assign a LevelDataSO to start.", MessageType.Info);
                return;
            }

            DrawPalettePanel();
            DrawCanvasPanel();
        }

        private void DrawTopPanel()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("Configuration", EditorStyles.boldLabel);
            
            _levelDatabase = (LevelDatabaseSO)EditorGUILayout.ObjectField("Database", _levelDatabase, typeof(LevelDatabaseSO), false);
            
            EditorGUI.BeginChangeCheck();
            _currentLevelData = (LevelDataSO)EditorGUILayout.ObjectField("Level", _currentLevelData, typeof(LevelDataSO), false);
            if (EditorGUI.EndChangeCheck() && _currentLevelData != null)
            {
                RefreshScenePreview();
            }

            _containerSprite = (Sprite)EditorGUILayout.ObjectField("Container", _containerSprite, typeof(Sprite), false);
            if (_board != null && _board.ContainerSprite != _containerSprite)
            {
                _board.ContainerSprite = _containerSprite;
                RefreshScenePreview();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create New Level", GUILayout.Height(30)))
            {
                CreateNewLevelData();
            }
            if (GUILayout.Button("Clear Data", GUILayout.Height(30)))
            {
                ClearLevelData();
            }
            if (GUILayout.Button("Auto Color", GUILayout.Height(30)))
            {
                AutoColorSolvable();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawPalettePanel()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("Icon Palette", EditorStyles.boldLabel);
            
            _currentLayer = EditorGUILayout.IntSlider("Layer", _currentLayer, 0, Constant.MAX_LAYERS);
            
            if (_iconPaletteSO != null && _iconPaletteSO.IconList.Count > 0)
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < _iconPaletteSO.IconList.Count; i++)
                {
                    GUI.backgroundColor = (_selectedIconIndex == i) ? Color.green : Color.white;
                    if (GUILayout.Button(_iconPaletteSO.IconList[i].texture, GUILayout.Width(Constant.PALETTE_BUTTON_SIZE), GUILayout.Height(Constant.PALETTE_BUTTON_SIZE)))
                    {
                        _selectedIconIndex = i;
                    }
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void DrawCanvasPanel()
        {
            GUILayout.Label("2D Canvas", EditorStyles.boldLabel);
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
            float canvasWidth = (_gridWidth + 1) * _cellSize;
            float canvasHeight = (_gridHeight + 1) * _cellSize;
            Rect canvasRect = GUILayoutUtility.GetRect(canvasWidth, canvasHeight);
            
            DrawBackgroundGrid(canvasRect);
            DrawTilesOnCanvas(canvasRect);
            HandleCanvasInput(canvasRect);

            GUILayout.EndScrollView();
        }

        private void DrawBackgroundGrid(Rect canvasRect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUI.color = new Color(1, 1, 1, 0.1f);
                for (int x = 0; x <= _gridWidth; x++)
                {
                    GUI.DrawTexture(new Rect(canvasRect.x + x * _cellSize, canvasRect.y, 1, _gridHeight * _cellSize), EditorGUIUtility.whiteTexture);
                }
                for (int y = 0; y <= _gridHeight; y++)
                {
                    GUI.DrawTexture(new Rect(canvasRect.x, canvasRect.y + y * _cellSize, _gridWidth * _cellSize, 1), EditorGUIUtility.whiteTexture);
                }
                GUI.color = Color.white;
            }
        }

        private void DrawTilesOnCanvas(Rect canvasRect)
        {
            if (Event.current.type != EventType.Repaint) return;
            if (_currentLevelData == null || _currentLevelData.layoutConfigurations == null) return;

            HashSet<Vector3Int> coveredTiles = CalculateCoveredTiles();
            var sortedTiles = _currentLevelData.layoutConfigurations.OrderBy(t => t.LayerIndex).ThenBy(t => t.GridY).ToList();

            foreach (var tile in sortedTiles)
            {
                float offset = (tile.LayerIndex % 2 != 0) ? _cellSize * 0.5f : 0f;
                Rect tileRect = new Rect(
                    canvasRect.x + tile.GridX * _cellSize + offset, 
                    canvasRect.y + tile.GridY * _cellSize + offset, 
                    _cellSize, _cellSize);

                bool isCovered = coveredTiles.Contains(new Vector3Int(tile.GridX, tile.GridY, tile.LayerIndex));
                
                Color drawColor = Color.white;
                if (isCovered || tile.LayerIndex < _currentLayer - 1)
                {
                    drawColor = Constant.LOCKED_COLOR;
                }
                else if (tile.LayerIndex > _currentLayer)
                {
                    drawColor = Constant.DIMMED_COLOR;
                }

                GUI.color = drawColor;

                if (_containerSprite != null)
                {
                    GUI.DrawTexture(tileRect, _containerSprite.texture, ScaleMode.ScaleToFit);
                }
                else
                {
                    GUI.DrawTexture(tileRect, EditorGUIUtility.whiteTexture);
                }

                if (_iconPaletteSO != null && tile.IconId >= 0 && tile.IconId < _iconPaletteSO.IconList.Count)
                {
                    GUI.DrawTexture(tileRect, _iconPaletteSO.IconList[tile.IconId].texture, ScaleMode.ScaleToFit);
                }

                GUI.color = Color.white;
            }
        }

        private void HandleCanvasInput(Rect canvasRect)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && canvasRect.Contains(e.mousePosition))
            {
                Vector2 localPos = e.mousePosition - canvasRect.position;
                float offset = (_currentLayer % 2 != 0) ? _cellSize * 0.5f : 0f;
                
                int gridX = Mathf.FloorToInt((localPos.x - offset) / _cellSize);
                int gridY = Mathf.FloorToInt((localPos.y - offset) / _cellSize);
                
                if (gridX >= 0 && gridX < _gridWidth && gridY >= 0 && gridY < _gridHeight)
                {
                    ToggleTileData(gridX, gridY, _currentLayer, _selectedIconIndex);
                    e.Use();
                }
            }
        }

        private void ToggleTileData(int x, int y, int layer, int iconId)
        {
            Undo.RecordObject(_currentLevelData, "Toggle Tile");

            var layout = _currentLevelData.layoutConfigurations;
            int existingIndex = layout.FindIndex(t => t.GridX == x && t.GridY == y && t.LayerIndex == layer);
            
            if (existingIndex >= 0)
            {
                layout.RemoveAt(existingIndex);
            }
            else
            {
                layout.Add(new TileSpawnInfor(x, y, layer, iconId));
            }

            EditorUtility.SetDirty(_currentLevelData);
            RefreshScenePreview();
        }

        private void RefreshScenePreview()
        {
            if (_board == null) _board = Object.FindFirstObjectByType<LevelEditorBoard>();
            if (_board != null && _currentLevelData != null)
            {
                var iconList = _iconPaletteSO != null ? _iconPaletteSO.IconList : null;
                _board.LoadLayout(_currentLevelData.layoutConfigurations, iconList);
                SceneView.RepaintAll();
            }
        }

        private HashSet<Vector3Int> CalculateCoveredTiles()
        {
            HashSet<Vector3Int> covered = new HashSet<Vector3Int>();
            if (_currentLevelData == null || _currentLevelData.layoutConfigurations == null) return covered;

            foreach (var tile in _currentLevelData.layoutConfigurations)
            {
                List<Vector3Int> underlying = GetUnderlyingPositions(tile.GridX, tile.GridY, tile.LayerIndex);
                foreach (var u in underlying) covered.Add(u);
            }
            return covered;
        }

        private void CreateNewLevelData()
        {
            int nextIndex = 0;
            if (_levelDatabase != null && _levelDatabase.LevelDatas != null)
            {
                _levelDatabase.CleanUpNulls();
                nextIndex = _levelDatabase.LevelDatas.Length + 1;
            }

            string defaultName = $"Level_{nextIndex}";
            
            // Ensure the directory exists
            string folderPath = "Assets/SO/LevelData/LevelDatas";
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            string path = EditorUtility.SaveFilePanelInProject("Create Level", defaultName, "asset", "Save file", folderPath);
            if (string.IsNullOrEmpty(path)) return;

            LevelDataSO newData = ScriptableObject.CreateInstance<LevelDataSO>();
            AssetDatabase.CreateAsset(newData, path);
            AssetDatabase.SaveAssets();

            _currentLevelData = newData;
            if (_levelDatabase != null) _levelDatabase.AddLevel(newData);

            RefreshScenePreview();
            Repaint();
        }

        private void ClearLevelData()
        {
            if (_currentLevelData == null || _currentLevelData.layoutConfigurations == null) return;
            
            if (EditorUtility.DisplayDialog("Clear Data", "Are you sure you want to clear all tiles in this level?", "Yes", "No"))
            {
                Undo.RecordObject(_currentLevelData, "Clear Level Data");
                _currentLevelData.layoutConfigurations.Clear();
                EditorUtility.SetDirty(_currentLevelData);
                RefreshScenePreview();
                Repaint();
            }
        }

        private void AutoColorSolvable()
        {
            if (_currentLevelData == null || _currentLevelData.layoutConfigurations.Count == 0) return;
            if (_iconPaletteSO == null || _iconPaletteSO.IconList.Count == 0) return;

            var tiles = _currentLevelData.layoutConfigurations;

            if (tiles.Count % Constant.TILE_MATCH_NUMBER != 0)
            {
                EditorUtility.DisplayDialog("Error", "Tiles count must be divisible by " + Constant.TILE_MATCH_NUMBER, "OK");
                return;
            }

            Undo.RecordObject(_currentLevelData, "Auto Color");

            Dictionary<Vector3Int, int> tileIndexMap = new Dictionary<Vector3Int, int>();
            for (int i = 0; i < tiles.Count; i++)
            {
                tileIndexMap[new Vector3Int(tiles[i].GridX, tiles[i].GridY, tiles[i].LayerIndex)] = i;
            }

            Dictionary<Vector3Int, int> coveringCounts = new Dictionary<Vector3Int, int>();
            foreach (var key in tileIndexMap.Keys) coveringCounts[key] = 0;

            foreach (var tile in tiles)
            {
                List<Vector3Int> underlying = GetUnderlyingPositions(tile.GridX, tile.GridY, tile.LayerIndex);
                foreach (var u in underlying)
                {
                    if (coveringCounts.ContainsKey(u)) coveringCounts[u]++;
                }
            }

            List<Vector3Int> remainingKeys = new List<Vector3Int>(tileIndexMap.Keys);

            while (remainingKeys.Count > 0)
            {
                List<Vector3Int> exposedKeys = new List<Vector3Int>();
                foreach (var key in remainingKeys)
                {
                    if (coveringCounts[key] == 0) exposedKeys.Add(key);
                }

                if (exposedKeys.Count < Constant.TILE_MATCH_NUMBER)
                {
                    EditorUtility.DisplayDialog("Error", "Stuck! Layout is too narrow at the top.", "OK");
                    return;
                }

                List<Vector3Int> picked = new List<Vector3Int>();
                for (int i = 0; i < Constant.TILE_MATCH_NUMBER; i++)
                {
                    int randIdx = Random.Range(0, exposedKeys.Count);
                    picked.Add(exposedKeys[randIdx]);
                    exposedKeys.RemoveAt(randIdx);
                }

                int randomIconId = Random.Range(0, _iconPaletteSO.IconList.Count);

                foreach (var p in picked)
                {
                    int index = tileIndexMap[p];
                    var t = tiles[index];
                    t.IconId = randomIconId;
                    tiles[index] = t;

                    remainingKeys.Remove(p);

                    List<Vector3Int> underlying = GetUnderlyingPositions(p.x, p.y, p.z);
                    foreach (var u in underlying)
                    {
                        if (coveringCounts.ContainsKey(u)) coveringCounts[u]--;
                    }
                }
            }

            EditorUtility.SetDirty(_currentLevelData);
            RefreshScenePreview();
            Repaint();
        }

        private List<Vector3Int> GetUnderlyingPositions(int x, int y, int layer)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            if (layer <= 0) return result;
            int lowerLayer = layer - 1;
            bool isCurrentOdd = layer % 2 != 0;
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
