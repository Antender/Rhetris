using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Rhetris
{
    public delegate void ReleasedEvent();

    enum TouchAction : int
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown
    }

    struct ButtonEvent
    {
        public Buttons Button;
        public ReleasedEvent Released;
        public ButtonEvent(Buttons b, ReleasedEvent d)
        {
            Button = b;
            Released = d;
        }
    }
    struct KeyEvent
    {
        public Keys Key;
        public ReleasedEvent Released;
        public KeyEvent(Keys k, ReleasedEvent d)
        {
            Key = k;
            Released = d;
        }
    }

    public class Input
    {
        private GamePadState oldGamepadState;
        private KeyboardState oldKeyboardState;
        private int _capacity;
        private int _freeSlot;
        private readonly ButtonEvent[] _buttons;
        private readonly KeyEvent[] _keys;
        private ReleasedEvent[] _touchevents = new ReleasedEvent[4];
        private bool mousestate = false;
        private int mousex = 0;
        private int mousey = 0;
        private const int mousevthreshold = 30;
        private const int mousehthreshold = 30;
        public Input(int cap)
        {
            _capacity = cap;
            _buttons = new ButtonEvent[cap];
            _keys = new KeyEvent[cap];
            oldGamepadState = GamePad.GetState(PlayerIndex.One);
            oldKeyboardState = Keyboard.GetState();
            for (int i = 0; i < 4; i++)
            {
                _touchevents[i] = () => { };
            }
        }
        public void Add(Buttons button, Keys key, ReleasedEvent callback)
        {
            _buttons[_freeSlot] = new ButtonEvent(button, callback);
            _keys[_freeSlot] = new KeyEvent(key, callback);
            _freeSlot++;
        }
        public void Update()
        {
            var currentGamepadState = GamePad.GetState(PlayerIndex.One);
            var currentKeyboardState = Keyboard.GetState();
            foreach (var button in _buttons.Where(button => oldGamepadState.IsButtonDown(button.Button)).Where(button => currentGamepadState.IsButtonUp(button.Button)))
            {
                button.Released();
            }
            foreach (var key in _keys.Where(key => oldKeyboardState.IsKeyDown(key.Key)).Where(key => currentKeyboardState.IsKeyUp(key.Key)))
            {
                key.Released();
            }
                var gesture = default(GestureSample);

                while (TouchPanel.IsGestureAvailable)
                    gesture = TouchPanel.ReadGesture();

                if (gesture.GestureType == GestureType.VerticalDrag)
                {
                    if (gesture.Delta.Y < 0)
                        _touchevents[(int) TouchAction.MoveDown]();

                    if (gesture.Delta.Y > 0)
                        _touchevents[(int)TouchAction.MoveUp]();
                }

                if (gesture.GestureType == GestureType.HorizontalDrag)
                {
                    if (gesture.Delta.X < 0)
                        _touchevents[(int)TouchAction.MoveLeft]();

                    if (gesture.Delta.X > 0)
                        _touchevents[(int)TouchAction.MoveRight]();
                }
            bool newmousestate = Mouse.GetState().LeftButton.HasFlag(ButtonState.Pressed);
            if ((mousestate == true) && (newmousestate == false))
            {
                int dx = Mouse.GetState().X - mousex;
                int dy = Mouse.GetState().Y - mousey;
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    if (dx > 0)
                    {
                        if (dx > mousehthreshold)
                        {
                            _touchevents[(int)TouchAction.MoveRight]();
                        }
                    }
                    else
                    {
                        if (Math.Abs(dx) > mousehthreshold)
                        {
                            _touchevents[(int)TouchAction.MoveLeft]();
                        }
                       
                    }
                }
                else
                {
                    if (dy > 0)
                    {
                        if (dy > mousevthreshold)
                        {
                            _touchevents[(int)TouchAction.MoveDown]();
                        }
                    }
                    else
                    {
                        if (Math.Abs(dy) > mousevthreshold)
                        {
                            _touchevents[(int)TouchAction.MoveUp]();
                        }
                    }
                }
                mousestate = false;
            }
            else if ((mousestate == false) && (newmousestate == true))
            {
                mousestate = true;
                mousex = Mouse.GetState().X;
                mousey = Mouse.GetState().Y;
            }
            oldGamepadState = currentGamepadState;
            oldKeyboardState = currentKeyboardState;
        }

        public void Clear()
        {
            _freeSlot = 0;
        }

        public void SetTouchActions(ReleasedEvent left, ReleasedEvent right, ReleasedEvent up, ReleasedEvent down)
        {
            _touchevents[(int) TouchAction.MoveLeft] = left;
            _touchevents[(int) TouchAction.MoveRight] = right;
            _touchevents[(int) TouchAction.MoveUp] = up;
            _touchevents[(int) TouchAction.MoveDown] = down;
        }
    }
}
