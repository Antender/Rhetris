using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rhetris
{
    public enum GameState
    {
        Playing,
        GameOver,
        GameOverLabel,
        Win,
        NewGame
    }

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
        public double Speed = 1.0;
        public float nextBeat;
        public GameState state;

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
            NewGame();
        }

        protected override void LoadContent()
        {
            _drawer.LoadContent();
            _drawer.ResetPalette();
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
 
            _input.Update();
            switch (state)
            {
                case GameState.Playing:                   
                    _previousMovement += gameTime.ElapsedGameTime.TotalMilliseconds;
                    _previousBeat += gameTime.ElapsedGameTime.TotalMilliseconds;
                    _logic.SetTime(_previousBeat);
                    if (_previousMovement > (2000.0 * Speed))
                    {
                        _oldNextFigure = _logic.MoveDown();                        
                        _previousMovement = 0;
                        
                        _audio.playBeat();
                    }
                    if (_previousBeat > nextBeat)
                    {
                        nextBeat = _random.Next(1000, 3000);
                        _previousBeat = 0;
                    }

                    break;
                case GameState.GameOver:
                    break;
            }
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            switch (state)
            {
                case GameState.NewGame:
                    _drawer.ClearForNewgame();
                    state = GameState.Playing;
                    break;
                case GameState.Playing:
                    _drawer.DrawAll(_oldNextFigure, _logic.NextFigure, _logic.Figure, _logic.score); 
                    _drawer.DrawLabels();
                    _drawer.DrawLimit();
                    break;
                case GameState.GameOver:
                    _drawer.DrawGameOver(_random.Next(_drawer.GetAllColors()));
                    break;
                case GameState.GameOverLabel:
                    _drawer.DrawGameOverLabel();
                    break;
                case GameState.Win:
                    _drawer.DrawWinLabel();
                    break;
            }
            base.Draw(gameTime);
        }

        public void GameOver()
        {
            state = GameState.GameOver;
            _input.Clear();
            _input.Add(Buttons.Back, Keys.Escape, Exit);
            _input.Add(Buttons.B, Keys.Space, NewGame);
            _input.Add(Buttons.DPadDown, Keys.Down, () => {});
            _input.Add(Buttons.DPadLeft, Keys.Left, () => {});
            _input.Add(Buttons.DPadRight, Keys.Right, () => {});
            _input.Add(Buttons.LeftShoulder, Keys.Z, () => {});
            _input.Add(Buttons.RightShoulder, Keys.X, () => {});
            _input.Add(Buttons.A, Keys.Up, () => {});
            _drawer.SetGameOver();
        }

        public void GameOverLabel()
        {
            state = GameState.GameOverLabel;
        }

        public void NewGame()
        {
            _logic.NewGame();
            _input.Clear();
            _input.Add(Buttons.Back, Keys.Escape, Exit);
            _input.Add(Buttons.B, Keys.Space, () => _oldNextFigure = _logic.SwapFigure());
            _input.Add(Buttons.DPadDown, Keys.Down, () => { _oldNextFigure = _logic.Drop(_previousBeat); _previousMovement = 0; });
            _input.Add(Buttons.DPadLeft, Keys.Left, () => _logic.MoveLeft());
            _input.Add(Buttons.DPadRight, Keys.Right, () => _logic.MoveRight());
            _input.Add(Buttons.LeftShoulder, Keys.Z, () => _logic.RotateCounterClockwize());
            _input.Add(Buttons.RightShoulder, Keys.X, () => _logic.RotateClockwize());
            _input.Add(Buttons.A, Keys.Up, () => _logic.RotateClockwize());
            _drawer.NewGame();
            nextBeat = 500;
            state = GameState.NewGame;
        }

        public void Win()
        {
            state = GameState.Win;
            _input.Clear();
            _input.Add(Buttons.Back, Keys.Escape, Exit);
            _input.Add(Buttons.B, Keys.Space, NewGame);
            _input.Add(Buttons.DPadDown, Keys.Down, () => { });
            _input.Add(Buttons.DPadLeft, Keys.Left, () => { });
            _input.Add(Buttons.DPadRight, Keys.Right, () => { });
            _input.Add(Buttons.LeftShoulder, Keys.Z, () => { });
            _input.Add(Buttons.RightShoulder, Keys.X, () => { });
            _input.Add(Buttons.A, Keys.Up, () => { });
        }

        public void CheckScore()
        {
            if (Math.Abs(_logic.score.points) < (nextBeat * 0.35))
            {
                _drawer.NextPalette();      
            }
            else
            {
                _drawer.ResetPalette();
            }
            _drawer.SetLimit((int)nextBeat * 0.35);
            nextBeat = _random.Next(1000, 3000);
            _previousBeat = 0;
        }
    }
}
