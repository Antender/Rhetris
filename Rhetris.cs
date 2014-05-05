using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rhetris
{

    delegate void ButtonKeyReleased();
    struct ButtonEvent
    {
        public Buttons Button;
        public ButtonKeyReleased Released;
        public ButtonEvent(Buttons b, ButtonKeyReleased d)
        {
            Button = b;
            Released = d;
        }
    }
    struct KeyEvent
    {
        public Keys Key;
        public ButtonKeyReleased Released;
        public KeyEvent(Keys k, ButtonKeyReleased d)
        {
            Key = k;
            Released = d;
        }
    }

    public class Rhetris : Game
    {
        private readonly Drawer _drawer;
        private readonly GameLogic _gamelogic;
        private readonly Random _random;
        private Point[] _clearNextFigure;
        private double _previousMovement;
        public int Width = 12;
        public int Height = 23;
        public int FigNum = 7;
        private const int Speed = 1;
        private GamePadState oldGamepadState;
        private KeyboardState oldKeyboardState;
        private readonly ButtonEvent[] _buttons = new ButtonEvent[7];
        private readonly KeyEvent[] _keys = new KeyEvent[7];
        public int Rnd(int max)
        {
            return _random.Next(max);
        }
        
        public Rhetris()
        {
            Content.RootDirectory = "Content";
            _gamelogic = new GameLogic(this);
            _drawer = new Drawer(this, _gamelogic.Blocks);
            _random = new Random();
            _gamelogic.NewGame();

            ButtonKeyReleased menuEvent = Exit;
            _buttons[0] = new ButtonEvent(Buttons.Back, menuEvent);
            _keys[0] = new KeyEvent(Keys.Escape, menuEvent);

            ButtonKeyReleased swapEvent = delegate()
            {
                if (_gamelogic.CanSwap())
                {
                    _clearNextFigure = _gamelogic.SwapFigure();
                }
            };
            _buttons[1] = new ButtonEvent(Buttons.A, swapEvent);
            _keys[1] = new KeyEvent(Keys.Space, swapEvent);

            ButtonKeyReleased downEvent = delegate()
            {
                while (_gamelogic.CanMove(new Point(0, 1)))
                {
                    _gamelogic.Move(new Point(0, 1));
                }
                _clearNextFigure = _gamelogic.NextFigure;
                _gamelogic.KillFigure();
                _gamelogic.PlaceFigure();
            };
            _buttons[2] = new ButtonEvent(Buttons.X, downEvent);
            _keys[2] = new KeyEvent(Keys.Down, downEvent);

            ButtonKeyReleased leftEvent = delegate
            {
                if (_gamelogic.CanMove(new Point(-1, 0)))
                {
                    _gamelogic.Move(new Point(-1, 0));
                }
            };
            _buttons[3] = new ButtonEvent(Buttons.LeftShoulder, leftEvent);
            _keys[3] = new KeyEvent(Keys.Left, leftEvent);

            ButtonKeyReleased rightEvent = delegate
            {
                if (_gamelogic.CanMove(new Point(1, 0)))
                {
                    _gamelogic.Move(new Point(1, 0));
                }
            };
            _buttons[4] = new ButtonEvent(Buttons.RightShoulder, rightEvent);
            _keys[4] = new KeyEvent(Keys.Right, rightEvent);

            ButtonKeyReleased rotateCwEvent = () => _gamelogic.RotateClockwize();
            _buttons[5] = new ButtonEvent(Buttons.B, rotateCwEvent);
            _keys[5] = new KeyEvent(Keys.Z, rotateCwEvent);

            ButtonKeyReleased rotateCcwEvent = () => _gamelogic.RotateCounterClockwize();
            _buttons[6] = new ButtonEvent(Buttons.B, rotateCcwEvent);
            _keys[6] = new KeyEvent(Keys.X, rotateCcwEvent);

            oldGamepadState = GamePad.GetState(PlayerIndex.One);
            oldKeyboardState = Keyboard.GetState();
        }

        protected override void Update(GameTime gameTime)
        {
            var currentGamepadState = GamePad.GetState(PlayerIndex.One);
            var currentKeyboardState = Keyboard.GetState();
            foreach (var button in _buttons.Where(button => oldGamepadState.IsButtonDown(button.Button)).Where(button => currentGamepadState.IsButtonUp(button.Button)))
            {
                button.Released();
            }
            foreach (var key in _keys.Where(key => oldKeyboardState.IsKeyDown(key.Key)).Where(key => currentKeyboardState.IsKeyUp(key.Key)))
            {
                key.Released();
            }
            oldGamepadState = currentGamepadState;
            oldKeyboardState = currentKeyboardState;
            if ((gameTime.TotalGameTime.TotalMilliseconds - _previousMovement) > (2000.0/Speed))
            {
                if (_gamelogic.CanMove(new Point(0,1)))
                {
                    _gamelogic.Move(new Point(0,1));
                }
                else
                {
                    _clearNextFigure = _gamelogic.NextFigure;
                    _gamelogic.KillFigure();
                    _gamelogic.PlaceFigure();
                }
                _previousMovement = gameTime.TotalGameTime.TotalMilliseconds;
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            if (_clearNextFigure != null)
            {
                _drawer.DrawNextFigure(_clearNextFigure, (uint)BlockType.Empty);
            }            
            _drawer.DrawNextFigure(_gamelogic.NextFigure, (uint)BlockType.Alive);
            _drawer.DrawField();
            _drawer.DrawFigure(_gamelogic.Figure,(uint) BlockType.Alive);
            base.Draw(gameTime);
        }
    }
}
