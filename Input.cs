using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Rhetris
{
    public delegate void ReleasedEvent();

    internal enum TouchAction
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown
    }

    internal struct ButtonEvent
    {
        public Buttons Button;
        public ReleasedEvent Released;

        public ButtonEvent(Buttons b, ReleasedEvent d)
        {
            Button = b;
            Released = d;
        }
    }

    internal struct KeyEvent
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
        private const int Mousevthreshold = 30;
        private const int Mousehthreshold = 30;
        private readonly ButtonEvent[] _buttons;
        private readonly KeyEvent[] _keys;
        private readonly ReleasedEvent[] _touchevents = new ReleasedEvent[4];
        private int _freeSlot;
        private bool _mousestate;
        private int _mousex;
        private int _mousey;
        private GamePadState _oldGamepadState;
        private KeyboardState _oldKeyboardState;

        public Input(int cap)
        {
            Capacity = cap;
            _buttons = new ButtonEvent[cap];
            _keys = new KeyEvent[cap];
            _oldGamepadState = GamePad.GetState(PlayerIndex.One);
            _oldKeyboardState = Keyboard.GetState();
            for (int i = 0; i < 4; i++)
            {
                _touchevents[i] = () => { };
            }
        }

        public int Capacity { get; set; }

        public void Add(Buttons button, Keys key, ReleasedEvent callback)
        {
            _buttons[_freeSlot] = new ButtonEvent(button, callback);
            _keys[_freeSlot] = new KeyEvent(key, callback);
            _freeSlot++;
        }

        public void Update()
        {
            GamePadState currentGamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState currentKeyboardState = Keyboard.GetState();
            foreach (
                ButtonEvent button in
                    _buttons.Where(button => _oldGamepadState.IsButtonDown(button.Button))
                        .Where(button => currentGamepadState.IsButtonUp(button.Button)))
            {
                button.Released();
            }
            foreach (
                KeyEvent key in
                    _keys.Where(key => _oldKeyboardState.IsKeyDown(key.Key))
                        .Where(key => currentKeyboardState.IsKeyUp(key.Key)))
            {
                key.Released();
            }
            GestureSample gesture = default(GestureSample);

            while (TouchPanel.IsGestureAvailable)
                gesture = TouchPanel.ReadGesture();

            if (gesture.GestureType == GestureType.VerticalDrag)
            {
                if (gesture.Delta.Y < 0)
                    _touchevents[(int) TouchAction.MoveDown]();

                if (gesture.Delta.Y > 0)
                    _touchevents[(int) TouchAction.MoveUp]();
            }

            if (gesture.GestureType == GestureType.HorizontalDrag)
            {
                if (gesture.Delta.X < 0)
                    _touchevents[(int) TouchAction.MoveLeft]();

                if (gesture.Delta.X > 0)
                    _touchevents[(int) TouchAction.MoveRight]();
            }
            bool newmousestate = Mouse.GetState().LeftButton.HasFlag(ButtonState.Pressed);
            if (_mousestate && (newmousestate == false))
            {
                int dx = Mouse.GetState().X - _mousex;
                int dy = Mouse.GetState().Y - _mousey;
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    if (dx > 0)
                    {
                        if (dx > Mousehthreshold)
                        {
                            _touchevents[(int) TouchAction.MoveRight]();
                        }
                    }
                    else
                    {
                        if (Math.Abs(dx) > Mousehthreshold)
                        {
                            _touchevents[(int) TouchAction.MoveLeft]();
                        }
                    }
                }
                else
                {
                    if (dy > 0)
                    {
                        if (dy > Mousevthreshold)
                        {
                            _touchevents[(int) TouchAction.MoveDown]();
                        }
                    }
                    else
                    {
                        if (Math.Abs(dy) > Mousevthreshold)
                        {
                            _touchevents[(int) TouchAction.MoveUp]();
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
            _oldGamepadState = currentGamepadState;
            _oldKeyboardState = currentKeyboardState;
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