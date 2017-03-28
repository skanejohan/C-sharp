namespace Ads.Game
{
    using System;
    using Microsoft.Xna.Framework;
    using Internal;

    internal class TimedEvent : Entity
    {
        public TimedEvent(IEngine engine, int intervalMS, Action action, bool deleteAfterFirstInvocation) : base(engine)
        {
            this.deleteAfterFirstInvocation = deleteAfterFirstInvocation;
            this.interval = TimeSpan.FromMilliseconds(intervalMS);
            this.lastEvent = TimeSpan.Zero;
            this.action = action;
        }

        public void Update(GameTime gameTime)
        {
            if (this.lastEvent == TimeSpan.Zero)
            {
                this.lastEvent = gameTime.TotalGameTime;
                (Engine as ILog).Log($"Timed event: set lastEvent to {lastEvent}");
            }

            if (!Paused)
            {
                if (backFromPause)
                {
                    lastEvent = gameTime.TotalGameTime - (interval - timeLeft);
                    backFromPause = false;
                }
                timeLeft = interval - (gameTime.TotalGameTime - lastEvent);
                if (timeLeft.TotalMilliseconds <= 0)
                {
                    lastEvent = gameTime.TotalGameTime;
                    action();
                    if (deleteAfterFirstInvocation)
                    {
                        Alive = false;
                    }
                }
            }
        }

        public override bool Paused
        {
            get
            {
                return base.Paused;
            }
            set
            {
                base.Paused = value;
                backFromPause = !value && !OutOfState;
            }
        }

        public bool OutOfState
        {
            get
            {
                return outOfState;
            }
            set
            {
                outOfState = value;
                backFromPause = !value && !Paused;
            }
        }

        private bool deleteAfterFirstInvocation;
        private TimeSpan interval;
        private TimeSpan timeLeft;
        private TimeSpan lastEvent;
        private bool backFromPause;
        private bool outOfState;
        private Action action;
    }
}
