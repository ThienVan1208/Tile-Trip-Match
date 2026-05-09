using UnityEngine;
using Unity.Mathematics;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
namespace TileAdventure
{
    public class LevelChosenController : MonoBehaviour
    {
        [SerializeField] private LevelChosenPanel _levelChosenPanel;
        [SerializeField] private LevelDatabaseSO _levelDatabaseSO;
        [SerializeField] private PlayerLevelSO _playerLevelSO;

        [Header("Audio")]
        [SerializeField] private AudioClipSO _tapAudioClip;
        [SerializeField] private AudioClipEventSO _audioChannel;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            int expectedLevels = Constant.TOTAL_LEVEL_SLOTS;
            List<bool> unlockStatusList = new List<bool>();

            for (int i = 0; i < expectedLevels; i++)
            {
                bool isUnlocked = false;
                if (_playerLevelSO != null && _levelDatabaseSO != null)
                {
                    isUnlocked = (i <= _playerLevelSO.MaxUnlockedLevel) && (i < _levelDatabaseSO.LevelDatas.Length);
                }
                unlockStatusList.Add(isUnlocked);
            }

            _levelChosenPanel.OnLevelSelected += HandleLevelSelected;
            _levelChosenPanel.GenerateLevelSlots(unlockStatusList);
        }

        private void OnDestroy()
        {
            _levelChosenPanel.OnLevelSelected -= HandleLevelSelected;
        }

        private void HandleLevelSelected(int levelId)
        {
            Debug.Log($"Level selected: LevelId={levelId}");
            _levelDatabaseSO.SetLevelIndex(levelId);
            _audioChannel.RaiseEvent(_tapAudioClip);
            LoadSceneManager.LoadScene(Constant.GAMEPLAY_SCENE_NAME).Forget();
        }
    }
}