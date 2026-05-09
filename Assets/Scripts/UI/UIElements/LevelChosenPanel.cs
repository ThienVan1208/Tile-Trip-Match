using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TileAdventure
{
    public class LevelChosenPanel : MonoBehaviour
    {
        [SerializeField] private AssetReference _levelChosenSlotPrefab;
        [SerializeField] private RectTransform _content;
        private List<LevelChosenSlot> _levelChosenSlots = new List<LevelChosenSlot>();
        public List<LevelChosenSlot> LevelChosenSlots => _levelChosenSlots;
        private int _shownLevelChosenNumber;

        public Action<int> OnLevelSelected;


        public async void GenerateLevelSlots(List<bool> unlockStatusList)
        {
            _shownLevelChosenNumber = unlockStatusList.Count;
            for (int i = 0; i < _shownLevelChosenNumber; i++)
            {
                GameObject slotObj = await AssetManager.InstantiateAsync(_levelChosenSlotPrefab, parent: _content);
                slotObj.name = $"LevelChosenSlot_{i}";
                LevelChosenSlot levelChosenSlot = slotObj.GetComponent<LevelChosenSlot>();
                levelChosenSlot.Init(i, unlockStatusList[i], OnLevelSelected);
                _levelChosenSlots.Add(levelChosenSlot);
            }
        }
    }
}