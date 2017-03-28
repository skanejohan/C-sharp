namespace Ads.Game
{
    public interface IEntity
    {
        bool Alive { get; set; }
        bool Paused { get; set; }
    }
}
