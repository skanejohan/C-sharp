using System;
using Ads.Game;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WHICAN
{
    public class Whican
    {
        public void Run()
        {
            using (var game = GameFactory.GetGameEngine(Console.WriteLine))
            {
                var scene = game.AddScene(100, loadSceneAssets);

                game.RegisterConditionalEvent(new ConditionalEvent(game, () => game.Left(), () => scene.Scroll(Direction.Left, 8)));
                game.RegisterConditionalEvent(new ConditionalEvent(game, () => game.Right(), () => scene.Scroll(Direction.Right, 8)));
                game.RegisterConditionalEvent(new ConditionalEvent(game, () => game.Up(), () => scene.Scroll(Direction.Up, 8)));
                game.RegisterConditionalEvent(new ConditionalEvent(game, () => game.Down(), () => scene.Scroll(Direction.Down, 8)));

                game.RegisterKeyDownEvent(Keys.Escape, () => game.Exit());
                game.OnContentLoading = engine => engine.SetFullScreen();

                game.BackgroundColor = Color.Green;
                game.Run();
            }
        }

        private void loadSceneAssets(IEngine engine, IScene scene)
        {
            scene.TileWidth = 128;
            scene.TileHeight = 128;

            var memorySlot1 = scene.AddTileType(@"Graphics\MemorySlot1", BlockingType.All);
            var memorySlot2 = scene.AddTileType(@"Graphics\MemorySlot2", BlockingType.All);
            var memorySlot3 = scene.AddTileType(@"Graphics\MemorySlot3", BlockingType.All);
            var memorySlot4 = scene.AddTileType(@"Graphics\MemorySlot4", BlockingType.All);
            var memorySlot5 = scene.AddTileType(@"Graphics\MemorySlot4", BlockingType.All, SpriteEffects.FlipHorizontally);
            var memorySlot6 = scene.AddTileType(@"Graphics\MemorySlot3", BlockingType.All, SpriteEffects.FlipHorizontally);

            scene.SetTile(0, 2, memorySlot4);
            scene.SetTile(1, 2, memorySlot3);
            scene.SetTile(2, 2, memorySlot2);
            scene.SetTile(3, 2, memorySlot2);
            scene.SetTile(4, 2, memorySlot2);
            scene.SetTile(5, 2, memorySlot2);
            scene.SetTile(6, 2, memorySlot2);
            scene.SetTile(7, 2, memorySlot1);
            scene.SetTile(8, 2, memorySlot2);
            scene.SetTile(9, 2, memorySlot2);
            scene.SetTile(10, 2, memorySlot2);
            scene.SetTile(11, 2, memorySlot2);
            scene.SetTile(12, 2, memorySlot2);
            scene.SetTile(13, 2, memorySlot6);
            scene.SetTile(14, 2, memorySlot5);
        }
    }
}
