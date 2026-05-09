using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace TileAdventure
{
    public class BoardController : MonoBehaviour
    {
        [Header("Behavior")]
        [SerializeField] private BoardBehavior _boardBehavior;
        public BoardBehavior BoardBehavior => _boardBehavior;

        [Header("Config")]
        [SerializeField] private AssetReference tilePrefabRef;

        [SerializeField] private TileIconPaletteSO iconPaletteSO;

        private BoardData _boardData;
        private Dictionary<string, TileBehavior> _tileBehaviors = new Dictionary<string, TileBehavior>();

        public bool IsEmpty => _boardData != null && _boardData.IsEmpty;

        public async UniTask Initialize(LevelDataSO levelData)
        {
            List<TileData> allTiles = new List<TileData>();
            int index = 0;

            foreach (var spawnData in levelData.layoutConfigurations)
            {
                string id = $"tile_{index++}";
                TileData model = new TileData(
                    id, spawnData.IconId,
                    spawnData.GridX, spawnData.GridY,
                    spawnData.LayerIndex
                );
                allTiles.Add(model);
            }

            _boardData = new BoardData(allTiles);

            foreach (var tile in allTiles)
            {
                Sprite icon = (tile.IconId >= 0 && tile.IconId < iconPaletteSO.IconList.Count)
                    ? iconPaletteSO.IconList[tile.IconId]
                    : null;

                TileBehavior tileBehavior = await _boardBehavior.SpawnTile(tile, tilePrefabRef, icon);
                tileBehavior.gameObject.SetActive(false);
                _tileBehaviors.Add(tile.Id, tileBehavior);
            }

            _boardBehavior.ShowInitBoardAnim(_tileBehaviors.Values.ToList());
        }

        public void RemoveTile(TileData tile)
        {
            _boardData.RemoveTile(tile);
            _tileBehaviors.Remove(tile.Id);
        }
    }
}
