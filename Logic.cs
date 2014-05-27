using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Rhetris
{
    struct Score
    {
        bool late;
        int points;

        public Score(double prevBeat)
        {
            if (prevBeat > 500)
            {
                late = false;
            }
            else
            {
                late = true;
            }

            points = 500 - Math.Abs(500 - (int)prevBeat);
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

    class Logic
    {
        public Point[] Figure;
        public Point[] NextFigure;
        private Point[][] _figures;
        private readonly Rhetris _parent;
        public uint[,] Blocks;
        public Point Start;
        Score score;
        public Logic(Rhetris main)
        {
            _parent = main;
            Blocks = new uint[main.Width, main.Height];
            Start = new Point(_parent.Width/2, 0);
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
            for (var i = 0; i < NextFigure.Length; i++)
            {
                Figure[i] = new Point(NextFigure[i].X + Start.X, NextFigure[i].Y + Start.Y);
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
            for (var i = 1; i < _parent.Width - 1; i++)
            {
                for (var j = 0; j < _parent.Width - 1; j++)
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
            for (var i = 0; i < Figure.Length; i++)
            {
                Figure[i] = new Point(Figure[i].X + direction.X, Figure[i].Y + direction.Y);
            }
        }

        public void KillFigure()
        {
            foreach (var block in Figure)
            {
                Blocks[block.X, block.Y] = (uint) BlockType.Dead;
            }
            CheckDeleted();
        }

        public Point[] SwapFigure()
        {
            var origin = Figure[0];
            if (NextFigure.Any(block => Blocks[block.X + origin.X, block.Y + origin.Y] != (uint)BlockType.Empty))
            {
                return null;
            }
            var newNextFigure = new Point[NextFigure.Length];
            var cleared = NextFigure;
            for (var i = 0; i < Figure.Length; i++)
            {
                Figure[i] = new Point(Figure[i].X - origin.X, Figure[i].Y - origin.Y);
            }
            for (var i = 0; i < NextFigure.Length; i++)
            {
                newNextFigure[i] = new Point(NextFigure[i].X + origin.X, NextFigure[i].Y + origin.Y);
            }
            var temp = Figure;
            Figure = newNextFigure;
            NextFigure = temp;
            return cleared;
        }

        public bool RotateClockwize()
        {
            var origin = Figure[0];
            var temp = new Point[Figure.Length];
            var rotated = true;
            for (var i = 0; i < Figure.Length; i++)
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
            var origin = Figure[0];
            var temp = new Point[Figure.Length];
            var rotated = true;
            for (var i = 0; i < Figure.Length; i++)
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
            var shift = 0;
            for (var i = _parent.Height-2; i >= shift; i--)
            {
                var full = true;
                for (var j = 1; j < _parent.Width-1; j++)
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
                        for (var j = 1; j < _parent.Width - 1; j++)
                        {
                            Blocks[j, i + shift] = Blocks[j, i];
                        }
                    }
                }
            }
            for (var i = 0; i < shift; i++)
            {
                for (var j = 1; j < _parent.Width-1; j++)
                {
                    Blocks[j, i] = (uint) BlockType.Empty;
                }
            }
        }

        public Point[] Drop(double prevBeat)
        {
            while (CanMove(new Point(0, 1)))
            {
                Move(new Point(0, 1));
            }
            var temp  = NextFigure;
            KillFigure();
            PlaceFigure();
            score = new Score(prevBeat); //Scoring
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
            else //Tetromino dies. NextFigure is to be redrawn.
            {
                var temp = NextFigure;
                KillFigure();
                PlaceFigure();
                return temp;
            }
        }
    }
}
