using UnityEngine.AddressableAssets;
using System;

namespace TileAdventure
{
    [Serializable]
    public class AssetReferenceLevelData : AssetReferenceT<LevelDataSO>
    {
        public AssetReferenceLevelData(string guid) : base(guid) { }
    }
}
