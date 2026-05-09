namespace TileAdventure
{
    public class SfxHandler : AudioHandler
    {
        protected override async void PlaySound(AudioClipSO data)
        {
            var clip = await data.GetAudioClipAsync();

            if (this == null || _source == null) return;

            _source.PlayOneShot(clip, data.volume);
        }
    }
}