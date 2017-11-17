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

/// <summary>
/// container for all systems code
/// </summary>
namespace Template
{
    /// <summary>
    /// Contains the code that setsup and closes the system
    /// </summary>
    public partial class GM
    {
        /*************************************************
        ************ all code AFTER this line ***********
        *************************************************/
        /// <summary>
        /// starts the game off with the title screen
        /// </summary>
        private void StartSystem()
        {
            GM.engineM.ScreenColour = Color.Black;
            //start in 1 second
            GM.eventM.DelayCall(1, Start);
        }

        /// <summary>
        /// start the system now everything is loaded
        /// </summary>
        private void Start()
        {
            GM.ClearAllManagedObjects();
            active = new TitleSetup("Press 1 to start.");
        }

        /// <summary>
        /// cleanly exits the game saving data to files
        /// </summary>
        private void ShutDown()
        {
        }

        /*************************************************
        ************ all code before this line ***********
        *************************************************/
    }
}