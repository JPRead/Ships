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
//--engine import
using Engine7;
using Template.Title;

namespace Template.Game
{
    internal class GameSetup : BasicSetup
    {
        /// <summary>
        /// The player's ship
        /// </summary>
        private static Player player;
        /// <summary>
        /// The AI's ship
        /// </summary>
        private static Opponent opponent;
        /// <summary>
        /// The wind direction
        /// </summary>
        private static float windDir;

        private static PlayerView playerView;

        internal static Player Player
        {
            get
            {
                return player;
            }
        }

        internal static Opponent Opponent
        {
            get
            {
                return opponent;
            }
        }

        public static float WindDir
        {
            get
            {
                return windDir;
            }
        }

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
        public GameSetup() : base(true)
        {
            //Init values
            GM.engineM.DebugDisplay = Debug.fps | Debug.version;
            GM.engineM.ScreenColour = Color.LightSkyBlue;

            windDir = GM.r.FloatBetween(0, 360);
            Sprite windDirSprite = new Sprite();
            GM.engineM.AddSprite(windDirSprite);
            windDirSprite.Frame.Define(Tex.Triangle);
            windDirSprite.SY = 1.5f;
            windDirSprite.WorldCoordinates = false;
            windDirSprite.Layer++;
            windDirSprite.Position2D = new Vector2(GM.screenSize.Center.X, 50);
            windDirSprite.RotationAngle = windDir;

            Viewport viewPort = new Viewport(0, 0, 1600, 900);
            playerView = new PlayerView(viewPort, 0, 0);
            playerView.ViewerPositionManual = true;
            playerView.Clamp = false;
            GM.engineM.viewport.Clear();
            GM.engineM.viewport.Add(playerView);

            player = new Player(new Vector2(400, 400));
            opponent = new Opponent(new Vector2(1600 - 400, 900 - 400));


        }

        /// <summary>
        /// Check for keypresses used to end game
        /// </summary>
        public override void Logic()
        {
            //display code
            //GM.textM.Draw(FontBank.gradius, "HI SCORE~" + GM.scoring.TopScore, GM.screenSize.Center.X, 30, TextAtt.Top);

            if (GM.inputM.KeyPressed(Keys.Escape))
            {
                BackToTitle();
            }
        }

        /// <summary>
        /// Resets game to title screen
        /// </summary>
        private static void BackToTitle()
        {
            GM.ClearAllManagedObjects();
            GM.active = new TitleSetup();
        }
    }
}
