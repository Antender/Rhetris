using System;
using Microsoft.Xna.Framework;
using XNAExtensions;

namespace Rhetris.Input
{
    public delegate void Action();

    public enum ActionTypes
    {
        MoveLeft, MoveRight, Drop,
        RotateClockwize, RotateCounterclockwize,
        Swap, Menu
    }

    public class Controller : PeriodicGameComponent                                                                                                                                                                                                                                                                                               
    {
        public const int ActionCapacity = 7;

        public static Action[] Actions
        {
            get { return _actions; }
            set
            {
                if (value.Length != ActionCapacity)
                {
                    throw new Exception("incompatible action array length");
                }
                _actions = value;
            }
        }

        private static Action[] _actions;
        public Controller(Game game, TimeSpan timespan) : base(game, timespan) {}
    }
}
