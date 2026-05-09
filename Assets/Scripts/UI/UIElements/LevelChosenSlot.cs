using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TileAdventure
{
    public class LevelChosenSlot : MonoBehaviour
    {
        private int _levelId;
        [SerializeField] private TextMeshProUGUI _levelText;
        private Button _button;
        public void Init(int levelId, bool isUnlocked, Action<int> onClickCallback)
        {
            UpdateUI(levelId);

            _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.interactable = isUnlocked;

                _button.onClick.RemoveAllListeners();

                if (isUnlocked)
                {
                    _button.onClick.AddListener(() => onClickCallback(_levelId));
                }

                Image bgImage = GetComponent<Image>();
                if (bgImage != null)
                {
                    bgImage.color = isUnlocked ? Color.white : Constant.LOCKED_COLOR;
                }
            }
        }

        public void UpdateUI(int levelId)
        {
            _levelId = levelId;
            if (_levelText != null)
            {
                _levelText.text = _levelId.ToString();
            }
        }


    }
}