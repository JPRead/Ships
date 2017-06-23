using System;

/// <summary>
/// container for all systems code
/// </summary>
namespace Template
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GM game = new GM())
            {
                game.Run();
            }
        }
    }
#endif
}

