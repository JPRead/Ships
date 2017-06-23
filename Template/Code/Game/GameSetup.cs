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
    class GameSetup : BasicSetup
    {
        public GameSetup() : base(true)
        {
            GM.engineM.DebugDisplay = Debug.fps | Debug.version;
            GM.engineM.ScreenColour = Color.Black;
            Player player = new Player(new Vector2(400, 400)); 
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
