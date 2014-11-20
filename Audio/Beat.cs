using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using XNAExtensions;

namespace Rhetris.Audio
{
    class Beat : PeriodicGameComponent
    {
        private SoundEffect _beatSound;
        private SoundEffectInstance _beat;
		private SoundEffect _subBeatSound;
        private SoundEffectInstance _subBeat;
        private bool _even;

        public Beat(Game game, TimeSpan span) : base (game, span) {}

        public override void Initialize()
        {
            base.Initialize();
            _beatSound = Game.Content.Load<SoundEffect>("AudioContent/beat");
            _beat = _beatSound.CreateInstance();
            _subBeatSound = Game.Content.Load<SoundEffect>("AudioContent/sub_beat");
            _subBeat = _subBeatSound.CreateInstance();
        }

		public override void PeriodicUpdate(TimeSpan updateInterval)
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

        protected override void Dispose(bool disposing)
        {
            _beat.Dispose();
            _subBeat.Dispose();
            base.Dispose(disposing);
        }
    }
}