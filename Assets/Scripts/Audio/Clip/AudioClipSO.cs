using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

public abstract class AudioClipSO : ScriptableObject
{
    [Range(0f, 1f)] public float volume = 1f;

    public abstract UniTask<AudioClip> GetAudioClipAsync();
}


