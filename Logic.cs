using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Rhetris
{
    internal struct Score
    {
        public bool Late;
        public int Points;

        public Score(double delta, bool l)
        {
            Points = (int) delta;
            Late = l;
        }
    }

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

    internal class Logic
    {
        private readonly Rhetris _parent;
        public uint[,] Blocks;
        public Point[] Figure;
        public Point[] NextFigure;
        public Point Start;
        private Point[][] _figures;
        public Score Score;
        public double Time;

        public Logic(Rhetris main)
        {
            _parent = main;
            Blocks = new uint[main.Width, main.Height];
            Start = new Point(_parent.Width/2, 2);
            CreateFigures();
        }

        private void CreateFigures()
        {
            _figures = new Point[_parent.FigNum][];
            _figures[(uint) Tetromino.I] = new[]
            {
                new Point(0, 0),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(2, 0)
            };
            _figures[(uint) Tetromino.J] = new[]
            {
                new Point(0, 0),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(1, 1)
            };
            _figures[(uint) Tetromino.L] = new[]
            {
                new Point(0, 0),
                new Point(-1, 0),
                new Point(-1, 1),
                new Point(1, 0)
            };
            _figures[(uint) Tetromino.O] = new[]
            {
                new Point(0, 0),
                new Point(0, 1),
                new Point(1, 0),
                new Point(1, 1)
            };
            _figures[(uint) Tetromino.S] = new[]
            {
                new Point(0, 0),
                new Point(1, 0),
                new Point(0, 1),
                new Point(-1, 1)
            };
            _figures[(uint) Tetromino.T] = new[]
            {
                new Point(0, 0),
                new Point(1, 0),
                new Point(-1, 0),
                new Point(0, 1)
            };
            _figures[(uint) Tetromino.Z] = new[]
            {
                new Point(0, 0),
                new Point(-1, 0),
                new Point(0, 1),
                new Point(1, 1)
            };
        }

        public void SpawnFigure()
        {
            NextFigure = (Point[]) _figures[_parent.Rnd(7)].Clone();
        }

        public void PlaceFigure()
        {
            Figure = new Point[NextFigure.Length];
            for (int i = 0; i < NextFigure.Length; i++)
            {
                Figure[i] = new Point(NextFigure[i].X + Start.X, NextFigure[i].Y + Start.Y);
                if (GetBlock(Figure[i]) != (uint) BlockType.Empty)
                {
                    _parent.GameOver();
                }
            }
            SpawnFigure();
        }

        public void NewGame()
        {
            _parent.State = GameState.Playing;
            for (int i = 0; i < _parent.Height; i++)
            {
                Blocks[0, i] = (uint) BlockType.Wall;
                Blocks[_parent.Width - 1, i] = (uint) BlockType.Wall;
            }
            for (int i = 1; i < _parent.Width - 1; i++)
            {
                for (int j = 0; j < _parent.Height - 1; j++)
                {
                    Blocks[i, j] = (uint) BlockType.Empty;
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
            return Figure.All(block => Blocks[block.X + direction.X, block.Y + direction.Y] == (uint) BlockType.Empty);
        }

        public void Move(Point direction)
        {
            for (int i = 0; i < Figure.Length; i++)
            {
                Figure[i] = new Point(Figure[i].X + direction.X, Figure[i].Y + direction.Y);
            }
        }

        public void KillFigure()
        {
            foreach (Point block in Figure)
            {
                Blocks[block.X, block.Y] = (uint) BlockType.Dead;
            }
            ComputeScore(_parent.NextBeat, Time);
            _parent.CheckScore();
            CheckDeleted();
        }

        public Point[] SwapFigure()
        {
            Point origin = Figure[0];
            if (NextFigure.Any(block => Blocks[block.X + origin.X, block.Y + origin.Y] != (uint) BlockType.Empty))
            {
                return null;
            }
            var newNextFigure = new Point[NextFigure.Length];
            Point[] cleared = NextFigure;
            for (int i = 0; i < Figure.Length; i++)
            {
                Figure[i] = new Point(Figure[i].X - origin.X, Figure[i].Y - origin.Y);
            }
            for (int i = 0; i < NextFigure.Length; i++)
            {
                newNextFigure[i] = new Point(NextFigure[i].X + origin.X, NextFigure[i].Y + origin.Y);
            }
            Point[] temp = Figure;
            Figure = newNextFigure;
            NextFigure = temp;
            return cleared;
        }

        public bool RotateClockwize()
        {
            Point origin = Figure[0];
            var temp = new Point[Figure.Length];
            bool rotated = true;
            for (int i = 0; i < Figure.Length; i++)
            {
                temp[i] = new Point(origin.X - Figure[i].Y + origin.Y, Figure[i].X - origin.X + origin.Y);
                if ((temp[i].X >= 0) && (temp[i].Y >= 0) && (Blocks[temp[i].X, temp[i].Y] == (uint) BlockType.Empty))
                    continue;
                rotated = false;
                break;
            }
            if (rotated)
            {
                Figure = temp;
            }
            return rotated;
        }

        public bool RotateCounterClockwize()
        {
            Point origin = Figure[0];
            var temp = new Point[Figure.Length];
            bool rotated = true;
            for (int i = 0; i < Figure.Length; i++)
            {
                temp[i] = new Point(origin.X + Figure[i].Y - origin.Y, origin.Y - Figure[i].X + origin.X);
                if ((temp[i].X >= 0) && (temp[i].Y >= 0) && (Blocks[temp[i].X, temp[i].Y] == (uint) BlockType.Empty))
                    continue;
                rotated = false;
                break;
            }
            if (rotated)
            {
                Figure = temp;
            }
            return rotated;
        }

        public void CheckDeleted()
        {
            int shift = 0;
            for (int i = _parent.Height - 2; i >= 0; i--)
            {
                bool full = true;
                for (int j = 1; j < _parent.Width - 1; j++)
                {
                    if (Blocks[j, i] != (uint) BlockType.Empty) continue;
                    full = false;
                    break;
                }
                if (full)
                {
                    shift++;
                }
                else
                {
                    if (shift > 0)
                    {
                        for (int j = 1; j < _parent.Width - 1; j++)
                        {
                            Blocks[j, i + shift] = Blocks[j, i];
                        }
                    }
                }
            }
        }

        public Point[] Drop(double prevBeat)
        {
            while (CanMove(new Point(0, 1)))
            {
                Move(new Point(0, 1));
            }
            Point[] temp = NextFigure;
            KillFigure();
            PlaceFigure();
            return temp;
        }

        public void MoveLeft()
        {
            if (CanMove(new Point(-1, 0)))
            {
                Move(new Point(-1, 0));
            }
        }

        public void MoveRight()
        {
            if (CanMove(new Point(1, 0)))
            {
                Move(new Point(1, 0));
            }
        }

        public Point[] MoveDown()
        {
            if (CanMove(new Point(0, 1))) //Move tetromino down. No NextFigure redrawing.
            {
                Move(new Point(0, 1));
                return null;
            }
            Point[] temp = NextFigure;
            KillFigure();
            PlaceFigure();
            return temp;
        }

        public void ComputeScore(float nextBeat, double currentDelta)
        {
            var dfuture = (float) (Math.Abs(nextBeat - currentDelta));
            if (currentDelta > dfuture)
            {
                Score = new Score(Math.Abs(dfuture), false);
            }
            else
            {
                Score = new Score(Math.Abs(currentDelta), true);
            }
        }

        public void SetTime(double t)
        {
            Time = t;
        }
    }
}