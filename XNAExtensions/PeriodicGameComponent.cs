using System;
using Microsoft.Xna.Framework;

namespace XNAExtensions
{
    public interface IPeriodicUpdateable
    {
        TimeSpan TimeDifference { get;}
        void PeriodicUpdate(TimeSpan updateInterval);
        void Synchronize(IPeriodicUpdateable updateable);
    }

    public class PeriodicGameComponent : GameComponent, IPeriodicUpdateable
    {
        public TimeSpan TimeDifference {
            get { return _timeDifference; } 
        }
        private TimeSpan _timeDifference = new TimeSpan(0);

        public TimeSpan MaxDifference
        {
            get { return _maxDifference; }
            set
            {
                if (value < Game.TargetElapsedTime)
                {
                    throw new ArgumentException("Insufficient Game.TargetElapsedTime precision");
                }
                _maxDifference = value;
            }
        }
        private TimeSpan _maxDifference;
        public PeriodicGameComponent(Game game, TimeSpan timeSpan) : base(game)
        {
            MaxDifference = timeSpan;
        }

        public override void Update(GameTime gameTime)
        {
            _timeDifference += gameTime.ElapsedGameTime;
            if (TimeDifference > MaxDifference)
            {
                PeriodicUpdate(TimeDifference);
                _timeDifference = new TimeSpan(0);
            }
            base.Update(gameTime);
        }

        public virtual void PeriodicUpdate(TimeSpan updateInterval) {}

        public void Synchronize(IPeriodicUpdateable updateable)
        {
            _timeDifference = updateable.TimeDifference;
        }
    }

    public class PeriodicDrawableGameComponent : DrawableGameComponent, IPeriodicUpdateable
    {
        public TimeSpan TimeDifference {
            get { return _timeDifference; } 
        }
        private TimeSpan _timeDifference = new TimeSpan(0);

        public TimeSpan MaxDifference
        {
            get { return _maxDifference; }
            set
            {
                if (value < Game.TargetElapsedTime)
                {
                    throw new ArgumentException("Insufficient Game.TargetElapsedTime precision");
                }
                _maxDifference = value;
            }
        }
        private TimeSpan _maxDifference;
        public PeriodicDrawableGameComponent(Game game, TimeSpan timeSpan) : base(game)
        {
            MaxDifference = timeSpan;
        }

        public sealed override void Update(GameTime gameTime)
        {
            _timeDifference += gameTime.ElapsedGameTime;
            if (TimeDifference > MaxDifference)
            {
                PeriodicUpdate(TimeDifference);
                _timeDifference = new TimeSpan(0);
            }
            base.Update(gameTime);
        }

        public virtual void PeriodicUpdate(TimeSpan updateInterval) { }

        public void Synchronize(IPeriodicUpdateable updateable)
        {
            _timeDifference = updateable.TimeDifference;
        }
    }
}
