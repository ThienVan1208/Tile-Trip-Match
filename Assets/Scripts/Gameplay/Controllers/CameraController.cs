using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace TileAdventure
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        public float rackSpacingY = 2.0f;
        public float padding = 1.15f;

        private void Awake()
        {
            if (mainCamera == null) mainCamera = Camera.main;
        }

        public void AdjustCameraAndLayout(List<TileSpawnInfor> layout, BoardBehavior board, RackBehavior rack, bool animate = false)
        {
            if (layout == null || layout.Count == 0 || mainCamera == null) return;

            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            float bX = board.transform.position.x;
            float bY = board.transform.position.y;

            foreach (var tile in layout)
            {
                float offset = (tile.LayerIndex % 2 != 0) ? board.CellSize * 0.5f : 0f;
                float wX = bX + tile.GridX * board.CellSize + offset;
                float wY = bY + tile.GridY * board.CellSize + offset;

                if (wX < minX) minX = wX;
                if (wX > maxX) maxX = wX;
                if (wY < minY) minY = wY;
                if (wY > maxY) maxY = wY;
            }

            float boardCenterX = (minX + maxX) / 2f;
            float bottomOfBoard = minY - (board.CellSize * 0.5f);

            Vector3 targetRackPos = rack != null ? rack.transform.position : Vector3.zero;
            if (rack != null)
            {
                float rackStartX = boardCenterX - (3 * rack.slotSpacing);
                float rackTargetY = bottomOfBoard - rackSpacingY;
                targetRackPos = new Vector3(rackStartX, rackTargetY, rack.transform.position.z);
            }

            float rackMinY = rack != null ? targetRackPos.y - (board.CellSize / 2f) : minY;
            float totalHeight = (maxY - rackMinY) + board.CellSize; 
            float rackTotalWidth = rack != null ? (6 * rack.slotSpacing + board.CellSize) : 0f;
            float boardTotalWidth = (maxX - minX) + board.CellSize;
            float totalWidth = Mathf.Max(boardTotalWidth, rackTotalWidth);
            
            float centerY = (maxY + rackMinY) / 2f;
            Vector3 targetCameraPos = new Vector3(boardCenterX, centerY, mainCamera.transform.position.z);
            
            float targetOrthoSize;
            float screenAspect = (float)Screen.width / Screen.height;
            if (screenAspect >= 1f) 
            {
                targetOrthoSize = (totalHeight / 2f) * padding;
            }
            else 
            {
                targetOrthoSize = (totalWidth / 2f / screenAspect) * padding;
            }

            if (rack != null)
            {
                float bottomRackAnchor = padding;
                float minScreenY = targetCameraPos.y - targetOrthoSize;
                if(targetRackPos.y - minScreenY > rackSpacingY)
                {
                    float newRackY = minScreenY + bottomRackAnchor * padding;
                    targetRackPos = new Vector3(targetRackPos.x, newRackY, targetRackPos.z);
                }
            }

            ApplyLayout(targetCameraPos, targetOrthoSize, targetRackPos, rack, animate);
        }

        private void ApplyLayout(Vector3 targetCameraPos, float targetOrthoSize, Vector3 targetRackPos, RackBehavior rack, bool animate)
        {
            mainCamera.DOKill();
            if (rack != null) rack.transform.DOKill();

            if (animate)
            {
                float duration = 0.5f;
                Ease ease = Ease.OutCubic;

                mainCamera.transform.DOMove(targetCameraPos, duration).SetEase(ease);
                mainCamera.DOOrthoSize(targetOrthoSize, duration).SetEase(ease);

                if (rack != null)
                {
                    rack.transform.DOMove(targetRackPos, duration).SetEase(ease);
                }
            }
            else
            {
                mainCamera.transform.position = targetCameraPos;
                mainCamera.orthographicSize = targetOrthoSize;
                if (rack != null)
                {
                    rack.transform.position = targetRackPos;
                }
            }
        }
    }
}