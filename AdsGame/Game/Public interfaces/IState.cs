namespace Ads.Game
{
    using System;

    public interface IState : IEntities
    {
        void RegisterEntry(Action action);
        void RegisterEntryFrom(int fromState, Action action);
        void RegisterExit(Action action);
        void RegisterExitTo(int toState, Action action);
    }
}
