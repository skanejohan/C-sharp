namespace Ads.Game
{
    public interface IMovementControl
    {
        IConditionalEvent LeftAction { get; }
        IConditionalEvent RightAction { get;  }
        IConditionalEvent UpAction { get; }
        IConditionalEvent DownAction { get; }
        IConditionalEvent MouseMovementAction { get; }
    }
}
