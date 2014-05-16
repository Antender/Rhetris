using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rhetris
{
    public class Rhetris : Game
    {
        private readonly Drawer _drawer;
        private readonly Logic _logic;
        private readonly Random _random;
        private readonly Input _input;
        private readonly Audio _audio;
        private Point[] _clearNextFigure;
        private double _previousMovement;
        private double _previousBeep;
        public int Width = 12;
        public int Height = 23;
        public int FigNum = 7;
        private const int Speed = 1;
        public int Rnd(int max)
        {
            return _random.Next(max);
        }
        
        public Rhetris()
        {
            Content.RootDirectory = "Content";
            _logic = new Logic(this);
            _drawer = new Drawer(this, _logic.Blocks);
            _input = new Input(8);
            _random = new Random();
            _audio = new Audio(this);
            _logic.NewGame();
            _input.Add(Buttons.Back,          Keys.Escape, Exit);
            _input.Add(Buttons.B,             Keys.Space,  () => _clearNextFigure = _logic.SwapFigure());
            _input.Add(Buttons.DPadDown,      Keys.Down,   () => {_clearNextFigure = _logic.Drop(); _previousMovement = 0;});
            _input.Add(Buttons.DPadLeft,      Keys.Left,   () => _logic.MoveLeft());
            _input.Add(Buttons.DPadRight,     Keys.Right,  () => _logic.MoveRight());
            _input.Add(Buttons.LeftShoulder,  Keys.Z,      () => _logic.RotateCounterClockwize());
            _input.Add(Buttons.RightShoulder, Keys.X,      () => _logic.RotateClockwize());
            _input.Add(Buttons.A,             Keys.Up,     () => _logic.RotateClockwize());
        }

        protected override void LoadContent()
        {
            _drawer.LoadContent();
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _input.Update();
            _previousMovement += gameTime.ElapsedGameTime.TotalMilliseconds;
            _previousBeep += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_previousMovement > (2000.0/Speed))
            {
                _clearNextFigure = _logic.MoveDown();
                _previousMovement = 0;
            }
            if (_previousBeep > 1000)
            {
                _audio.playBeep();
                _previousBeep = 0;
            }
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            _drawer.DrawAll(_clearNextFigure, _logic.NextFigure, _logic.Figure);
            base.Draw(gameTime);
        }
    }
}
