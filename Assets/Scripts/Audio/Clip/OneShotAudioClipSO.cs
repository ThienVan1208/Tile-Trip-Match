using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using TileAdventure;
[CreateAssetMenu(fileName = "OneShotAudioClipSO", menuName = "AudioClipSO/OneShot")]
public class OneShotAudioClipSO : AudioClipSO
{
    [SerializeField] private AssetReferenceT<AudioClip> _audioClipRef;
    private AudioClip _cachedClip;
    private UniTask<AudioClip>? _loadingTask;

    public override async UniTask<AudioClip> GetAudioClipAsync()
    {
        if (_cachedClip != null) return _cachedClip;

        if (_loadingTask.HasValue) return await _loadingTask.Value;

        _loadingTask = AssetManager.LoadAssetAsync<AudioClip>(_audioClipRef);
        _cachedClip = await _loadingTask.Value;
        
        return _cachedClip;
    }
}