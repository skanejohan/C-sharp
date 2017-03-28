namespace Ads.Game.Internal
{
    class VoidMovementControl : IMovementControl
    {
        public IConditionalEvent LeftAction => voidAction;
        public IConditionalEvent RightAction => voidAction;
        public IConditionalEvent UpAction => voidAction;
        public IConditionalEvent DownAction => voidAction;
        public IConditionalEvent MouseMovementAction => voidAction;

        public VoidMovementControl()
        {
            voidAction = new ConditionalEvent(null, () => false, () => { });
        }

        private IConditionalEvent voidAction;
    }
}
