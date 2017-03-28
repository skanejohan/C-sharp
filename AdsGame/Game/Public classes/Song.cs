namespace Ads.Game
{
    using Microsoft.Xna.Framework;
    using M = Microsoft.Xna.Framework.Media;

    public class Song : Entity, IResourceEntity
    {
        public Song(string resource, IEngine engine) : base(engine)
        {
            this.resource = resource;
        }

        public virtual void Initialize()
        {
        }

        public virtual void LoadResources()
        {
            song = Engine.Content.Load<M.Song>(resource);
            M.MediaPlayer.Play(song);
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public override bool Paused
        {
            get
            {
                return paused;
            }
            set
            {
                paused = value;
                if (paused)
                {
                    M.MediaPlayer.Pause();
                }
                if (!paused)
                {
                    M.MediaPlayer.Resume();
                }
            }
        }

        private string resource;
        private M.Song song;
        private bool paused;
    }
}
