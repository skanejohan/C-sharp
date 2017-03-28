namespace Ads.Game
{
    using Microsoft.Xna.Framework.Graphics;

    public interface IVisibleEntity : IResourceEntity
    {
        void Draw(SpriteBatch spriteBatch);
    }
}
