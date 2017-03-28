namespace Ads.Game
{
    using System;
    using System.Diagnostics;

    public class ConditionalEvent : Entity, IConditionalEvent
    {
        public ConditionalEvent(IEngine engine, Func<bool> applies, Action perform) : base(engine)
        {
            this.applies = applies;
            this.perform = perform;
        }

        public bool Applies()
        {
            return applies();
        }

        public void Perform()
        {
            if (!Paused)
            {
                perform();
            }
            else
            {
                Debug.WriteLine("paused");
            }
        }

        private Func<bool> applies;
        private Action perform;
    }
}
