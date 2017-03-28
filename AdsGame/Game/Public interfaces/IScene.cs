namespace Ads.Game
{
    using Microsoft.Xna.Framework.Graphics;

    public enum Direction
    {
        Right,
        Left,
        Up,
        Down
    }

    public enum BlockingType
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        All = 15
    }


    public interface IScene : IEntity, IResourceEntity
    {
        int TileWidth { get; set; }
        int TileHeight { get; set; }
        int HorizontalOffset { get; set; }
        int VerticalOffset { get; set; }

        int AddTileType(string asset, BlockingType blockingType = BlockingType.None,
            SpriteEffects spriteEffects = SpriteEffects.None);
        void SetTile(int x, int y, int tileIndex);

        void Scroll(Direction direction, int steps = 1);

        void Move(Direction direction, IPositionable sprite, int steps);
        void MoveBlocking(Direction direction, IPositionable sprite, int steps);
    }
}
