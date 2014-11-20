using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Rhetris.Audio
{
    class Music : GameComponent
    {
        private SoundEffect _loop;
        private SoundEffectInstance _loopInstance;

        public Music(Game game) : base(game) {}

        public override void Initialize()
        {
            base.Initialize();
            _loop = Game.Content.Load<SoundEffect>("AudioContent/loop");
            _loopInstance = _loop.CreateInstance();
            _loopInstance.IsLooped = true;
            _loopInstance.Volume = 0.3f;
        }

        public override void Update(GameTime gameTime)
        {
            _loopInstance.Play();
            Enabled = false;
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            _loop.Dispose();
            base.Dispose(disposing);
        }
    }
}
