using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Rhetris.Input
{
    class TouchController : Controller
    {
        public TouchController(Game game, TimeSpan timespan) : base(game, timespan) {}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var gesture = default(GestureSample);

            while (TouchPanel.IsGestureAvailable)
                gesture = TouchPanel.ReadGesture();

            if (gesture.GestureType == GestureType.VerticalDrag)
            {
                if (gesture.Delta.Y < 0)
                    Actions[(int)ActionTypes.Drop]();

                if (gesture.Delta.Y > 0)
                    Actions[(int)ActionTypes.RotateClockwize]();
            }

            if (gesture.GestureType == GestureType.HorizontalDrag)
            {
                if (gesture.Delta.X < 0)
                    Actions[(int)ActionTypes.MoveLeft]();

                if (gesture.Delta.X > 0)
                    Actions[(int)ActionTypes.MoveRight]();
            }
        }
    }
}
