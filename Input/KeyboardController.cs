using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rhetris.Input
{
    class KeyboardController : Controller
    {
        public Keys[] BoundKeys
        {
            get { return _boundKeys; }
            set
            {
                if (value.Length != ActionCapacity)
                {
                    throw new Exception("incompatible keys array length");
                }
                _boundKeys = value;
            }
        }

        private Keys[] _boundKeys;

        private KeyboardState _oldKeyboardState;
        private KeyboardState _oldPeriodicKeyboardState;
        private int _keywaspressed;

        public KeyboardController(Game game, TimeSpan timespan) : base(game, timespan) {}

        public override void Update(GameTime gameTime)
        {
            var currentKeyboardState = Keyboard.GetState();
            for (var keyIndex = 0; keyIndex < ActionCapacity; keyIndex++)
            {
                if (_oldKeyboardState.IsKeyDown(BoundKeys[keyIndex]) && currentKeyboardState.IsKeyUp(BoundKeys[keyIndex]))
                {
                    _keywaspressed = keyIndex;
                    Actions[keyIndex]();
                }
            }    
            base.Update(gameTime);
            _oldKeyboardState = currentKeyboardState;
        }

        public override void PeriodicUpdate(TimeSpan timespan)
        {
            var currentKeyboardState = Keyboard.GetState();
            for (var keyIndex = 0; keyIndex < ActionCapacity; keyIndex++)
            {
                if ((keyIndex!=_keywaspressed) && _oldPeriodicKeyboardState.IsKeyDown(BoundKeys[keyIndex]) && currentKeyboardState.IsKeyDown(BoundKeys[keyIndex]))
                {
                    Actions[keyIndex]();
                }
            }
            _keywaspressed = -1;
            _oldPeriodicKeyboardState = currentKeyboardState;
        }
    }
}
