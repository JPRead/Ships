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
using Engine7;//--engine import

/// <summary>
/// container for all systems code
/// </summary>
namespace Template
{
    /// <summary>
    /// holds all game assets and code to set up those assets
    /// </summary>
    public partial class GM
    {
        /*********** all code AFTER this line ***********/
        /// <summary>
        /// organises and saves files in the given folder
        /// </summary>
        public FileManager fileM = new FileManager("template");
        /// <summary>
        /// determines if game is being throttled (DEFAULT), 
        /// this links rendering to the refresh rate of the monitor (which prevents tearing)
        /// </summary>
        public static bool THROTTLE = true;
        /// <summary>
        /// how many times a second do you want the internal clocks and logic to operate at defaults to 100
        /// </summary>
        public static int UPDATE_RATE = 60;
        /// <summary>
        /// reference to screen size - smaller is better as this will be quickler
        /// </summary>
        public static Rectangle screenSize = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        /*************************************************
        ************   STORAGE FOR GLOBALS     ***********
        *************************************************/
        /// <summary>
        /// holds currently active container
        /// </summary>
        public static BasicSetup active;

        /*************************************************
        ************   STORAGE FOR SPRITE TYPES  *********
        *************************************************/
        //public const int tPLAYER = 2000; // example
        /*************************************************
        ************   STORAGE FOR TEXTURES    ***********
        *************************************************/
        /// <summary>
        /// holds the example sprite
        /// </summary>
        public static Texture2D txSprite;

        /*************************************************
************    STORAGE FOR TRACKS     ***********
*************************************************/


        /// <summary>
        /// specifies what assets are being loaded
        /// </summary>
        public void StartPreLoader()
        {
            GM.gameState = GM.LOADING_ASSETS;
            //load image files and copy to textures for easy reference
            //tell preloader to load the following textures
            GM.loadM.AddTexture(delegate (Texture2D t, string f) { txSprite = t; }, "graphics\\Sprites");
            //fruit
            GM.loadM.AddSoundEffect("fruit", "audio\\fruit");
            //on up
            GM.loadM.AddSoundEffect("extra man", "audio\\extra man");
            //add the fire sound effect calling it shoot
            GM.loadM.AddSoundEffect("fire", "audio\\fire");
            //add the explode sound effect calling it blow up
            GM.loadM.AddSoundEffect("explode", "audio\\explode");

            //start background loader waiting 1 milliseconds between each load
            GM.loadM.Start(1);

        } // end StartPreLoader


        /// <summary>
        /// deals with creation of other assets once all textures and sounds are loaded
        /// run after all assets have been loaded
        /// </summary>
        internal void Setup()
        {
            //show mouse cursor or not - not in this case
            IsMouseVisible = false;
            //GM.engineM.ToggleFullScreen();

            //position window for game
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);
            form.Location = new System.Drawing.Point(10, 10);
            //add any other lifetime assets creation methods here
            CreateTracks();
        } // end Setup

       
        /// <summary>
        /// create any track definitions for the game
        /// </summary>
        private  void CreateTracks()
        {
            //TODO: add calls to your subroutines here that define permanent tracks
        } // end CreateTracks

        /*************************************************
        ************ all code before this line ***********
        *************************************************/
    }
}
