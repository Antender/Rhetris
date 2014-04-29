
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rhetris
{
    class Drawer
    {
        const int Palettewidth = 8;
        private const int Blockwidth = 32;
        private const int Blockheight = 32;
        private readonly Rhetris _parent;
        GraphicsDeviceManager graphicsManager;
        private GraphicsDevice graphics;
        readonly SpriteBatch _spriteBatch;
        readonly Texture2D[] _palette;
        private readonly uint[,] _gamefield;
        public Point nextFigure;
        public Drawer(Rhetris main, uint[,] gamefield)
        {
            _parent = main;
            _gamefield = gamefield;
            graphics = new GraphicsDevice();
            graphicsManager = new GraphicsDeviceManager(_parent)
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
            nextFigure = new Point(_parent.Width + 3,3);
            graphics.Clear(Color.Black);
        }

        public void Lock()
        {
            _spriteBatch.Begin();
        }

        public void Unlock()
        {
            _spriteBatch.End();
        }

        public void Draw(int x, int y, uint blocktype)
        {
            _spriteBatch.Draw(_palette[blocktype],new Rectangle(x*Blockwidth,y*Blockheight,Blockwidth,Blockheight),Color.White);
        }

        public void DrawField()
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

        public void DrawFigure(Point[] figure, uint blocktype)
        {
            Lock();
            foreach (var block in figure)
            {
                Draw(block.X, block.Y, blocktype);
            }
            Unlock();
        }

        public void DrawNextFigure(Point[] figure, uint blocktype)
        {
            Lock();
            foreach (var block in figure)
            {
                Draw(block.X + nextFigure.X, block.Y + nextFigure.Y, blocktype);
            }
            Unlock();
        }
    }
}
