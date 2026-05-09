using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;

namespace TileAdventure
{
    public class BoardBehavior : MonoBehaviour
    {
        [SerializeField] private float cellSize = 1f;
        public float CellSize => cellSize;
        
        public async UniTask<TileBehavior> SpawnTile(TileData tileData, AssetReference tilePrefabRef, Sprite iconSprite)
        {
            float offset = (tileData.Layer % 2 != 0) ? cellSize * 0.5f : 0f;
            float physX = transform.position.x + (tileData.GridX * cellSize) + offset;
            float physY = transform.position.y + (tileData.GridY * cellSize) + offset;
            float physZ = transform.position.z - tileData.Layer;
            Vector3 spawnPos = new Vector3(physX, physY, physZ);

            GameObject go = await AssetManager.InstantiateAsync(tilePrefabRef, spawnPos, Quaternion.identity, this.transform);
            TileBehavior tileBehavior = go.GetComponent<TileBehavior>();
            
            tileBehavior.gameObject.name = $"Tile_{tileData.Layer}_{tileData.GridX}_{tileData.GridY}";
            tileBehavior.Init(tileData, iconSprite, tileData.Layer);

            return tileBehavior;
        }

        public async void ShowInitBoardAnim(List<TileBehavior> tileBehaviors)
        {
            foreach (var tileBehavior in tileBehaviors)
            {
                tileBehavior.gameObject.SetActive(true);
                tileBehavior.ShowSpawnAnim();
                await UniTask.Delay(1);
            }
        }
    }
}