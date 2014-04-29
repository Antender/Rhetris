using System;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using Microsoft.Xna.Framework;

namespace Rhetris
{
    public enum BlockType : uint
    {
        Empty = 0,
        Wall = 1,
        Dead = 2,
        Alive = 3
    }

    public enum Tetromino : uint
    {
        I = 0,
        J = 1,
        L = 2,
        O = 3,
        S = 4,
        T = 5,
        Z = 6
    }
    class GameLogic
    {
        public Point[] figure;
        public Point[] nextFigure;
        private Point[][] figures;
        private readonly Rhetris _parent;
        public uint[,] Blocks;
        public Point Start;

        public GameLogic(Rhetris main)
        {
            _parent = main;
            Blocks = new uint[main.Width, main.Height];
            Start = new Point(_parent.Width/2,0);
            CreateFigures();
        }

        private void CreateFigures()
        {
            figures = new Point[_parent.FigNum][];
            figures[(uint)Tetromino.I] = new []
            {
                new Point(0, 0),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(2, 0) 
            };
            figures[(uint)Tetromino.J] = new []
            {
                new Point(0, 0),
                new Point(-1, 0),
                new Point(1, 0), 
                new Point(1, 1) 
            };
            figures[(uint) Tetromino.L] = new []
            {
                new Point(0, 0),
                new Point(-1,0),
                new Point(-1,1),
                new Point(1,0) 
            };
            figures[(uint) Tetromino.O] = new []
            {
                new Point(0,0),
                new Point(0,1),
                new Point(1,0),
                new Point(1,1)
            };
            figures[(uint) Tetromino.S] = new []
            {
                new Point(0,0),
                new Point(1,0),
                new Point(0,1),
                new Point(-1,1) 
            };
            figures[(uint) Tetromino.T] = new []
            {
                new Point(0,0),
                new Point(1,0),
                new Point(-1,0),
                new Point(0,1) 
            };
            figures[(uint) Tetromino.Z] = new []
            {
                new Point(0,0),
                new Point(-1,0),
                new Point(0,1),
                new Point(1,1) 
            };
        }

        public void SpawnFigure()
        {
            nextFigure = (Point[])figures[_parent.Rnd(7)].Clone();
        }

        public void PlaceFigure()
        {
            figure = new Point[nextFigure.Length];
            for (var i = 0; i < nextFigure.Length; i++)
            {
                figure[i] = new Point(nextFigure[i].X + Start.X, nextFigure[i].Y + Start.Y);
            }
            SpawnFigure();
        }

        public void NewGame()
        {
            for (var i = 0; i < _parent.Height; i++)
            {
                Blocks[0, i] = (uint) BlockType.Wall;
                Blocks[_parent.Width - 1, i] = (uint) BlockType.Wall;
            }
            for (var i = 1; i < _parent.Width-1; i++)
            {
                for (var j = 0; j < _parent.Width-1; j++)
                {
                    Blocks[i, j] = (uint)BlockType.Empty;
                }
                Blocks[i, _parent.Height - 1] = (uint) BlockType.Wall;
            }
            SpawnFigure();
            PlaceFigure();
        }

        public uint GetBlock(Point pos)
        {
            return Blocks[pos.X, pos.Y];
        }

        public bool CanMove(Point direction)
        {
            return figure.All(block => Blocks[block.X + direction.X, block.Y + direction.Y] == (uint) BlockType.Empty);
        }

        public void Move(Point direction)
        {
            for (var i = 0; i < figure.Length; i++)
            {
                figure[i] = new Point(figure[i].X + direction.X,figure[i].Y + direction.Y);
            }
        }

        public void KillFigure()
        {
            foreach (var block in figure)
            {
                Blocks[block.X, block.Y] = (uint) BlockType.Dead;
            }
        }

        public bool CanSwap()
        {
            var origin = figure[0];
            return nextFigure.All(block => Blocks[block.X + origin.X, block.Y + origin.Y] == (uint) BlockType.Empty);
        }

        public Point[] SwapFigure()
        {
            var origin = figure[0];
            var newNextFigure = new Point[nextFigure.Length];
            var cleared = nextFigure;
            for (var i = 0; i < figure.Length; i++)
            {
                figure[i] = new Point(figure[i].X - origin.X, figure[i].Y - origin.Y);
            }
            for (var i = 0; i < nextFigure.Length; i++)
            {
                newNextFigure[i] = new Point(nextFigure[i].X + origin.X, nextFigure[i].Y + origin.Y);
            }
            var temp = figure;
            figure = newNextFigure;
            nextFigure = temp;
            return cleared;
        }
    }
}
