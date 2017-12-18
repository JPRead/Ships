﻿using System;
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
        /// <summary>
        /// Sprite used to point to enemy when out of view
        /// </summary>
        private static Sprite opponentArrow;
        /// <summary>
        /// Viewport for the player
        /// </summary>
        private static PlayerView playerView;
        /// <summary>
        /// True if boarding preparations made and crew should now be fighting
        /// </summary>
        private static bool boardingInProgress;
        /// <summary>
        /// Timer for 1 second delay
        /// </summary>
        private Event tiOneSecond;
        /// <summary>
        /// Sprite used to display the player's crew numbers in boarding
        /// </summary>
        private Sprite crewDisplayPlayer;
        /// <summary>
        /// Sprite used for background if player's crew numbers in boarding
        /// </summary>
        private Sprite crewDisplayBackground;
        /// <summary>
        /// Sprite used to display boundary of world
        /// </summary>
        private Sprite worldEdgeDisplay;

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

        internal static bool BoardingInProgress
        {
            get
            {
                return boardingInProgress;
            }

            set
            {
                boardingInProgress = value;
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
            boardingInProgress = false;
            GM.eventM.AddTimer(tiOneSecond = new Event(1, "Boarding Tick"));

            windDir = GM.r.FloatBetween(0, 360);
            Sprite windDirSprite = new Sprite();
            GM.engineM.AddSprite(windDirSprite);
            windDirSprite.Frame.Define(Tex.Triangle);
            windDirSprite.SY = 1.5f;
            windDirSprite.WorldCoordinates = false;
            windDirSprite.Layer++;
            windDirSprite.Position2D = new Vector2(GM.screenSize.Center.X, 50);
            windDirSprite.RotationAngle = windDir;

            opponentArrow = new Sprite();
            GM.engineM.AddSprite(opponentArrow);
            opponentArrow.Frame.Define(Tex.Triangle);
            opponentArrow.Wash = Color.Beige;
            opponentArrow.Alpha = 0.5f;
            opponentArrow.SY = 1.5f;
            opponentArrow.Layer++;
            opponentArrow.Visible = false;

            worldEdgeDisplay = new Sprite();
            GM.engineM.AddSprite(worldEdgeDisplay);
            worldEdgeDisplay.Frame.Define(Tex.SingleWhitePixel);
            worldEdgeDisplay.Wash = Color.BlueViolet;
            worldEdgeDisplay.Alpha = 0.25f;
            worldEdgeDisplay.Align = Align.centre;
            worldEdgeDisplay.ScaleBoth = 1900f;

            Viewport viewPort = new Viewport(0, 0, 1600, 900);
            playerView = new PlayerView(viewPort, 0, 0);
            playerView.ViewerPositionManual = true;
            playerView.Clamp = false;
            GM.engineM.viewport.Clear();
            GM.engineM.viewport.Add(playerView);
            
            crewDisplayPlayer = new Sprite();
            GM.engineM.AddSprite(crewDisplayPlayer);
            crewDisplayPlayer.Frame.Define(Tex.SingleWhitePixel);
            crewDisplayPlayer.SY = 10;
            crewDisplayPlayer.SX = 50;
            crewDisplayPlayer.Wash = Color.Green;
            crewDisplayPlayer.Layer+=2;
            crewDisplayPlayer.WorldCoordinates = false;
            crewDisplayPlayer.Align = Align.bottomLeft;
            crewDisplayPlayer.Position2D = new Vector2(GM.screenSize.Center.X, GM.screenSize.Center.Y - 180);
            crewDisplayPlayer.Visible = false;

            crewDisplayBackground = new Sprite();
            GM.engineM.AddSprite(crewDisplayBackground);
            crewDisplayBackground.Frame.Define(Tex.SingleWhitePixel);
            crewDisplayBackground.SY = 10;
            crewDisplayBackground.SX = 100;
            crewDisplayBackground.Wash = Color.Red;
            crewDisplayBackground.Layer++;
            crewDisplayBackground.WorldCoordinates = false;
            crewDisplayBackground.Align = Align.bottomLeft;
            crewDisplayBackground.Position2D = new Vector2(GM.screenSize.Center.X, GM.screenSize.Center.Y - 180);
            crewDisplayBackground.Visible = false;

            player = new Player(new Vector2(0, 0));
            opponent = new Opponent(new Vector2(500, 0));
        }

        /// <summary>
        /// Check for keypresses used to end game
        /// </summary>
        public override void Tick()
        {
            //Point to opponent if off screen
            if (opponent.Position2D.X < playerView.ViewPortOutline.Left || opponent.Position2D.X > playerView.ViewPortOutline.Right || opponent.Position2D.Y < playerView.ViewPortOutline.Top || opponent.Position2D.Y > playerView.ViewPortOutline.Bottom)
            {
                Vector2 arrowDir = opponent.Position2D - player.Position2D;
                arrowDir.Normalize();

                opponentArrow.Visible = true;
                opponentArrow.Position2D = arrowDir * 200 + player.Position2D;
                opponentArrow.RotationAngle = RotationHelper.AngleFromDirection(arrowDir);
                GM.textM.Draw(FontBank.arcadePixel, 
                    Convert.ToString((int)(Vector2.Distance(opponent.Position2D, player.Position2D))) + "m", 
                    (PointHelper.Vector2FromPoint(GM.screenSize.Center) + arrowDir * 200).X - 50, 
                    (PointHelper.Vector2FromPoint(GM.screenSize.Center) + arrowDir * 200).Y - 50);
            }
            else
            {
                opponentArrow.Visible = false;
            }

            if (GM.inputM.KeyPressed(Keys.Escape))
            {
                BackToTitle("Press 1 to start.");
            }

            //Boarding
            if (boardingInProgress)
            {
                if (GM.eventM.Elapsed(tiOneSecond))
                {
                    OneSecond();
                }
                crewDisplayPlayer.Visible = true;
                crewDisplayBackground.Visible = true;
                GM.textM.Draw(FontBank.arcadePixel, "Boarding", GM.screenSize.Center.X, GM.screenSize.Center.Y - 200);
                crewDisplayPlayer.SX = ((float)player.CrewNum / (player.CrewNum + opponent.CrewNum)) * 100f;
            }
            else
            {
                crewDisplayPlayer.Visible = false;
                crewDisplayBackground.Visible = false;
            }
        }

        /// <summary>
        /// Code to run each second
        /// </summary>
        private void OneSecond()
        {
            player.CrewNum -= (int)GM.r.FloatBetween(1, (opponent.CrewNum * 0.02f) + 1.5f);
            opponent.CrewNum -= (int)GM.r.FloatBetween(1, (player.CrewNum * 0.02f) + 1.5f);
        }

        /// <summary>
        /// Resets game to title screen
        /// </summary>
        public static void BackToTitle(string stringText)
        {
            GM.ClearAllManagedObjects();
            GM.active = new TitleSetup(stringText);
        }
    }
}
