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

using System.Runtime.InteropServices;
using System.Reflection;
//--engine import
using Engine7;

/// <summary>
/// container for all systems code
/// </summary>
namespace Template
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class GM : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Creates reference to the graphics device
        /// </summary>
       GraphicsDeviceManager graphics;
        /// <summary>
        /// the engine manager
        /// </summary>
        public static EngineManager engineM;
        /// <summary>
        /// the tilemap manager
        /// </summary>
        public static TileMapManager tileMapM;
        /// <summary>
        /// handles menu display and activation
        /// </summary>
        public static MenuManager menuM;
        /// <summary>
        /// the event manager
        /// </summary>
        public static EventManager eventM;
        /// <summary>
        /// the text manager
        /// </summary>
        public static TextManager textM;
        /// <summary>
        /// the audio manager
        /// </summary>
        public static AudioManager audioM;
        /// <summary>
        /// the input manager
        /// </summary>
        public static InputManager inputM;
        /// <summary>
        /// handles loading assets in the background
        /// </summary>
        public static Loader loadM;


        
        /// <summary>
        /// constructs the basic game elements
        /// </summary>
        public GM()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //update 100 times a second but draw at refresh rate
            if (THROTTLE)
                eventM = new EventManager(this, UPDATE_RATE, true);
            else
                eventM = new EventManager(this, -1, false);

            //set resolution 
            graphics.PreferredBackBufferWidth = screenSize.Width;
            graphics.PreferredBackBufferHeight = screenSize.Height;

            
            //CheckSaveFile();
            //CheckAssemblyDetails();
        }

        private void CheckSaveFile()
        {
            if (fileM.SaveLocation == "ChangeMe") throw new ApplicationException("Change save file location at top of Assets.cs");
        }

        /// <summary>
        /// determines if basic template settings have been changed or not
        /// throwing exceptions if they have not
        /// </summary>
        private void CheckAssemblyDetails()
        {
            //pick up the default assembly name - the name given to the executable
            var assembly = typeof(Program).Assembly;
            string dave = assembly.GetName().Name;
            if (dave == "Worker") throw new ApplicationException("Go to Project -> Properties -> Application tab, and change the Assembly name\nThis is the name given to the executable file produced");

            //pick up the unique GUID this needs to be changed if delivering to the xbox
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            string id = attribute.Value;
            //if (id == "286979a8-1343-409f-b26f-69947dd15cce") throw new ApplicationException("Go to Tools -> Create GUID to create a unique GUID code for your app, copy this\nthen Go to Project -> Properties -> Application tab -> Assembly Information button to put your new GUID in\nThis uniquely identifies an application, critical to change for xbox applications");

            //check title in assembly information 
            var attribs = (AssemblyTitleAttribute)assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true)[0];
            string title = attribs.Title;
            if (title == "DaveBlokeSystem") throw new ApplicationException("Go to Project -> Properties -> Application tab -> Assembly Information button and set the assembly Information\nthis information is available when you look at the properties of an application");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //engine components created
            tileMapM = new TileMapManager(this);
            audioM = new AudioManager(this);
            inputM = new InputManager(this);
            //engine helpers created

            textM = new TextManager(screenSize.Width);
            menuM = new MenuManager(this, textM);
            //create engine manager
            engineM = new EngineManager(this, tileMapM, eventM, textM, /*fontB,*/ audioM);
            
            //load manager
            loadM = new Loader(this);//, textureProcessor);
            //associate components with the game
            Components.Add(eventM);
            Components.Add(menuM);
            Components.Add(tileMapM);
            Components.Add(audioM);
            Components.Add(loadM);
            Components.Add(inputM);
            Components.Add(engineM);
            //engineM.WorldSize(65536, 65536);
            //add services for storage - disabled for now until missing component fix in place
            //Components.Add(new GamerServicesComponent(this));

            //create an event to handle exit command
            this.Exiting += new EventHandler<EventArgs>(GM_Exiting);
            
            base.Initialize();
        }

        /// <summary>
        /// called when game is shutting down
        /// </summary>
        /// <param name="sender">the sender of the exit request</param>
        /// <param name="e">arguments to go along with the closing event</param>
        void GM_Exiting(object sender, EventArgs e)
        {
            ShutDown();
        }
        /// <summary>
        /// closes the system down formally
        /// </summary>
        public static void CloseSystem()
        {
            gameState = EXIT;
        }
        /// <summary>
        /// holds current state of game, starts with LOAD_UP
        /// </summary>
        private static int gameState = LOAD_UP;

        /// <summary>
        /// a random number generator
        /// </summary>
        public static RandomHelper r = new RandomHelper();
        /*************************************************
        ************          states          ***********
        *************************************************/
        private const int LOAD_UP = 1;
        private const int LOADING_ASSETS = 2;
        private const int LOADING_COMPLETE = 3;
        private const int EXIT = 4;
        //-- add any more states here
        /// <summary>
        /// holds the state the game is in
        /// </summary>
        public static StateChanger startThisState = null;
        /// <summary>
        /// perform general logic actions
        /// </summary>
        private void GameLoop()
        {
            //state specific logic
            switch (gameState)
            {
                case LOAD_UP:
                    StartPreLoader();
                    break;
                case LOADING_ASSETS:
                    LoadingLogic();
                    break;
                case EXIT:
                    Exit();
                    break;
            }
            //check to see if we need to move to another game state
            ProcessStateChange();
        }
        /// <summary>
        /// processes game state change requests
        /// </summary>
        private void ProcessStateChange()
        {
            //if a change is requested call the subroutine
            if (startThisState != null)
            {
                StateChanger s = startThisState;
                startThisState = null;
                s();
            }
        } // end ProcessStateChange


        /// <summary>
        /// performs logic while loading assets
        /// </summary>
        private void LoadingLogic()
        {
            //once all assets loaded perform setup tasks
            if (loadM.LoadComplete)
            {
                //move out of the LOADING_ASSETS state so this code only runs once
                gameState = LOADING_COMPLETE;

                //build remaining lifetime assets for game
                Setup();

                //game finished loading start here
                startThisState = StartSystem;
            }

        }// end LoadingLogic

        /// <summary>
        /// removes/disables or stops all managed assets except sprites
        /// </summary>
        public static void ClearAllButSprites()
        {
            MessageBus.Instance.Unsubscribe();
            inputM.Flush();
            menuM.HideAll();
            engineM.ClearLineList();
            engineM.ClearAllParticles();
            eventM.RemoveAll();
            audioM.RemoveAllManagedEffects();
            audioM.StopCurrentSong();
            tileMapM.Clear();
        } // ClearAllButSprites

        /// <summary>
        /// removes/disables or stops all managed assets
        /// </summary>
        public static void ClearAllManagedObjects()
        {
            MessageBus.Instance.Unsubscribe();
            inputM.Flush();
            menuM.HideAll();
            engineM.ClearSprites();
            engineM.ClearLineList();
            engineM.ClearAllParticles();
            eventM.RemoveAll();
            audioM.RemoveAllManagedEffects();
            audioM.StopCurrentSong();
            tileMapM.Clear();
        } // end ClearAllManagedObjects

        /// <summary>
        /// stops all sounds playing
        /// </summary>
        public static void ClearSounds()
        {
            audioM.RemoveAllManagedEffects();
            audioM.StopCurrentSong();
        } // end StopSounds

        /// <summary>
        /// removes/disables everything but music and sound effects
        /// </summary>
        public static void ClearAllButSound()
        {
            MessageBus.Instance.Unsubscribe();
            inputM.Flush();
            menuM.HideAll();
            engineM.ClearSprites();
            engineM.ClearLineList();
            engineM.ClearAllParticles();
            eventM.RemoveAll();
            tileMapM.Clear();
        } // end ClearAllButSound

        /// <summary>
        /// removes/disables everything but the current music playing
        /// </summary>
        public static void ClearAllButMusic()
        {
            MessageBus.Instance.Unsubscribe();
            inputM.Flush();
            menuM.HideAll();
            engineM.ClearSprites();
            engineM.ClearLineList();
            engineM.ClearAllParticles();
            eventM.RemoveAll();
            audioM.StopCurrentSong();
            tileMapM.Clear();
        } // end ClearAllButMusic
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            engineM.DebugDisplay = Debug.none;
            //Guide.ShowSignIn(1, false);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //call internal game logic control
            GameLoop();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
