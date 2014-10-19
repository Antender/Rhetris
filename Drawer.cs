using System.IO;
using BmFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rhetris
{
    internal class Drawer
    {
        private const int Palettewidth = 16;
        private const int BasePalettewidth = 7;
        private const int Blockwidth = 32;
        private const int Blockheight = 32;
        private const int GameOverSpeed = 5;
        private readonly uint[,] _gamefield;
        private readonly Rhetris _parent;
        private Point _goBlock;
        private uint _goBlockColor;
        private Texture2D _background;
        private FontRenderer _fontrenderer;
        private GraphicsDeviceManager _graphicsManager;
        private Point _nextFigure;
        private Texture2D[] _palette;
        private Texture2D[][] _palettes;
        private SpriteBatch _spriteBatch;
        public int Basepalette;
        private Texture2D _blacktexture;
        public int Currentpalette;
        private double _limit;

        public Drawer(Rhetris main, uint[,] gamefield)
        {
            _parent = main;
            _gamefield = gamefield;
            _graphicsManager = new GraphicsDeviceManager(_parent)
            {
                PreferredBackBufferWidth = Blockwidth*(_parent.Width + 6),
                PreferredBackBufferHeight = Blockheight*(_parent.Height - 2),
            };
        }

        public void LoadContent()
        {
            _spriteBatch = new SpriteBatch(_parent.GraphicsDevice);
            _palettes = new Texture2D[BasePalettewidth][];
            var palettedata = new uint[Blockwidth*Blockheight];
            _blacktexture = new Texture2D(_parent.GraphicsDevice, Blockwidth, Blockheight);
            for (int j = 0; j < Blockwidth*Blockheight; j++)
            {
                palettedata[j] = Color.Black.PackedValue;
            }
            _blacktexture.SetData(palettedata);
            for (int b = 0; b < BasePalettewidth; b++)
            {
                _palette = new Texture2D[Palettewidth];
                var basecolor = (uint) (
                    ((((b + 1) & 0x00000001) == 0) ? 0 : 0x000000FF) +
                    ((((b + 1) & 0x00000002) == 0) ? 0 : 0x00FF0000) +
                    ((((b + 1) & 0x00000004) == 0) ? 0 : 0x0000FF00)
                    );
                for (int i = 0; i < Palettewidth; i++)
                {
                    var color =
                        (uint)
                            ((basecolor/Palettewidth*i) + 0xFF000000);
                    _palette[i] = new Texture2D(_parent.GraphicsDevice, Blockwidth, Blockheight);
                    for (int j = 0; j < Blockwidth*Blockheight; j++)
                    {
                        palettedata[j] = color;
                    }
                    _palette[i].SetData(palettedata);
                }
                _palettes[b] = _palette;
            }
            var backgrounddata = new uint[(_parent.Height - 2)*Blockheight*_parent.Width*Blockwidth];
            const uint darkbackground = 0xFF000000;
            const uint lightbackground = 0xFF101010;
            const uint wallcolor = 0xFF303030;
            for (int k = 0; k < _parent.Width*Blockwidth*(_parent.Height - 3)*Blockheight; k++)
            {
                int row = k%(_parent.Width*Blockwidth);
                if ((row < Blockwidth) || (row >= ((_parent.Width - 1)*Blockwidth)))
                {
                    backgrounddata[k] = wallcolor;
                }
                else
                {
                    if (k%64 >= Blockwidth)
                    {
                        backgrounddata[k] = darkbackground;
                    }
                    else
                    {
                        backgrounddata[k] = lightbackground;
                    }
                }
            }
            for (int k = _parent.Width*Blockwidth*(_parent.Height - 3)*Blockheight;
                k < _parent.Width*Blockwidth*(_parent.Height - 2)*Blockheight;
                k++)
            {
                backgrounddata[k] = wallcolor;
            }
            _background = new Texture2D(_parent.GraphicsDevice, _parent.Width*Blockwidth,
                (_parent.Height - 2)*Blockheight);
            _background.SetData(backgrounddata);
            _nextFigure = new Point(_parent.Width + 2, 3);
            _fontrenderer = new FontRenderer(Path.Combine("Content", "Latin.fnt"),
                Path.Combine("Content", "Latin_0.png"), _parent.GraphicsDevice);
            _parent.GraphicsDevice.Clear(Color.Black);
        }

        private void Lock()
        {
            _spriteBatch.Begin();
        }

        private void Unlock()
        {
            _spriteBatch.End();
        }

        private void Draw(int x, int y, uint blocktype)
        {
            if (blocktype == (uint) BlockType.Empty)
            {
                _spriteBatch.Draw(_blacktexture, new Vector2(x*Blockwidth, y*Blockheight));
            }
            else
            {
                _spriteBatch.Draw(_palettes[Currentpalette][blocktype + Basepalette],
                    new Vector2(x*Blockwidth, y*Blockheight));
            }
        }

        public void DrawByIndex(Point block, uint blocktype)
        {
            _spriteBatch.Draw(_palettes[blocktype/Palettewidth][blocktype%Palettewidth],
                new Vector2(block.X*Blockwidth, block.Y*Blockheight));
        }

        private void DrawField()
        {
            Lock();
            _spriteBatch.Draw(_background, new Vector2(0, 0));
            for (int x = 1; x < (_parent.Width - 1); ++x)
            {
                for (int y = 2; y < _parent.Height - 1; ++y)
                {
                    if (_gamefield[x, y] != (uint) BlockType.Empty)
                    {
                        Draw(x, (y - 2), _gamefield[x, y]);
                    }
                }
            }
            Unlock();
        }

        private void DrawFigure(Point[] figure, uint blocktype)
        {
            Lock();
            foreach (Point block in figure)
            {
                Draw(block.X, (block.Y - 2), blocktype);
            }
            Unlock();
        }

        private void DrawNextFigure(Point[] figure, uint blocktype)
        {
            Lock();
            foreach (Point block in figure)
            {
                Draw(block.X + _nextFigure.X, block.Y + _nextFigure.Y, blocktype);
            }
            Unlock();
        }

        public void DrawAll(Point[] clear, Point[] next, Point[] figure, Score score)
        {
            if (clear != null)
            {
                DrawNextFigure(clear, (uint) BlockType.Empty);
            }
            DrawNextFigure(next, (uint) BlockType.Alive);
            DrawField();
            DrawFigure(figure, (uint) BlockType.Alive);
            DrawScore(score);
        }

        public void DrawScore(Score score)
        {
            Lock();
            for (int i = 14; i < 19; i++)
            {
                Draw(i, 7, (uint) BlockType.Empty);
            }
            Unlock();
            _fontrenderer.DrawText(_spriteBatch, (_parent.Width + 1)*Blockwidth, 7*Blockheight, score.Points + " ms",
                score.Late ? Color.Blue : Color.Red);
        }

        public void SetGameOver()
        {
            _goBlock = new Point(0, 0);
            _goBlockColor = 0;
        }

        public void DrawGameOver(int color)
        {
            _goBlockColor = (uint) color;
            for (int gocounter = 0; gocounter < GameOverSpeed; gocounter++)
            {
                _goBlock = new Point(_goBlock.X + 1, _goBlock.Y);
                if (_goBlock.X == _parent.Width)
                {
                    _goBlock = new Point(0, _goBlock.Y + 1);
                }
                if (_goBlock.Y == _parent.Height)
                {
                    _parent.GameOverLabel();
                    break;
                }
                Lock();
                DrawByIndex(_goBlock, _goBlockColor);
                Unlock();
            }
        }

        public void DrawGameOverLabel()
        {
            _fontrenderer.DrawText(_spriteBatch, (_parent.Width + 1)*Blockwidth, 10*Blockheight, "The End",
                Color.White);
        }

        public void DrawWinLabel()
        {
            _fontrenderer.DrawText(_spriteBatch, (_parent.Width + 1)*Blockwidth, 10*Blockheight, "You Won!",
                Color.White);
        }

        public int GetAllColors()
        {
            return Palettewidth*BasePalettewidth;
        }

        public void NextPalette()
        {
            Basepalette++;
            if ((Basepalette + 3) == Palettewidth)
            {
                ResetPalette();
                Currentpalette++;
                if (Currentpalette == BasePalettewidth)
                {
                    _parent.Win();
                }
                else
                {
                    _palette = _palettes[Currentpalette];
                    _parent.Speed = 3.0/(Currentpalette + 3.0);
                }
            }
        }

        public void ResetPalette()
        {
            if (_palette != null)
            {
                Basepalette = 1;
            }
        }

        public void NewGame()
        {
            Currentpalette = 0;
            Basepalette = 1;
        }

        public void ClearForNewgame()
        {
            Lock();
            for (int i = 0; i < 7; i++)
            {
                Draw(_parent.Width + 1 + i, 10, (uint) BlockType.Empty);
            }
            Unlock();
        }

        public void DrawLabels()
        {
            _fontrenderer.DrawText(_spriteBatch, (_parent.Width + 1)*Blockwidth, 0, "Next",
                Color.White);
            _fontrenderer.DrawText(_spriteBatch, (_parent.Width + 1)*Blockwidth, 1*Blockheight, "Figure",
                Color.White);
            _fontrenderer.DrawText(_spriteBatch, (_parent.Width + 1)*Blockwidth, 5*Blockheight, "Your",
                Color.White);
            _fontrenderer.DrawText(_spriteBatch, (_parent.Width + 1)*Blockwidth, 6*Blockheight, "Latency",
                Color.White);
            _fontrenderer.DrawText(_spriteBatch, (_parent.Width + 1)*Blockwidth, 8*Blockheight, "from",
                Color.White);
        }

        public void DrawLimit()
        {
            Lock();
            for (int i = 14; i < 19; i++)
            {
                Draw(i, 9, (uint) BlockType.Empty);
            }
            Unlock();
            _fontrenderer.DrawText(_spriteBatch, (_parent.Width + 1)*Blockwidth, 9*Blockheight, _limit + " ms",
                Color.LightGray);
        }

        public void SetLimit(double l)
        {
            _limit = l;
        }
    }
}