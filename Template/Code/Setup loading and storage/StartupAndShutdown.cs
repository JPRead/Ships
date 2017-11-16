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
            //setup scoring system 
            fileM.LoadXMLFile(new ScoreSystem(), "scores.xml", LoadScore);
            
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
        /// load score table when system retrieves file
        /// </summary>
        /// <param name="r">helper with file data</param>
        private void LoadScore(ReadHelper r)
        {
            //did the file exist
            if (r.ReadPossible)
            {
                try
                {
                    scoring = (ScoreSystem)r.Deserialised;
                    return;
                }
                catch { }
            }
            //set default score table if we failed to load or couldn't load
            scoring = ScoreSystem.Default();
        }

        /// <summary>
        /// cleanly exits the game saving data to files
        /// </summary>
        private void ShutDown()
        {
            if (scoring != null)
                fileM.SaveXMLFile(scoring, "scores.xml");
        }

        /*************************************************
        ************ all code before this line ***********
        *************************************************/
    }
}