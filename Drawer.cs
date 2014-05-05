using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rhetris
{
    class Drawer
    {
        private const int Palettewidth = 8;
        private const int Blockwidth = 32;
        private const int Blockheight = 32;
        private readonly Rhetris _parent;
        private GraphicsDeviceManager _graphicsManager;
        private GraphicsDevice graphics;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D[] _palette;
        private readonly uint[,] _gamefield;
        private Point _nextFigure;
        public Drawer(Rhetris main, uint[,] gamefield)
        {
            _parent = main;
            _gamefield = gamefield;
            graphics = new GraphicsDevice();
            _graphicsManager = new GraphicsDeviceManager(_parent)
            {
                PreferredBackBufferWidth = Blockwidth*(_parent.Width + 6),
                PreferredBackBufferHeight = Blockheight*_parent.Height,
            };
            _spriteBatch = new SpriteBatch(_parent.GraphicsDevice);

            _palette = new Texture2D[Palettewidth];
            var pixel = new uint[1024];
            for (var i = 0; i < Palettewidth; i++)
            {
                var color = (uint) (i * (256 / Palettewidth) * 0x010000 + i * (256 / Palettewidth) * 0x0100 + i * (256 / Palettewidth) * 0x01 + 0xFF000000);
                _palette[i] = new Texture2D(graphics, Blockwidth, Blockheight);
                for (var j = 0; j < Blockwidth * Blockheight; j++)
                {
                    pixel[j] = color;
                }
                _palette[i].SetData(pixel, 0, 1024);
            }
            _nextFigure = new Point(_parent.Width + 3,3);
            graphics.Clear(Color.Black);
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
            _spriteBatch.Draw(_palette[blocktype],new Rectangle(x*Blockwidth,y*Blockheight,Blockwidth,Blockheight),Color.White);
        }

        private void DrawField()
        {
            Lock();
            for (var x = 0; x < _parent.Width; ++x)
            {
                for (var y = 0; y < _parent.Height; ++y)
                {
                    Draw(x, y, _gamefield[x, y]);
                }
            }
            Unlock();
        }

        private void DrawFigure(Point[] figure, uint blocktype)
        {
            Lock();
            foreach (var block in figure)
            {
                Draw(block.X, block.Y, blocktype);
            }
            Unlock();
        }

        private void DrawNextFigure(Point[] figure, uint blocktype)
        {
            Lock();
            foreach (var block in figure)
            {
                Draw(block.X + _nextFigure.X, block.Y + _nextFigure.Y, blocktype);
            }
            Unlock();
        }

        public void DrawAll(Point[] clear, Point[] next, Point[] figure)
        {
            if (clear != null)
            {
                DrawNextFigure(clear, (uint)BlockType.Empty);
            }
            DrawNextFigure(next, (uint)BlockType.Alive);
            DrawField();
            DrawFigure(figure, (uint)BlockType.Alive);
        }
    }
}
