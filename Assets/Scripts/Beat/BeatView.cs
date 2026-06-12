using Logy.UnityCommonV01;

namespace GarenaGameJam2026Team6
{
    public class BeatView
    {
        public void Beat()
        {
            SFXPlayer.instance.PlayOneShot(AudioName.beat);
        }

        public void OneTimeBeat()
        {
            SFXPlayer.instance.PlayOneShot(AudioName.oneTimeBeat);
        }
    }
}

