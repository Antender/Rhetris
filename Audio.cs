using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Rhetris
{
    internal class Audio
    {
        private SoundEffect _beat;
        private SoundEffect _subBeat;
        private bool _even;

        public Audio(Rhetris parent)
        {
            Parent = parent;
        }

        public void LoadContent(ContentManager content)
        {
            _beat = content.Load<SoundEffect>("AudioContent/beat");
            _subBeat = content.Load<SoundEffect>("AudioContent/sub_beat");
        }

        public Rhetris Parent { get; set; }

        public void PlayBeat()
        {
            if (_even == false)
            {
                _beat.Play();
                _even = true;
            }
            else
            {
                _subBeat.Play();
                _even = false;
            }
        }
    }
}