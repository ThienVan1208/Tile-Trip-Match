using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using DG.Tweening;

namespace TileAdventure
{
    public class RackBehavior : MonoBehaviour
    {
        public float slotSpacing = 1.1f;
        private int rackSize = 7;
        [SerializeField] private AssetReference rackSlotPrefabRef;

        private void Start()
        {
            Init();
        }

        public Vector3 GetSlotPosition(int currentSlotIndex)
        {
            Vector3 origin = transform.position;
            var result = origin + Vector3.right * (currentSlotIndex * slotSpacing);
            return result;
        }

        public void UpdateTilesPosition(List<TileData> tiles, Dictionary<string, TileBehavior> behaviors, Action onComplete = null)
        {
            int total = tiles.Count;
            if (total == 0)
            {
                onComplete?.Invoke();
                return;
            }

            int completed = 0;
            for (int i = 0; i < total; i++)
            {
                TileData tile = tiles[i];
                if (behaviors.TryGetValue(tile.Id, out TileBehavior tileBehavior))
                {
                    Vector3 targetPos = GetSlotPosition(i);
                    tileBehavior.OnMove(targetPos, () =>
                    {
                        completed++;
                        if (completed == total)
                        {
                            onComplete?.Invoke();
                        }
                    });
                }
            }
        }

        public void PlayMatchAnimation(List<TileData> matchedTiles, Dictionary<string, TileBehavior> behaviors, Action onComplete = null)
        {
            if (matchedTiles == null || matchedTiles.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            TileBehavior firstTileBehavior = behaviors[matchedTiles[0].Id];
            int tileCount = 0;
            
            if (matchedTiles.Count == 1)
            {
                firstTileBehavior.transform.DOShakeScale(0.5f, 0.75f).OnComplete(() => onComplete?.Invoke());
                return;
            }

            for (int i = 1; i < matchedTiles.Count; i++)
            {
                TileData tile = matchedTiles[i];
                if (behaviors.TryGetValue(tile.Id, out TileBehavior tileBehavior))
                {
                    Vector3 targetPos = firstTileBehavior.transform.position;
                    tileBehavior.OnMove(targetPos, () =>
                    {
                        tileBehavior.gameObject.SetActive(false);
                        tileCount++;
                        if (tileCount == matchedTiles.Count - 1)
                        {
                            firstTileBehavior.transform.DOShakeScale(duration: 0.5f, strength: 0.75f)
                            .OnComplete(() =>
                            {
                                onComplete?.Invoke();
                            });
                        }
                    });
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            for (int i = 0; i < rackSize; i++)
            {
                Gizmos.DrawWireCube(GetSlotPosition(i), new Vector3(1f, 1f, 0.1f));
            }
        }

        private async void Init()
        {
            if (rackSlotPrefabRef == null || !rackSlotPrefabRef.RuntimeKeyIsValid())
            {
                Debug.LogError("Rack Slot Prefab Reference is not assigned or invalid.");
                return;
            }
            for (int i = 0; i < rackSize; i++)
            {
                GameObject slot = await AssetManager.InstantiateAsync(rackSlotPrefabRef, GetSlotPosition(i), Quaternion.identity, transform);
                slot.name = $"RackSlot_{i}";
                InitSlotAnim(slot);
            }
        }

        private void InitSlotAnim(GameObject slot)
        {
            Vector3 originalScale = slot.transform.localScale;
            slot.transform.localScale = Vector3.zero;
            slot.transform.DOScale(originalScale, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        }
    }
}