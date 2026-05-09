using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TileAdventure
{

    public class GameplayController : MonoBehaviour
    {
        [Header("Sub-Controllers")]
        [SerializeField] private BoardController _board;
        [SerializeField] private RackController _rack;
        [SerializeField] private CameraController _cameraController;

        [Header("Config")]
        private LevelDataSO _currentLevelSO;
        [SerializeField] private LevelDatabaseSO _levelDatabaseSO;

        [Header("Event Channels")]
        [SerializeField] private TileClickedEventSO _onTileClickedSO;
        [SerializeField] private ScreenOrientationEventSO _onScreenOrientationChangedSO;
        [SerializeField] private VoidEventChannelSO _onGameSavedSO;

        [Header("Audio")]
        [SerializeField] private AudioClipSO _tapMusicClip;
        [SerializeField] private AudioClipEventSO _audioChannel;

        private async void Awake()
        {
            await AssetManager.InitializeAsync();
            _currentLevelSO = await _levelDatabaseSO.GetCurrentLevelDataAsync();
            if (_currentLevelSO != null)
            {
                await InitializeLevel(_currentLevelSO);
            }
            else
            {
                Debug.LogError("Failed to load level data.");
            }
        }

        private void OnEnable()
        {
            _onTileClickedSO.Subscribe(HandleTileClicked);
            _onScreenOrientationChangedSO.Subscribe(HandleScreenChanged);
            _rack.OnMatchFinished += HandleMatchFinished;
        }

        private void OnDisable()
        {
            _onTileClickedSO.Unsubscribe(HandleTileClicked);
            _onScreenOrientationChangedSO.Unsubscribe(HandleScreenChanged);
            _rack.OnMatchFinished -= HandleMatchFinished;
        }

        public async UniTask InitializeLevel(LevelDataSO levelData)
        {
            _rack.Initialize();
            await _board.Initialize(levelData);

            if (_cameraController != null)
            {
                _cameraController.AdjustCameraAndLayout(
                    levelData.layoutConfigurations,
                    _board.BoardBehavior,
                    _rack.RackBehavior
                );
            }
        }

        private void HandleTileClicked(TileClickedEventArgs args)
        {
            TileData tileData = args.TileData;
            TileBehavior tileBehavior = args.TileBehavior;

            if (_rack.IsAnimating) return;
            if (_rack.IsFull) return;

            _board.RemoveTile(tileData);
            _audioChannel.RaiseEvent(_tapMusicClip);
            _rack.AddTile(tileData, tileBehavior);
        }

        private void HandleScreenChanged(ScreenOrientationEventArgs args)
        {
            if (_currentLevelSO != null && _cameraController != null)
            {
                _cameraController.AdjustCameraAndLayout(
                    _currentLevelSO.layoutConfigurations,
                    _board.BoardBehavior,
                    _rack.RackBehavior,
                    animate: true
                );
            }
        }

        private void HandleMatchFinished()
        {
            if (_board.IsEmpty && _rack.IsEmpty)
            {
                Debug.Log("Game Win");
                _levelDatabaseSO.SetNextLevel();
                _onGameSavedSO.RaiseEvent();
                UIController.OpenUI<UIWin>().Forget();
            }
            else if (_rack.IsFull)
            {
                Debug.LogWarning("Game Over");
                _onGameSavedSO.RaiseEvent();
                UIController.OpenUI<UILose>().Forget();
            }
        }
    }
}