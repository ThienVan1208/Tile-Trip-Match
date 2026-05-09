using Cysharp.Threading.Tasks;
using UnityEngine;
namespace TileAdventure
{
    [CreateAssetMenu(fileName = "LevelDatabaseSO", menuName = "LevelData/LevelDatabaseSO")]
    public class LevelDatabaseSO : ScriptableObject
    {
        private int _currentLevelIndex = 0;
        [SerializeField] private PlayerLevelSO _playerLevelSO;
        [SerializeField] private AssetReferenceLevelData[] _levelDatas;
        public AssetReferenceLevelData[] LevelDatas => _levelDatas;

        public async UniTask<LevelDataSO> GetCurrentLevelDataAsync()
        {
            if (_currentLevelIndex >= 0 && _currentLevelIndex < _levelDatas.Length)
            {
                var assetRef = _levelDatas[_currentLevelIndex];
                return await AssetManager.LoadAssetAsync<LevelDataSO>(assetRef);
            }
            else
            {
                Debug.LogWarning("Level index out of bounds.");
                return null;
            }
        }

        public void SetNextLevel()
        {
            if (_currentLevelIndex < _levelDatas.Length - 1)
            {
                _currentLevelIndex++;
                if (_currentLevelIndex > _playerLevelSO.MaxUnlockedLevel)
                {
                    _playerLevelSO.UpdateLevel(_currentLevelIndex);
                }
            }
            else
            {
                Debug.LogWarning("Already at the last level.");
            }
        }

        public void ResetLevelProgress()
        {
            _currentLevelIndex = 0;
            _playerLevelSO.UpdateLevel(_currentLevelIndex);
        }

        public void SetLevelIndex(int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < _levelDatas.Length)
            {
                _currentLevelIndex = levelIndex;
                _playerLevelSO.UpdateLevel(_currentLevelIndex);
            }
            else
            {
                Debug.LogWarning($"Level index {levelIndex} is out of bounds. Level index remains unchanged.");
            }
        }

#if UNITY_EDITOR
        public void CleanUpNulls()
        {
            if (_levelDatas == null) return;
            var list = new System.Collections.Generic.List<AssetReferenceLevelData>(_levelDatas);
            bool removed = list.RemoveAll(item => item == null || !item.RuntimeKeyIsValid()) > 0;
            if (removed)
            {
                _levelDatas = list.ToArray();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        public void AddLevel(LevelDataSO newLevel)
        {
            CleanUpNulls();
            if (_levelDatas == null)
            {
                _levelDatas = new AssetReferenceLevelData[0];
            }
            
            string path = UnityEditor.AssetDatabase.GetAssetPath(newLevel);
            string guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            
            var newRef = new AssetReferenceLevelData(guid);
            
            UnityEditor.ArrayUtility.Add(ref _levelDatas, newRef);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}