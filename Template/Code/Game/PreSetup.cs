using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Engine7;
using Template.Title;

namespace Template.Game
{
    internal class PreSetup : BasicSetup
    {   
        /// <summary>
        /// Viewport for the player
        /// </summary>
        private static PlayerView playerView;
        private TextButton startButton;
        private int roundShotNum;

        public static PlayerView PlayerView
        {
            get
            {
                return playerView;
            }

            set
            {
                playerView = value;
            }
        }

        /// <summary>
        /// Sets up all the initial values for the game
        /// </summary>
        public PreSetup() : base(true)
        {
            //Init values
            GM.engineM.DebugDisplay = Debug.fps | Debug.version;
            new Cursor(GM.screenSize.Center);

            Viewport viewPort = new Viewport(0, 0, 1600, 900);
            playerView = new PlayerView(viewPort, 0, 0);
            playerView.ViewerPositionManual = true;
            playerView.Clamp = false;
            GM.engineM.viewport.Clear();
            GM.engineM.viewport.Add(playerView);

            startButton = new TextButton(new Rectangle(1500, 800, 100, 100), "Start");

            roundShotNum = 0;
            new IncrementButton(new Vector2(100, 100), true, ref roundShotNum);
            
            GM.engineM.WorldSize(1600, 900);
        }

        /// <summary>
        /// Check for keypresses used to end game
        /// </summary>
        public override void Tick()
        {
            GM.textM.Draw(FontBank.arcadePixel, "Round Shot: " + roundShotNum, 150, 100);
            if (GM.inputM.KeyPressed(Keys.Escape))
            {
                BackToTitle("Press 1 to start.");
            }
            if (startButton.PressedLeft())
            {
                StartGame();
            }
        }

        /// <summary>
        /// Resets game to title screen
        /// </summary>
        public static void BackToTitle(string stringText)
        {
            GM.ClearAllManagedObjects();
            GM.active = new TitleSetup(stringText);
        }

        private static void StartGame()
        {
            //tidy up before moving to another mode
            GM.ClearAllManagedObjects();
            GM.active = new GameSetup();
        }
    }
}
