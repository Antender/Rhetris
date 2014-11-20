using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rhetris.Input
{
    class GamePadController : Controller
    {
        public Buttons[] BoundButtons
        {
            get { return _boundButtons; }
            set
            {
                if (value.Length != ActionCapacity)
                {
                    throw new Exception("incompatible Buttons array length");
                }
                _boundButtons = value;
            }
        }

        private Buttons[] _boundButtons;

        private GamePadState _oldGamePadState;
        private GamePadState _oldPeriodicGamePadState;
        private int _buttonwaspressed;
        private readonly PlayerIndex _playerIndex;

        public GamePadController(PlayerIndex playerindex, Game game, TimeSpan timespan) : base(game, timespan)
        {
            _playerIndex = playerindex;
        }

        public override void Update(GameTime gameTime)
        {
            var currentGamePadState = GamePad.GetState(_playerIndex);
            for (var buttonIndex = 0; buttonIndex < ActionCapacity; buttonIndex++)
            {
                if (_oldGamePadState.IsButtonDown(BoundButtons[buttonIndex]) && currentGamePadState.IsButtonUp(BoundButtons[buttonIndex]))
                {
                    _buttonwaspressed = buttonIndex;
                    Actions[buttonIndex]();
                }
            }
            base.Update(gameTime);
            _oldGamePadState = currentGamePadState;
        }

        public override void PeriodicUpdate(TimeSpan timespan)
        {
            var currentGamePadState = GamePad.GetState(_playerIndex);
            for (var buttonIndex = 0; buttonIndex < ActionCapacity; buttonIndex++)
            {
                if ((buttonIndex != _buttonwaspressed) && _oldPeriodicGamePadState.IsButtonDown(BoundButtons[buttonIndex]) && currentGamePadState.IsButtonDown(BoundButtons[buttonIndex]))
                {
                    Actions[buttonIndex]();
                }
            }
            _buttonwaspressed = -1;
            _oldPeriodicGamePadState = currentGamePadState;
        }
    }
}
