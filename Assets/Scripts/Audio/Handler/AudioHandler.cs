using UnityEngine;
namespace TileAdventure
{
    public abstract class AudioHandler : MonoBehaviour
    {
        [SerializeField] protected AudioClipEventSO _musicChannel;
        protected AudioSource _source;

        protected virtual void Awake() {
            _source = GetComponent<AudioSource>();
        }

        protected virtual void OnEnable() {
            _musicChannel.Subscribe(PlaySound);
        }
        protected virtual void OnDisable() {
            _musicChannel.Unsubscribe(PlaySound);
        }

        protected abstract void PlaySound(AudioClipSO data);
    }
}