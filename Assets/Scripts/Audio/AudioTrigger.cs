using UnityEngine;

namespace TileAdventure
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] private AudioClipSO _clipData;
        [SerializeField] private AudioClipEventSO _audioChannel;

        public bool isTriggeredOnStart = false;

        private void Start()
        {
            if (isTriggeredOnStart)
            {
                TriggerAudio();
            }
        }

        public void TriggerAudio()
        {
            if (_clipData != null)
            {
                _audioChannel.RaiseEvent(_clipData);
            }
        }
    }
}