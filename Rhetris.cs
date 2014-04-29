using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rhetris
{
    public class Rhetris : Game
    {
        private readonly Drawer _drawer;
        private readonly GameLogic _gamelogic;
        private readonly Random _random;
        private Point[] _clearNextFigure;
        private double _previousUpdate;
        private double _previousMovement;
        public int Width = 12;
        public int Height = 23;
        public int FigNum = 7;
        private int speed = 1;
        
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
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - _previousUpdate > 70)
            {

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    if (_gamelogic.CanSwap())
                    {
                        _clearNextFigure = _gamelogic.SwapFigure();
                    }
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    if (_gamelogic.CanMove(new Point(-1, 0)))
                    {
                        _gamelogic.Move(new Point(-1,0));
                    }
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    if (_gamelogic.CanMove(new Point(1, 0)))
                    {
                        _gamelogic.Move(new Point(1, 0));
                    }
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    while (_gamelogic.CanMove(new Point(0, 1)))
                    {
                        _gamelogic.Move(new Point(0,1));
                    }
                    _clearNextFigure = _gamelogic.NextFigure;
                    _gamelogic.KillFigure();
                    _gamelogic.PlaceFigure();
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Z))
                {
                    _gamelogic.RotateClockwize();
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.X))
                {
                    _gamelogic.RotateCounterClockwize();
                }
                _previousUpdate = gameTime.TotalGameTime.TotalMilliseconds;
            }
            if ((gameTime.TotalGameTime.TotalMilliseconds - _previousMovement) > (2000.0/speed))
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
