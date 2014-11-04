using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Rhetris
{
    internal class Audio
    {
        private SoundEffect _beat;
		private SoundEffect _subBeat;
		private SoundEffect _loop;
		private SoundEffectInstance _loopInstance;
        private bool _even;
		private bool _LoopStarted = false;

        public Audio(Rhetris parent)
        {
            Parent = parent;
        }

        public void LoadContent(ContentManager content)
        {
            _beat = content.Load<SoundEffect>("AudioContent/beat");
            _subBeat = content.Load<SoundEffect>("AudioContent/sub_beat");
			_loop = content.Load<SoundEffect>("AudioContent/loop");
			_loopInstance = _loop.CreateInstance();
			_loopInstance.IsLooped = true;
			_loopInstance.Volume = 0.3f;
        }

        public Rhetris Parent { get; set; }

		public void PlayBeat()
		{
			if(_even == false)
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

		public void StartLoop()
		{
			if(_LoopStarted == false)
			{
				_loopInstance.Play();
				_LoopStarted = true;
			}
		}
	}
}