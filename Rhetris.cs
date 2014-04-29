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
        private Point[] _clearFigure;
        private Point[] _clearNextFigure;
        private double _previousUpdate;
        public int Width = 12;
        public int Height = 23;
        public int FigNum = 7;
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

        protected override void LoadContent()
        {
            
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - _previousUpdate > 100)
            {

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    _clearFigure = _gamelogic.figure;
                    _clearNextFigure = _gamelogic.nextFigure;
                    _gamelogic.PlaceFigure();
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
                _previousUpdate = gameTime.TotalGameTime.TotalMilliseconds;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_clearFigure != null)
            {
                _drawer.DrawFigure(_clearFigure,(uint) BlockType.Empty);
            }
            if (_clearNextFigure != null)
            {
                _drawer.DrawNextFigure(_clearNextFigure, (uint)BlockType.Empty);
            }            
            _drawer.DrawNextFigure(_gamelogic.nextFigure, (uint)BlockType.Alive);
            _drawer.DrawField();
            _drawer.DrawFigure(_gamelogic.figure,(uint) BlockType.Alive);
            base.Draw(gameTime);
        }
    }
}
