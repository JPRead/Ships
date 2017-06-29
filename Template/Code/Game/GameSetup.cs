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
        private static Player player;
        private static Opponent opponent;
        private static float windDir;

        internal static Player Player
        {
            get
            {
                return player;
            }

            set
            {
                player = value;
            }
        }

        internal static Opponent Opponent
        {
            get
            {
                return opponent;
            }

            set
            {
                opponent = value;
            }
        }

        public static float WindDir
        {
            get
            {
                return windDir;
            }

            set
            {
                windDir = value;
            }
        }

        public GameSetup() : base(true)
        {
            GM.engineM.DebugDisplay = Debug.fps | Debug.version;
            GM.engineM.ScreenColour = Color.Black;

            windDir = GM.r.FloatBetween(0, 360);
            Sprite windDirSprite = new Sprite();
            GM.engineM.AddSprite(windDirSprite);
            windDirSprite.Frame.Define(Tex.Triangle);
            windDirSprite.SY = 1.5;
            windDirSprite.Position2D = new Vector2(GM.screenSize.Center.X, 50);
            windDirSprite.RotationAngle = windDir;

            player = new Player(new Vector2(400, 400));
            opponent = new Opponent(new Vector2(GM.screenSize.X - 400, GM.screenSize.Y - 400));
        }

        public override void Logic()
        {
            //display code
            //GM.textM.Draw(FontBank.gradius, "HI SCORE~" + GM.scoring.TopScore, GM.screenSize.Center.X, 30, TextAtt.Top);

            if (GM.inputM.KeyPressed(Keys.Escape))
            {
                BackToTitle();
            }
        }


        private static void BackToTitle()
        {
            GM.ClearAllManagedObjects();
            GM.active = new TitleSetup();
        }
    }
}
