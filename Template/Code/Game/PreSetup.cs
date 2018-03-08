using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private int playerRoundShotNum;
        private int playerGrapeShotNum;
        private int playerCarcassShotNum;
        private int playerBarShotNum;
        private int playerGrappleShotNum;
        private int playerMoney;

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

        public int PlayerRoundShotNum
        {
            get
            {
                return playerRoundShotNum;
            }

            set
            {
                playerRoundShotNum = value;
            }
        }

        public int PlayerGrapeShotNum
        {
            get
            {
                return playerGrapeShotNum;
            }

            set
            {
                playerGrapeShotNum = value;
            }
        }

        public int PlayerCarcassShotNum
        {
            get
            {
                return playerCarcassShotNum;
            }

            set
            {
                playerCarcassShotNum = value;
            }
        }

        public int PlayerBarShotNum
        {
            get
            {
                return playerBarShotNum;
            }

            set
            {
                playerBarShotNum = value;
            }
        }

        public int PlayerGrappleShotNum
        {
            get
            {
                return playerGrappleShotNum;
            }

            set
            {
                playerGrappleShotNum = value;
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

            playerMoney = 1000;
            playerRoundShotNum = 100;
            new ResourceSetter(200, 200, "PlayerRoundShotNum");
            playerBarShotNum = 100;
            new ResourceSetter(200, 300, "PlayerBarShotNum");
            playerCarcassShotNum = 100;
            new ResourceSetter(200, 400, "PlayerCarcassShotNum");
            playerGrapeShotNum = 100;
            new ResourceSetter(200, 500, "PlayerGrapeShotNum");
            playerGrappleShotNum = 100;
            new ResourceSetter(200, 600, "PlayerGrappleShotNum");

            GM.engineM.WorldSize(1600, 900);
        }

        /// <summary>
        /// Check for keypresses used to end game
        /// </summary>
        public override void Tick()
        {
            playerMoney = 1000 - playerRoundShotNum - playerBarShotNum - playerCarcassShotNum - playerGrapeShotNum - playerGrapeShotNum;
            GM.textM.Draw(FontBank.arcadePixel, "Unspent resources: " + playerMoney, 200, 100);

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

        private void StartGame()
        {
            //tidy up before moving to another mode
            GM.ClearAllManagedObjects();
            GM.active = new GameSetup();
            GameSetup.Player.roundShotNum = playerRoundShotNum;
            GameSetup.Player.barShotNum = playerBarShotNum;
            GameSetup.Player.grapeShotNum = playerGrapeShotNum;
            GameSetup.Player.carcassShotNum = playerCarcassShotNum;
            GameSetup.Player.grappleShotNum = playerGrappleShotNum;
        }
    }
}
