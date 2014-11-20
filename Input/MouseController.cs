using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rhetris.Input
{
    class MouseController : Controller
    {
        private const int Mousevthreshold = 30;
        private const int Mousehthreshold = 30;
        private bool _mousestate;
        private int _mousex;
        private int _mousey;

        public MouseController(Game game, TimeSpan timespan) : base(game, timespan) {}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var newmousestate = Mouse.GetState().LeftButton.HasFlag(ButtonState.Pressed);
            if (_mousestate && (newmousestate == false))
            {
                var dx = Mouse.GetState().X - _mousex;
                var dy = Mouse.GetState().Y - _mousey;
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    if (dx > 0)
                    {
                        if (dx > Mousehthreshold)
                        {
                            Actions[(int)ActionTypes.MoveRight]();
                        }
                    }
                    else
                    {
                        if (Math.Abs(dx) > Mousehthreshold)
                        {
                            Actions[(int)ActionTypes.MoveLeft]();
                        }
                    }
                }
                else
                {
                    if (dy > 0)
                    {
                        if (dy > Mousevthreshold)
                        {
                            Actions[(int)ActionTypes.Drop]();
                        }
                    }
                    else
                    {
                        if (Math.Abs(dy) > Mousevthreshold)
                        {
                            Actions[(int)ActionTypes.RotateClockwize]();
                        }
                    }
                }
                _mousestate = false;
            }
            else if ((_mousestate == false) && newmousestate)
            {
                _mousestate = true;
                _mousex = Mouse.GetState().X;
                _mousey = Mouse.GetState().Y;
            }
        }
    }
}
