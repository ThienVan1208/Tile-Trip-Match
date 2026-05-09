using UnityEngine;
using System;
using DG.Tweening;

namespace TileAdventure
{
    public class TileBehavior : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private SpriteRenderer baseRenderer;

        [Header("Event Channels")]
        [SerializeField] private TileClickedEventSO onTileClicked;

        private TileData _tileData;
        public TileData TileData => _tileData;

        public void Init(TileData tileData, Sprite iconSprite, int layerIndex)
        {
            _tileData = tileData;
            if (iconRenderer != null && iconSprite != null)
            {
                iconRenderer.sprite = iconSprite;
            }


            if (baseRenderer != null) baseRenderer.sortingOrder = layerIndex * 2;
            if (iconRenderer != null) iconRenderer.sortingOrder = layerIndex * 2 + 1;

            _tileData.OnCoveringCountChanged += UpdateVisualState;
            UpdateVisualState(_tileData.CoveringTilesCount);
        }
        public void ShowSpawnAnim()
        {
            Vector3 originalScale = transform.localScale;
            transform.DOScale(originalScale, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        }
        private void UpdateVisualState(int coveringCount)
        {
            bool isExposed = coveringCount == 0;
            Color targetColor = isExposed ? Color.white : Constant.LOCKED_COLOR;
            if (iconRenderer != null) iconRenderer.color = targetColor;
            if (baseRenderer != null) baseRenderer.color = targetColor;
        }

        public void Interact()
        {
            if (_tileData != null && _tileData.IsExposed)
            {
                onTileClicked.RaiseEvent(new TileClickedEventArgs(_tileData, this));
            }
        }

        private void OnDestroy()
        {
            if (_tileData != null)
            {
                _tileData.OnCoveringCountChanged -= UpdateVisualState;
            }
        }

        public void OnMove(Vector3 targetPosition, Action onComplete = null)
        {
            if (iconRenderer != null) iconRenderer.sortingOrder += 100;
            if (baseRenderer != null) baseRenderer.sortingOrder += 100;

            transform.DOMove(targetPosition, Constant.TILE_MOVE_SPEED)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}