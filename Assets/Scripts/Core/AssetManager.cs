using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace TileAdventure
{
    public static class AssetManager
    {
        public static async UniTask InitializeAsync()
        {
            await Addressables.InitializeAsync().ToUniTask();
        }
        public static async UniTask<T> LoadAssetAsync<T>(AssetReference reference) where T : Object
        {
            if (reference.RuntimeKeyIsValid() == false)
            {
                Debug.LogError("AssetReference invalid!");
                return null;
            }

            
            var handle = Addressables.LoadAssetAsync<T>(reference);
            return await handle.ToUniTask();
        }

        public static async UniTask<T> LoadAssetAsync<T>(string address) where T : Object
        {
            
            var handle = Addressables.LoadAssetAsync<T>(address);
            return await handle.ToUniTask();
        }

        public static async UniTask<GameObject> InstantiateAsync(AssetReference reference, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            if(position == default) position = Vector3.zero;
            if(rotation == default) rotation = Quaternion.identity;
            var handle = Addressables.InstantiateAsync(reference, position, rotation, parent);
            return await handle.ToUniTask();
        }

        public static void ReleaseInstance(GameObject instance)
        {
            Addressables.ReleaseInstance(instance);
        }

        public static void ReleaseAsset<T>(T asset) where T : Object
        {
            Addressables.Release(asset);
        }
    }
}