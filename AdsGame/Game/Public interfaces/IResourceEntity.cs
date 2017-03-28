namespace Ads.Game
{
    using Microsoft.Xna.Framework;

    public interface IResourceEntity : IEntity
    {
        void Initialize();
        void Update(GameTime gameTime);
        void LoadResources();
    }
}
