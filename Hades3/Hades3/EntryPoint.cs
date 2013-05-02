using System;

namespace Hades3
{
#if WINDOWS
    static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (SimulationCore game = new SimulationCore())
            {
                game.Run();
            }
        }
    }
#endif
}

