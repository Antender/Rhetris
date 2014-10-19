using System.IO;
using System.Media;

namespace Rhetris
{
    internal class Audio
    {
        private readonly SoundPlayer _beatPlayer;
        private readonly SoundPlayer _subBeatPlayer;
        private bool _even;

        public Audio(Rhetris parent)
        {
            Parent = parent;
            _beatPlayer = new SoundPlayer(new FileStream("Content\\Audio\\beat.wav", FileMode.Open));
            _subBeatPlayer = new SoundPlayer(new FileStream("Content\\Audio\\sub_beat.wav", FileMode.Open));
        }

        public Rhetris Parent { get; set; }

        public void PlayBeat()
        {
            if (_even == false)
            {
                _beatPlayer.Play();
                _even = true;
            }
            else
            {
                _subBeatPlayer.Play();
                _even = false;
            }
        }
    }
}