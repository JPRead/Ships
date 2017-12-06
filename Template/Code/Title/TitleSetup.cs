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
using Template.Game;
/*
step 1 add title graphic
step 2 add press start graphic
step 3 add showscores
step 4 add start logic and method
*/
namespace Template.Title
{
    /// <summary>
    /// generates graphs for display
    /// </summary>
    public class TitleSetup : BasicSetup
    {
        string text;
        /// <summary>
        /// constructor
        /// </summary>
        public TitleSetup(string screenText) : base(true)
        {
            text = screenText;
            GM.engineM.DebugDisplay = Debug.version;
            GM.engineM.ScreenColour = Color.Gray;
        }

        /// <summary>
        /// Display title screen
        /// </summary>
        public override void Tick()
        {
            GM.textM.Draw(FontBank.arcadeLarge, text, GM.screenSize.Center.X, GM.screenSize.Center.Y, TextAtt.Centred);

            if (GM.inputM.KeyPressed(Keys.D1))
            {
                StartGame();
            }
            if (GM.inputM.KeyPressed(Keys.Escape))
            {
                GM.ClearAllManagedObjects();
                GM.CloseSystem();
            }
        }

        private static void StartGame()
        {
            //tidy up before moving to another mode
            GM.ClearAllManagedObjects();
            GM.active = new GameSetup();
        }
    }
}
