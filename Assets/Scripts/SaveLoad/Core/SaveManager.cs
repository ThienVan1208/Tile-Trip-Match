using System.Collections.Generic;
using TileAdventure;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private List<ScriptableObject> _saveableScriptableObjects;
    [SerializeField] private VoidEventChannelSO _onGameSavedSO;
    private IStorageService _storageService;
    private GameData _currentGameData;
    private List<ISaveableSO> _saveables = new List<ISaveableSO>();
    private const string SAVE_KEY = "Tile_GameSave";

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); 
        _storageService = new JsonStorageService();
        
        foreach (var so in _saveableScriptableObjects)
        {
            if (so is ISaveableSO saveable)
            {
                _saveables.Add(saveable);
            }
            else
            {
                Debug.LogWarning($"ScriptableObject {so.name} does not implement ISaveableSO!");
            }
        }
        
        LoadGame();
    }
    private void OnEnable() {
        _onGameSavedSO.Subscribe(SaveGame);
    }
    private void OnDisable()
    {
        _onGameSavedSO.Unsubscribe(SaveGame);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }

    public void SaveGame()
    {
        if (_currentGameData == null) _currentGameData = new GameData();

        foreach (var saveable in _saveables)
        {
            saveable.PopulateSaveData(_currentGameData);
        }

        _storageService.Save(SAVE_KEY, _currentGameData);
    }

    public void LoadGame()
    {
        _currentGameData = _storageService.Load<GameData>(SAVE_KEY) ?? new GameData();

        foreach (var saveable in _saveables)
        {
            saveable.LoadFromSaveData(_currentGameData);
        }
    }
}