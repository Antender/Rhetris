using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rhetris
{
    public delegate void ReleasedEvent();
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
        public Input(int cap)
        {
            _capacity = cap;
            _buttons = new ButtonEvent[cap];
            _keys = new KeyEvent[cap];
            oldGamepadState = GamePad.GetState(PlayerIndex.One);
            oldKeyboardState = Keyboard.GetState();
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
            oldGamepadState = currentGamepadState;
            oldKeyboardState = currentKeyboardState;
        }

        public void Clear()
        {
            _freeSlot = 0;
        }
    }
}
