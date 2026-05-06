using UnityEditor;
using UnityEngine;
using TileTrip.Tiles.Data;
using TileTrip.EditorTools;

namespace TileTrip.Editor
{
    public class TileLevelEditorWindow : EditorWindow
    {
        private LevelDataSO _currentLevelData;
        private LevelEditorBoard _board;
        private int _currentLayer = 0;
        private int _gridWidth = 10;
        private int _gridHeight = 10;

        [MenuItem("Tile Trip/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<TileLevelEditorWindow>("Level Editor");
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            FindBoardInScene();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void FindBoardInScene()
        {
            _board = Object.FindFirstObjectByType<LevelEditorBoard>();
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Editor Settings", EditorStyles.boldLabel);

            if (_board == null)
            {
                EditorGUILayout.HelpBox("Không tìm thấy LevelEditorBoard trong Scene.\nHãy tạo một GameObject rỗng và gắn script LevelEditorBoard vào.", MessageType.Error);
                if (GUILayout.Button("Create LevelEditorBoard"))
                {
                    GameObject go = new GameObject("LevelEditorBoard");
                    _board = go.AddComponent<LevelEditorBoard>();
                    Selection.activeGameObject = go;
                }
                return;
            }

            EditorGUI.BeginChangeCheck();
            _currentLevelData = (LevelDataSO)EditorGUILayout.ObjectField("Level Data SO", _currentLevelData, typeof(LevelDataSO), false);
            if (EditorGUI.EndChangeCheck() && _currentLevelData != null)
            {
                _board.LoadLayout(_currentLevelData.layoutConfigurations);
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();
            _currentLayer = EditorGUILayout.IntSlider("Current Layer", _currentLayer, 0, 10);
            
            _gridWidth = EditorGUILayout.IntField("Grid Max Width", _gridWidth);
            _gridHeight = EditorGUILayout.IntField("Grid Max Height", _gridHeight);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create New Level SO"))
            {
                CreateNewLevelData();
            }

            if (_currentLevelData != null)
            {
                if (GUILayout.Button("Save Level to SO", GUILayout.Height(30)))
                {
                    SaveLevelData();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Hãy gán hoặc tạo Level Data SO để Lưu.", MessageType.Warning);
            }
        }

        private void CreateNewLevelData()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create New Level", "Level_", "asset", "Save level file");
            if (string.IsNullOrEmpty(path)) return;

            LevelDataSO newData = ScriptableObject.CreateInstance<LevelDataSO>();
            AssetDatabase.CreateAsset(newData, path);
            AssetDatabase.SaveAssets();
            
            _currentLevelData = newData;
            if (_board != null)
            {
                _board.ClearBoard();
            }
            Repaint();
        }

        private void SaveLevelData()
        {
            if (_currentLevelData == null || _board == null) return;

            _currentLevelData.layoutConfigurations = _board.GetTilesData();
            EditorUtility.SetDirty(_currentLevelData);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Level Editor] Đã lưu {_currentLevelData.layoutConfigurations.Count} ô vào {_currentLevelData.name}");
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (_board == null) return;

            // Xử lý Input
            Event e = Event.current;
            
            // Hiển thị lưới mờ ảo tại _currentLayer để dễ thao tác
            DrawVirtualGrid();

            // Mouse click đặt / xoá
            if (e.type == EventType.MouseDown && e.button == 0 && e.shift) // Shift + Left Click để thao tác
            {
                Plane layerPlane = new Plane(Vector3.forward, new Vector3(0, 0, -_currentLayer));
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                
                if (layerPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    
                    // Tính ngược lại logic grid
                    float offset = (_currentLayer % 2 != 0) ? _board.CellSize * 0.5f : 0f;
                    int gridX = Mathf.RoundToInt((hitPoint.x - _board.transform.position.x - offset) / _board.CellSize);
                    int gridY = Mathf.RoundToInt((hitPoint.y - _board.transform.position.y - offset) / _board.CellSize);

                    if (gridX >= 0 && gridX < _gridWidth && gridY >= 0 && gridY < _gridHeight)
                    {
                        Undo.RecordObject(_board, "Toggle Tile");
                        _board.ToggleTile(gridX, gridY, _currentLayer);
                        e.Use(); // Tiêu thụ event
                        sceneView.Repaint(); // Cập nhật hình ảnh Scene
                    }
                }
            }
        }

        private void DrawVirtualGrid()
        {
            Handles.color = new Color(1f, 1f, 1f, 0.2f);
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    Vector3 wPos = _board.GetWorldPosition(x, y, _currentLayer);
                    Handles.DrawWireCube(wPos, new Vector3(_board.CellSize, _board.CellSize, 0));
                }
            }

            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10, 10, 200, 45));
            GUILayout.Label($"Editing Layer: {_currentLayer}\nShift + Click to toggle.", EditorStyles.helpBox);
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }
}
