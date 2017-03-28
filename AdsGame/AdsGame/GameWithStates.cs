using Ads.Game;
using AdsGame.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace AdsGame
{
    public class GameWithStates
    {
        // States
        const int NoGame = 0;
        const int Playing = 1;
        const int Paused = 2;
        const int GameEnded = 3;

        // Entities
        Player player;
        IEntity Background1;
        IEntity Background2;
        IEntity Background3;
        IEntity scoreText;
        IEntity healthText;
        List<Enemy> enemies = new List<Enemy>();
        List<IResourceEntity> lasers = new List<IResourceEntity>();
        List<string> log = new List<string>();

        void PauseBackground()
        {
            Background1.Paused = true;
            Background2.Paused = true;
            Background3.Paused = true;
        }

        void StartBackground()
        {
            Background1.Paused = false;
            Background2.Paused = false;
            Background3.Paused = false;
        }

        void AddEnemy(IEngine engine)
        {
            Enemy enemy = engine.AddEntity(new Enemy(engine)) as Enemy;
            engine.RegisterCollisionEvent(player as ICollidable, enemy as ICollidable, (p, e) =>
            {
                player.Health -= 10;
                enemy.Alive = false;
                enemies.Remove(enemy);
                if (player.Health <= 0)
                {
                    player.Alive = false;
                }
            });
            enemies.Add(enemy);
        }

        void FireLaser(IEngine engine)
        {
            var laser = engine.AddEntity(new Laser(player.X, player.Y, engine));
            foreach (var enemy in enemies)
            {
                engine.RegisterCollisionEvent(laser as ICollidable, enemy as ICollidable, (l, e) =>
                    {
                        player.Score += 10;
                        engine.AddEntity(new Explosion((e as IPositionable).X, (e as IPositionable).Y, engine));
                        enemies.Remove(enemy);
                        enemy.Alive = false;
                    });
                lasers.Add(laser);
            }
        }

        public void Run()
        {
            using (var game = GameFactory.GetGameEngine(log.Add))
            {

                // ---------- All states ------------------------------------------------------------------------------

                Background1 = game.AddEntity(new Background(game, "Graphics/mainbackground"));
                Background2 = game.AddEntity(new ScrollingBackground(game, -1, "Graphics/bgLayer1"));
                Background3 = game.AddEntity(new ScrollingBackground(game, -2, "Graphics/bgLayer2"));
                game.RegisterDebugOutput(10, 100, "Graphics\\gameFont");
                game.RegisterKeyDownEvent(Keys.D, () => game.DebugMode = !game.DebugMode);

                // ---------- No game in progress ---------------------------------------------------------------------

                game.State(NoGame).AddCenteredText(10, "Graphics\\gameFont", () => "Press space to play!");
                game.State(NoGame).RegisterEntry(() => PauseBackground());
                game.State(NoGame).RegisterKeyDownEvent(Keys.Space, () => game.CurrentState = Playing);
                game.State(NoGame).RegisterKeyDownEvent(Keys.Escape, () => game.Exit());

                // ---------- Game in progress ------------------------------------------------------------------------

                game.State(Playing).RegisterEntry(() =>
                    {
                        scoreText = game.AddText(10, 10, "Graphics\\gameFont", Color.White, () => $"Score: {player.Score}");
                        healthText = game.AddText(10, 40, "Graphics\\gameFont", Color.White, () => $"Health: {player.Health}");
                    });
                game.State(Playing).RegisterEntryFrom(NoGame, () =>
                    {
                        StartBackground();
                        player = game.AddEntity(new Player(game)) as Player;
                    });
                game.State(Playing).RegisterTimedEvent(1500, () => AddEnemy(game));
                game.State(Playing).RegisterKeyDownEvent(Keys.Space, () => FireLaser(game));
                game.State(Playing).RegisterKeyDownEvent(Keys.P, () => { game.CurrentState = Paused; });
                game.State(Playing).RegisterKeyDownEvent(Keys.Escape, () => { game.CurrentState = GameEnded; });
                game.State(Playing).RegisterConditionalEvent(new ConditionalEvent(game, () => player.Health <= 0, () => game.CurrentState = GameEnded));
                game.State(Playing).RegisterExit(() =>
                {
                    scoreText.Alive = false;
                    healthText.Alive = false;
                });

                // ---------- Game paused -----------------------------------------------------------------------------

                game.State(Paused).AddCenteredText(10, "Graphics\\gameFont", () => "Game paused. Press P to continue!");
                game.State(Paused).RegisterEntry(() => { game.PauseAll(); });
                game.State(Paused).RegisterKeyDownEvent(Keys.P, () => { game.StartAll(); game.CurrentState = Playing; });

                // ---------- Game ended ------------------------------------------------------------------------------

                game.State(GameEnded).AddCenteredText(10, "Graphics\\gameFont", () => "GAME OVER");
                game.State(GameEnded).RegisterEntry(() =>
                    {
                        enemies.ForEach(e => e.Alive = false);
                        lasers.ForEach(l => l.Alive = false);
                        player.Alive = false;
                        enemies.Clear();
                        lasers.Clear();
                        PauseBackground();
                        game.State(GameEnded).RegisterSingleTimedEvent(3000, () => { game.CurrentState = NoGame; });
                    });
                game.State(GameEnded).RegisterKeyDownEvent(Keys.Escape, () => game.Exit());

                game.CurrentState = NoGame;
                game.Run();
            }
        }
    }
}
