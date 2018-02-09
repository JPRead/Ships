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
        private int grapeShotNum;
        private int carcassShotNum;
        private int barShotNum;
        private int grappleShotNum;

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

        public int GrapeShotNum
        {
            get
            {
                return grapeShotNum;
            }

            set
            {
                grapeShotNum = value;
            }
        }

        public int CarcassShotNum
        {
            get
            {
                return carcassShotNum;
            }

            set
            {
                carcassShotNum = value;
            }
        }

        public int BarShotNum
        {
            get
            {
                return barShotNum;
            }

            set
            {
                barShotNum = value;
            }
        }

        public int GrappleShotNum
        {
            get
            {
                return grappleShotNum;
            }

            set
            {
                grappleShotNum = value;
            }
        }

        public int RoundShotNum
        {
            get
            {
                return roundShotNum;
            }

            set
            {
                roundShotNum = value;
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
            new ResourceSetter(200, 200, "RoundShotNum");
            barShotNum = 0;
            new ResourceSetter(200, 300, "BarShotNum");
            carcassShotNum = 0;
            new ResourceSetter(200, 400, "CarcassShotNum");
            grapeShotNum = 0;
            new ResourceSetter(200, 500, "GrapeShotNum");
            grappleShotNum = 0;
            new ResourceSetter(200, 600, "GrappleShotNum");

            GM.engineM.WorldSize(1600, 900);
        }

        /// <summary>
        /// Check for keypresses used to end game
        /// </summary>
        public override void Tick()
        {
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
