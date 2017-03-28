namespace Ads.Game
{
    using Internal.Extensions;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;

    public class Scene : Entity, IScene, IVisibleEntity
    {
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int HorizontalOffset { get; set; }
        public int VerticalOffset { get; set; }

        private Action<IEngine,IScene> loader;
        private List<TileType> tileTypes;
        private List<Tile> tiles;
        private Rectangle availableArea;
        private int borderSize;

        internal Scene(IEngine engine, int borderSize, Action<IEngine,IScene> loader) : base(engine)
        {
            tileTypes = new List<TileType>();
            tiles = new List<Tile>();
            this.loader = loader;
            this.borderSize = borderSize;
        }

        public int AddTileType(string asset, BlockingType blockingType = BlockingType.None, 
            SpriteEffects spriteEffects = SpriteEffects.None)
        {
            tileTypes.Add(new TileType(Engine, asset, blockingType, spriteEffects));
            return tileTypes.Count - 1;
        }

        public void SetTile(int x, int y, int tileIndex)
        {
            tiles.Add(new Tile(Engine, this, x, y, tileTypes[tileIndex]));
        }

        public void Move(Direction direction, IPositionable sprite, int steps)
        {
            int spriteSteps = 0;
            switch (direction)
            {
                case Direction.Left:
                    spriteSteps = (int)Math.Min(sprite.X - availableArea.Left, steps);
                    sprite.X -= spriteSteps;
                    Scroll(Direction.Right, steps - spriteSteps);
                    break;
                case Direction.Right:
                    spriteSteps = (int)Math.Min(availableArea.Right - sprite.X, steps);
                    sprite.X += spriteSteps;
                    Scroll(Direction.Left, steps - spriteSteps);
                    break;
                case Direction.Up:
                    spriteSteps = (int)Math.Min(sprite.Y - availableArea.Top, steps);
                    sprite.Y -= spriteSteps;
                    Scroll(Direction.Down, steps - spriteSteps);
                    break;
                case Direction.Down:
                    spriteSteps = (int)Math.Min(availableArea.Bottom - sprite.Y, steps);
                    sprite.Y += spriteSteps;
                    Scroll(Direction.Up, steps - spriteSteps);
                    break;
            }
        }

        public void MoveBlocking(Direction direction, IPositionable sprite, int steps)
        {
            var coll = sprite as ICollidable;
            int allowedSteps = coll != null ? AdjustMovement(direction, coll.Rectangle(), steps) : steps;
            Move(direction, sprite, allowedSteps);
        }

        public void Scroll(Direction direction, int steps = 1)
        {
            switch (direction)
            {
                case Direction.Left:
                    HorizontalOffset -= steps;
                    break;
                case Direction.Right:
                    HorizontalOffset += steps;
                    break;
                case Direction.Up:
                    VerticalOffset -= steps;
                    break;
                case Direction.Down:
                    VerticalOffset += steps;
                    break;
            }
        }

        private int AdjustMovement(Direction direction, Rectangle originalRectangle, int steps)
        {
            var location = new Point(originalRectangle.Left, originalRectangle.Top);
            switch (direction)
            {
                case Direction.Left:
                    location.X -= steps;
                    break;
                case Direction.Right:
                    location.X += steps;
                    break;
                case Direction.Up:
                    location.Y -= steps;
                    break;
                case Direction.Down:
                    location.Y += steps;
                    break;
            }

            var movedRectangle = new Rectangle(location, originalRectangle.Size);
            foreach (var tile in tiles)
            {
                if (tile.Rectangle().Intersects(movedRectangle) &&
                    !tile.Rectangle().Intersects(originalRectangle))
                {
                    switch (direction)
                    {
                        case Direction.Left:
                            if ((tile.TileType.BlockingType & BlockingType.Right) == BlockingType.Right)
                            {
                                return originalRectangle.Left - tile.Rectangle().Right;
                            }
                            else
                            {
                                return steps;
                            }
                        case Direction.Right:
                            if ((tile.TileType.BlockingType & BlockingType.Left) == BlockingType.Left)
                            {
                                return tile.Rectangle().Left - originalRectangle.Right;
                            }
                            else
                            {
                                return steps;
                            }
                        case Direction.Up:
                            if ((tile.TileType.BlockingType & BlockingType.Bottom) == BlockingType.Bottom)
                            {
                                return originalRectangle.Top - tile.Rectangle().Bottom;
                            }
                            else
                            {
                                return steps;
                            }
                        case Direction.Down:
                            if ((tile.TileType.BlockingType & BlockingType.Top) == BlockingType.Top)
                            {
                                return tile.Rectangle().Top - originalRectangle.Bottom;
                            }
                            else
                            {
                                return steps;
                            }
                    }
                }
            }
            return steps;
        }

        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void LoadResources()
        {
            loader(Engine, this);
            availableArea.X = borderSize;
            availableArea.Y = borderSize;
            availableArea.Width = Engine.GraphicsDevice.Viewport.TitleSafeArea.Width - 2 * borderSize;
            availableArea.Height = Engine.GraphicsDevice.Viewport.TitleSafeArea.Height - 2 * borderSize;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                foreach (var tile in tiles)
                {
                    var screenX = HorizontalOffset + TileWidth * tile.X;
                    var screenY = VerticalOffset + TileHeight * tile.Y;
                    spriteBatch.Draw(tile.TileType.Texture, new Vector2(screenX, screenY),
                        null, Color.White, 0f, Vector2.Zero, 1f, tile.TileType.SpriteEffects, 0f);
                    if (Engine.DebugMode && tile.TileType.BlockingType != BlockingType.None)
                    {
                        tile.DrawRectangle(Engine.GraphicsDevice, spriteBatch);
                    }
                }
            }
        }

        internal class TileType
        {
            public Texture2D Texture { get; }
            public BlockingType BlockingType { get; }
            public SpriteEffects SpriteEffects { get; }
            public TileType(IEngine engine, string textureFile, BlockingType blockingType = BlockingType.None,
                SpriteEffects spriteEffects = SpriteEffects.None)
            {
                Texture = engine.Content.Load<Texture2D>(textureFile);
                SpriteEffects = spriteEffects;
                BlockingType = blockingType;
            }
        }

        internal class Tile : ICollidable
        {
            public TileType TileType { get; }
            public int X { get; }
            public int Y { get; }

            public int ScreenX
            {
                get
                {
                    return scene.HorizontalOffset + scene.TileWidth * X;
                }
            }
            public int ScreenY
            {
                get
                {
                    return scene.VerticalOffset + scene.TileHeight * Y;
                }
            }

            private IScene scene;

            public Tile(IEngine engine, IScene scene, int x, int y, TileType TileType)
            {
                X = x;
                Y = y;
                this.scene = scene;
                this.TileType = TileType;
            }

            public virtual Rectangle Rectangle()
            {
                return new Rectangle(ScreenX, ScreenY, scene.TileWidth, scene.TileHeight);
            }
        }

    }
}
