namespace Ads.Game
{
    using System;
    using Ads.Game.Internal;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;


    public interface IEngine : IDisposable, IEntities, IInputManager, ISceneProvider
    {
        ContentManager Content { get; }
        GraphicsDevice GraphicsDevice { get; }
        Color BackgroundColor { get; set; }

        Action<IEngine> OnContentLoading { get; set; }
        void SetFullScreen();
        void SetWindowed(int width, int height);

        int CurrentState { get; set; }

        bool DebugMode { get; set; }
        void RegisterDebugOutput(int left, int top, string font);

        IState State(int stateIndex);

        void SwitchAll();
        void PauseAll();
        void StartAll();

        void Exit();
        void Run();
    }
}
