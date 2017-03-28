namespace Ads.Game
{
    public interface IConditionalEvent : IEntity
    { 
        bool Applies();
        void Perform();
    }
}
