namespace AdsGame
{
    using System;

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            //new SimpleGame().Run();
            //new GameWithStates().Run();
            new TiledGame().Run();
        }
    }
}
