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
        /// <summary>
        /// Button used to start the game
        /// </summary>
        private TextButton startButton;
        /// <summary>
        /// Remaining money for the player
        /// </summary>
        private int playerMoney;
        /// <summary>
        /// How much round shot the player will have
        /// </summary>
        private int playerRoundShotNum;
        /// <summary>
        /// How much grape shot the player will have
        /// </summary>
        private int playerGrapeShotNum;
        /// <summary>
        /// How much carcass shot the player will have
        /// </summary>
        private int playerCarcassShotNum;
        /// <summary>
        /// How much bar shot the player will have
        /// </summary>
        private int playerBarShotNum;
        /// <summary>
        /// How much grapple shot the player will have
        /// </summary>
        private int playerGrappleShotNum;
        /// <summary>
        /// How much hull repair materials the player will have
        /// </summary>
        private int playerHullRepairMats;
        /// <summary>
        /// How much sail repair materials the player will have
        /// </summary>
        private int playerSailRepairMats;
        /// <summary>
        /// Remaining money for the opponent
        /// </summary>
        private int opponentMoney;
        /// <summary>
        /// How much round shot the opponent will have
        /// </summary>
        private int opponentRoundShotNum;
        /// <summary>
        /// How much grape shot the opponent will have
        /// </summary>
        private int opponentGrapeShotNum;
        /// <summary>
        /// How much carcass shot the opponent will have
        /// </summary>
        private int opponentCarcassShotNum;
        /// <summary>
        /// How much bar shot the opponent will have
        /// </summary>
        private int opponentBarShotNum;
        /// <summary>
        /// How much grapple shot the opponent will have
        /// </summary>
        private int opponentGrappleShotNum;
        /// <summary>
        /// How much hull repair materials the opponent will have
        /// </summary>
        private int opponentHullRepairMats;
        /// <summary>
        /// How much sail repair materials the opponent will have
        /// </summary>
        private int opponentSailRepairMats;

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

        public int PlayerSailRepairMats
        {
            get
            {
                return playerSailRepairMats;
            }

            set
            {
                playerSailRepairMats = value;
            }
        }

        public int PlayerHullRepairMats
        {
            get
            {
                return playerHullRepairMats;
            }

            set
            {
                playerHullRepairMats = value;
            }
        }

        public int OpponentRoundShotNum
        {
            get
            {
                return opponentRoundShotNum;
            }

            set
            {
                opponentRoundShotNum = value;
            }
        }

        public int OpponentGrapeShotNum
        {
            get
            {
                return opponentGrapeShotNum;
            }

            set
            {
                opponentGrapeShotNum = value;
            }
        }

        public int OpponentCarcassShotNum
        {
            get
            {
                return opponentCarcassShotNum;
            }

            set
            {
                opponentCarcassShotNum = value;
            }
        }

        public int OpponentBarShotNum
        {
            get
            {
                return opponentBarShotNum;
            }

            set
            {
                opponentBarShotNum = value;
            }
        }

        public int OpponentGrappleShotNum
        {
            get
            {
                return opponentGrappleShotNum;
            }

            set
            {
                opponentGrappleShotNum = value;
            }
        }

        public int OpponentHullRepairMats
        {
            get
            {
                return opponentHullRepairMats;
            }

            set
            {
                opponentHullRepairMats = value;
            }
        }

        public int OpponentSailRepairMats
        {
            get
            {
                return opponentSailRepairMats;
            }

            set
            {
                opponentSailRepairMats = value;
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
            playerRoundShotNum = 200;
            new ResourceSetter(300, 200, "PlayerRoundShotNum", "Round shot");
            playerBarShotNum = 100;
            new ResourceSetter(300, 300, "PlayerBarShotNum", "Bar shot");
            playerCarcassShotNum = 100;
            new ResourceSetter(300, 400, "PlayerCarcassShotNum", "Carcass shot");
            playerGrapeShotNum = 100;
            new ResourceSetter(300, 500, "PlayerGrapeShotNum", "Grape shot");
            playerGrappleShotNum = 100;
            new ResourceSetter(300, 600, "PlayerGrappleShotNum", "Grapple shot");
            playerHullRepairMats = 100;
            new ResourceSetter(300, 700, "PlayerHullRepairMats", "Hull repair materials");
            playerSailRepairMats = 50;
            new ResourceSetter(300, 800, "PlayerSailRepairMats", "Sail repair materials");

            opponentMoney = 1000;
            opponentRoundShotNum = 200;
            new ResourceSetter(GM.screenSize.Center.X + 300, 200, "OpponentRoundShotNum", "Round shot");
            opponentBarShotNum = 100;
            new ResourceSetter(GM.screenSize.Center.X + 300, 300, "OpponentBarShotNum", "Bar shot");
            opponentCarcassShotNum = 100;
            new ResourceSetter(GM.screenSize.Center.X + 300, 400, "OpponentCarcassShotNum", "Carcass shot");
            opponentGrapeShotNum = 100;
            new ResourceSetter(GM.screenSize.Center.X + 300, 500, "OpponentGrapeShotNum", "Grape shot");
            opponentGrappleShotNum = 100;
            new ResourceSetter(GM.screenSize.Center.X + 300, 600, "OpponentGrappleShotNum", "Grapple shot");
            opponentHullRepairMats = 100;
            new ResourceSetter(GM.screenSize.Center.X + 300, 700, "OpponentHullRepairMats", "Hull repair materials");
            opponentSailRepairMats = 50;
            new ResourceSetter(GM.screenSize.Center.X + 300, 800, "OpponentSailRepairMats", "Sail repair materials");

            GM.engineM.WorldSize(1600, 900);
        }

        /// <summary>
        /// Check for keypresses used to end game
        /// </summary>
        public override void Tick()
        {
            playerMoney = 1000 - playerRoundShotNum - playerBarShotNum - playerCarcassShotNum - playerGrapeShotNum - playerGrappleShotNum - playerHullRepairMats - playerSailRepairMats;
            GM.textM.Draw(FontBank.arcadePixel, "Unspent player resources: " + playerMoney, 50, 100);

            opponentMoney = 1000 - opponentRoundShotNum - opponentBarShotNum - opponentCarcassShotNum - opponentGrapeShotNum - opponentGrappleShotNum - opponentHullRepairMats - opponentSailRepairMats;
            GM.textM.Draw(FontBank.arcadePixel, "Unspent opponent resources: " + opponentMoney, GM.screenSize.Center.X + 50, 100);

            if (GM.inputM.KeyPressed(Keys.Escape))
            {
                BackToTitle("Press 1 to start.");
            }
            if (startButton.PressedLeft())
            {
                if(playerMoney >= 0 && opponentMoney >= 0)
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
            GameSetup.Player.hullRepMats = playerHullRepairMats;
            GameSetup.Player.sailRepMats = playerSailRepairMats;

            GameSetup.Opponent.roundShotNum = opponentRoundShotNum;
            GameSetup.Opponent.barShotNum = opponentBarShotNum;
            GameSetup.Opponent.grapeShotNum = opponentGrapeShotNum;
            GameSetup.Opponent.carcassShotNum = opponentCarcassShotNum;
            GameSetup.Opponent.grappleShotNum = opponentGrappleShotNum;
            GameSetup.Opponent.hullRepMats = opponentHullRepairMats;
            GameSetup.Opponent.sailRepMats = opponentSailRepairMats;
        }
    }
}
