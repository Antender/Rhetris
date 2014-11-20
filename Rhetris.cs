using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Rhetris.Audio;
using Rhetris.Input;

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
        private readonly Beat _music;
        private readonly Drawer _drawer;
        private readonly GamePadController _gamePadController;
        private readonly KeyboardController _keyboardController;
        private readonly MouseController _mouseController;
        private readonly TouchController _touchController;
        private readonly Logic _logic;
        private readonly Random _random;
        public int FigNum = 7;
        public int Height = 23;

        public double Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        private double _speed = 1.0;
        public int Width = 12;
        private Point[] _oldNextFigure;
        private double _previousBeat;
        public double NextBeat;
        public GameState State;

        public Rhetris()
        {
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 1000.0f);
            _logic = new Logic(this);
            _drawer = new Drawer(this, _logic.Blocks);
            _random = new Random();
            Components.Add(_keyboardController = new KeyboardController(this, TimeSpan.FromMilliseconds(150)));
            _keyboardController.BoundKeys = new []
            {
                Keys.Left,
                Keys.Right,
                Keys.Down,
                Keys.Z,
                Keys.X,
                Keys.Space,
                Keys.Escape           
            };
            Components.Add(_gamePadController = new GamePadController(PlayerIndex.One, this, TimeSpan.FromMilliseconds(150)));
            _gamePadController.BoundButtons = new[]
            {
                Buttons.DPadLeft,
                Buttons.DPadRight,
                Buttons.DPadDown,
                Buttons.LeftShoulder,
                Buttons.RightShoulder,
                Buttons.B,
                Buttons.Start
            };
            Components.Add(_music = new Beat(this,TimeSpan.FromSeconds(1)));
            _music.Enabled = true;
            NewGame();
        }

        public int Rnd(int max)
        {
            return _random.Next(max);
        }

        protected override void LoadContent()
        {
            _drawer.LoadContent();
            _drawer.ResetPalette();
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            switch (State)
            {
                case GameState.Playing:
                    _previousBeat += gameTime.ElapsedGameTime.TotalMilliseconds;
                    _logic.SetTime(_previousBeat);
                    if (_previousBeat > NextBeat)
                    {
						_oldNextFigure = _logic.MoveDown();

                        NextBeat = 2000.0*Speed;
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
            switch (State)
            {
                case GameState.NewGame:
                    _drawer.ClearForNewgame();
                    State = GameState.Playing;
                    break;
                case GameState.Playing:
                    _drawer.DrawAll(_oldNextFigure, _logic.NextFigure, _logic.Figure, _logic.Score);
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
            State = GameState.GameOver;
            RestrictControllers();
            _drawer.SetGameOver();
            _music.Enabled = false;
        }

        public void GameOverLabel()
        {
            State = GameState.GameOverLabel;
        }

        public void NewGame()
        {
            _logic.NewGame();
            Controller.Actions = new Input.Action[]
            {
                () => _logic.MoveLeft(),
                () => _logic.MoveRight(),
                () =>
                {
                    _oldNextFigure = _logic.Drop(_previousBeat);
                },
                () => _logic.RotateClockwize(),
                () => _logic.RotateCounterClockwize(),
                () => _oldNextFigure = _logic.SwapFigure(),
                Exit
            };
            _drawer.NewGame();
            NextBeat = 500;
            State = GameState.NewGame;
        }

        public void Win()
        {
            State = GameState.Win;
            RestrictControllers();
            _music.Enabled = false;
        }

        public void CheckScore()
        {
            if (Math.Abs(_logic.Score.Points) < (NextBeat*0.35))
            {
                _drawer.NextPalette();
            }
            else
            {
                _drawer.ResetPalette();
            }
            _drawer.SetLimit((int) NextBeat*0.35);
        }

        public void RestrictControllers()
        {
            Controller.Actions = new Input.Action[]
            {
                delegate {},
                delegate {},
                delegate {},
                delegate {},
                delegate {},
                NewGame,
                Exit
            };
        }
    }
}