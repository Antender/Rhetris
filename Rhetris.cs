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
        private Point[] _oldNextFigure;
        private double _previousMovement;
        private double _previousBeat;
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
            _input.Add(Buttons.B,             Keys.Space,  () => _oldNextFigure = _logic.SwapFigure());
            _input.Add(Buttons.DPadDown,      Keys.Down,   () => {_oldNextFigure = _logic.Drop(_previousBeat); _previousMovement = 0;});
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
            _previousBeat += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_previousMovement > (2000.0/Speed))
            {
                _oldNextFigure = _logic.MoveDown();
                _previousMovement = 0;
            }
            if (_previousBeat > 1000)
            {
                _audio.playBeat();
                _previousBeat = 0;
            }
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            _drawer.DrawAll(_oldNextFigure, _logic.NextFigure, _logic.Figure,_logic.score);
            base.Draw(gameTime);
        }
    }
}
