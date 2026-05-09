

namespace TileAdventure
{
    public class MusicHandler : AudioHandler
    {
        protected override async void PlaySound(AudioClipSO data)
        {
            var clip = await data.GetAudioClipAsync();

            if (this == null || _source == null) return;

            _source.Stop();
            _source.clip = clip;
            _source.volume = data.volume;
            _source.loop = true; 
            _source.Play();
        }
    }
}