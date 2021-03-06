﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine7;
using Template.Game;

namespace Template
{
    /// <summary>
    /// Ship for the player
    /// </summary>
    internal class Player : Ship
    {
        /// <summary>
        /// Player's cursor
        /// </summary>
        private Cursor cursor;
        /// <summary>
        /// Point to move towards
        /// </summary>
        private Point moveTo;
        /// <summary>
        /// Button for cutting ropes when boarding
        /// </summary>
        private Button cutRopeButton;
        /// <summary>
        /// Button for setting speed
        /// </summary>
        private Button speedButton;
        /// <summary>
        /// Button to fire and load right cannon
        /// </summary>
        private Button fireRightButton;
        /// <summary>
        /// Button to fire and load left cannon
        /// </summary>
        private Button fireLeftButton;
        /// <summary>
        /// Button to start and stop repairing
        /// </summary>
        private Button repairButton;
        /// <summary>
        /// Background element for Buttons, used to stop player giving move orders behind the Buttons
        /// </summary>
        private Button UIButtonsBackground;
        /// <summary>
        /// Background element for DamageSprites, used to stop player giving move orders behind the DamageSprites
        /// </summary>
        private Button UIDamageBackground;
        /// <summary>
        /// Array for UI background elements
        /// </summary>
        private Button[] UIBackgroundElements;
        /// <summary>
        /// Sprite used to display the range and firing angles of the cannons
        /// </summary>
        private Sprite fireZone;
        /// <summary>
        /// DamageSprite used to display the health of the front hull HitBox
        /// </summary>
        private DamageSprite damageHullFront;
        /// <summary>
        /// DamageSprite used to display the health of the back hull HitBox
        /// </summary>
        private DamageSprite damageHullBack;
        /// <summary>
        /// DamageSprite used to display the health of the left hull HitBox
        /// </summary>
        private DamageSprite damageHullLeft;
        /// <summary>
        /// DamageSprite used to display the health of the right hull HitBox
        /// </summary>
        private DamageSprite damageHullRight;
        /// <summary>
        /// DamageSprite used to display the health of the front sail HitBox
        /// </summary>
        private DamageSprite damageSailFront;
        /// <summary>
        /// DamageSprite used to display the health of the middle sail HitBox
        /// </summary>
        private DamageSprite damageSailMiddle;
        /// <summary>
        /// DamageSprite used to display the health of the back sail HitBox
        /// </summary>
        private DamageSprite damageSailBack;
        /// <summary>
        /// Sprite used to represent right cannons reloading
        /// </summary>
        private Sprite UIReloadRight;
        /// <summary>
        /// Sprite used to represent left cannons reloading
        /// </summary>
        private Sprite UIReloadLeft;
        /// <summary>
        /// This must be set to true if a button has been pressed during a tick, else button presses could be interpreted as movement orders
        /// </summary>
        private bool buttonPressed;
        /// <summary>
        /// UI element to display sinking, goes behind.
        /// </summary>
        private Sprite UISinkBarBot;
        /// <summary>
        /// UI element to display sinking, goes on top and scales as more water is taken on.
        /// </summary>
        private Sprite UISinkBarTop;
        /// <summary>
        /// Queue of points to move to.
        /// </summary>
        private Queue<Point> movePath;
        /// <summary>
        /// Queue for displaying the path
        /// </summary>
        private ArrowQueue arrowQueue;

        internal Cursor Cursor
        {
            get
            {
                return cursor;
            }

            set
            {
                cursor = value;
            }
        }

        public bool ButtonPressed
        {
            get
            {
                return buttonPressed;
            }

            set
            {
                buttonPressed = value;
            }
        }

        public bool MoveTargetReached
        {
            get
            {
                return moveTargetReached;
            }

            set
            {
                moveTargetReached = value;
            }
        }

        internal Button RepairButton
        {
            get
            {
                return repairButton;
            }

            set
            {
                repairButton = value;
            }
        }

        public Queue<Point> MovePath
        {
            get
            {
                return movePath;
            }

            set
            {
                movePath = value;
            }
        }

        internal ArrowQueue ArrowQueue
        {
            get
            {
                return arrowQueue;
            }

            set
            {
                arrowQueue = value;
            }
        }

        /// <summary>
        /// Contains functions for the player to give orders
        /// </summary>
        /// <param name="startPos">Position to spawn at</param>
        public Player(Vector2 startPos)
        {
            //Init values
            isPlayer = true;
            Position2D = startPos;
            cursor = new Cursor(GM.screenSize.Center);
            moveTo = PointHelper.PointFromVector2(Position2D);
            movePath = new Queue<Point>(100);
            arrowQueue = new ArrowQueue(Vector2.Zero, Vector2.Zero);
            arrowQueue.Reset();

            //UI setup
            cutRopeButton = new Button(new Rectangle(GM.screenSize.Center.X - 75, GM.screenSize.Bottom - 50, 50, 50), true, "Cut Ropes", new Shortcut(Keys.A));
            cutRopeButton.SetDisplay(new Rectangle(153, 200, 34, 26));

            speedButton = new Button(new Rectangle(GM.screenSize.Center.X, GM.screenSize.Bottom - 125, 50, 50), true, "Increase Speed", new Shortcut(Keys.LeftShift, Keys.W), "Decrease Speed", new Shortcut(Keys.LeftControl, Keys.W));
            speedButton.SetDisplay(new Rectangle(75, 159, 6, 40));

            RepairButton = new Button(new Rectangle(GM.screenSize.Center.X, GM.screenSize.Bottom - 50, 50, 50), true, "Start Repairing", new Shortcut(Keys.LeftShift, Keys.S), "Stop Repairing", new Shortcut(Keys.LeftControl, Keys.S));
            RepairButton.SetDisplay(new Rectangle(67, 200, 42, 38));

            fireRightButton = new Button(new Rectangle(GM.screenSize.Center.X - 75, GM.screenSize.Bottom - 125, 50, 50), true, "Fire Left Cannons", new Shortcut(Keys.LeftShift, Keys.Q), "Change Left Shot Type", new Shortcut(Keys.LeftControl, Keys.Q));
            fireRightButton.SetDisplay(new Rectangle(150, 160, 20, 20));

            fireLeftButton = new Button(new Rectangle(GM.screenSize.Center.X + 75, GM.screenSize.Bottom - 125, 50, 50), true, "Fire Right Cannons", new Shortcut(Keys.LeftShift, Keys.E), "Change Right Shot Type", new Shortcut(Keys.LeftControl, Keys.E));
            fireLeftButton.SetDisplay(new Rectangle(150, 160, 20, 20));

            fireZone = new Sprite();
            GM.engineM.AddSprite(fireZone);
            fireZone.Frame.Define(Tex.SingleWhitePixel);
            fireZone.SY = 100;
            fireZone.SX = 400;
            fireZone.Wash = Color.Beige;
            fireZone.Alpha = 0.25f;

            UIReloadRight = new Sprite();
            UIReloadRight.Frame.Define(Tex.SingleWhitePixel);
            UIReloadRight.SX = 50;
            UIReloadRight.SY = 50;
            UIReloadRight.Align = Align.bottom;
            UIReloadRight.WorldCoordinates = false;
            UIReloadRight.Position2D = new Vector2(fireLeftButton.Position2D.X, fireLeftButton.Bottom);
            UIReloadRight.Alpha = 0.5f;
            UIReloadRight.Wash = Color.Beige;
            UIReloadRight.Layer += 3;
            GM.engineM.AddSprite(UIReloadRight);

            UIReloadLeft = new Sprite();
            UIReloadLeft.Frame.Define(Tex.SingleWhitePixel);
            UIReloadLeft.SX = 50;
            UIReloadLeft.SY = 50;
            UIReloadLeft.Align = Align.bottom;
            UIReloadLeft.WorldCoordinates = false;
            UIReloadLeft.Position2D = new Vector2(fireRightButton.Position2D.X, fireRightButton.Bottom);
            UIReloadLeft.Alpha = 0.5f;
            UIReloadLeft.Wash = Color.Beige;
            UIReloadLeft.Layer += 3;
            GM.engineM.AddSprite(UIReloadLeft);

            UISinkBarBot = new Sprite();
            UISinkBarBot.Frame.Define(Tex.SingleWhitePixel);
            UISinkBarBot.SX = 5;
            UISinkBarBot.SY = 100;
            UISinkBarBot.Align = Align.bottomLeft;
            UISinkBarBot.WorldCoordinates = false;
            UISinkBarBot.Position2D = new Vector2(25, GM.screenSize.Height - 25);
            UISinkBarBot.Wash = Color.White;
            UISinkBarBot.Layer += 2;
            GM.engineM.AddSprite(UISinkBarBot);

            UISinkBarTop = new Sprite();
            UISinkBarTop.Frame.Define(Tex.SingleWhitePixel);
            UISinkBarTop.SX = 5;
            UISinkBarTop.SY = 100;
            UISinkBarTop.Align = Align.bottomLeft;
            UISinkBarTop.WorldCoordinates = false;
            UISinkBarTop.Position2D = new Vector2(25, GM.screenSize.Height - 25);
            UISinkBarTop.Wash = Color.LightSkyBlue;
            UISinkBarTop.Layer += 3;
            GM.engineM.AddSprite(UISinkBarTop);

            Sprite UISinkText = new Sprite();
            UISinkText.Frame.Define(GM.txSprite, new Rectangle(0, 199, 43, 7));
            UISinkText.Align = Align.bottomLeft;
            UISinkText.WorldCoordinates = false;
            UISinkText.Position2D = new Vector2(6, GM.screenSize.Height - 15);
            UISinkText.Layer += 3;
            GM.engineM.AddSprite(UISinkText);

            UIButtonsBackground = new Button(new Rectangle(GM.screenSize.Center.X, GM.screenSize.Bottom - 87, 250, 175), false);
            
            UIDamageBackground = new Button(new Rectangle(GM.screenSize.Left + 75, GM.screenSize.Bottom - 75, 150, 150), false);
            UIBackgroundElements = new Button[] { UIButtonsBackground, UIDamageBackground };//Background elements must be in this array

            damageHullLeft = new DamageSprite(UIDamageBackground.Position2D + new Vector2(-10, 0), new Vector2(20, 65), hitBoxHullLeft);
            damageHullLeft.WorldCoordinates = false;
            damageHullRight = new DamageSprite(UIDamageBackground.Position2D + new Vector2(10, 0), new Vector2(20, 65), hitBoxHullRight);
            damageHullRight.WorldCoordinates = false;
            damageHullFront = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, -40), new Vector2(40, 25), hitBoxHullFront);
            damageHullFront.WorldCoordinates = false;
            damageHullBack = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, 40), new Vector2(40, 25), hitBoxHullBack);
            damageHullBack.WorldCoordinates = false;
            damageSailFront = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, -25), new Vector2(60, 5), hitBoxSailFront);
            damageSailFront.WorldCoordinates = false;
            damageSailMiddle = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, -1), new Vector2(70, 5), hitBoxSailMiddle);
            damageSailMiddle.WorldCoordinates = false;
            damageSailBack = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, 30), new Vector2(65, 5), hitBoxSailBack);
            damageSailBack.WorldCoordinates = false;

            UpdateCallBack += Tick;
            FuneralCallBack += Death;
        }

        private void Death()
        {
            GameSetup.BackToTitle("You lose.");
        }

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Tick()
        {
            //DEBUG
            //GM.textM.Draw(FontBank.arcadePixel, 
            //    "Hull Front  " + hitBoxHullFront.Health + "~Hull Back   " + hitBoxHullBack.Health + "~Hull Left   " + hitBoxHullLeft.Health + "~Hull Right  " + hitBoxHullRight.Health +
            //    "~Sail Front  " + hitBoxSailFront.Health + "~Sail Middle " + hitBoxSailMiddle.Health + "~Sail Back   " + hitBoxSailBack.Health, 100, 100, TextAtt.TopLeft);
            GM.textM.Draw(FontBank.arcadePixel, "Crew: " + CrewNum + 
                "~~Ammunition:~    Round: " + roundShotNum + "~    Bar: " + barShotNum + "~    Carcass: " + carcassShotNum + "~    Grape: " + grapeShotNum + "~    Grapple: " + grappleShotNum +
                "~~Repair Materials:~    Hull: " + (int)hullRepMats + "~    Sail: " + (int)sailRepMats, 
                100, 50, TextAtt.TopLeft);
            
            //Sinking bar
            UISinkBarTop.SY = sinkAmount * 0.1f;

            //Make camera follow player
            GameSetup.PlayerView.Position = Position - new Vector3(GM.screenSize.Center.X, GM.screenSize.Center.Y - 100, 0);

            //UI updates
            if (!tiReloadRight.Paused && !IsRepairing)
            {
                UIReloadRight.Visible = true;
                float heightMul = (tiReloadRight.Interval - tiReloadRight.ElapsedSoFar) / tiReloadRight.Interval ;
                UIReloadRight.SY = 50 * heightMul;
            }
            if (!tiReloadLeft.Paused && !IsRepairing)
            {
                UIReloadLeft.Visible = true;
                float heightMul = (tiReloadLeft.Interval - tiReloadLeft.ElapsedSoFar) / tiReloadLeft.Interval ;
                UIReloadLeft.SY = 50 * heightMul;
            }

            //Mouse inputs
            //Set sail amount
            if (speedButton.PressedLeft())
            {
                if (sailAmount == 0)
                {
                    sailAmount++;
                    speedButton.SetDisplay(new Rectangle(82, 159, 12, 40));
                }
                else if(sailAmount == 1)
                {
                    sailAmount++;
                    speedButton.SetDisplay(new Rectangle(95, 159, 12, 40));
                }
            }
            else if (speedButton.PressedRight())
            {
                if (sailAmount == 1)
                {
                    sailAmount--;
                    speedButton.SetDisplay(new Rectangle(75, 159, 6, 40));
                }
                else if(sailAmount == 2)
                {
                    sailAmount--;
                    speedButton.SetDisplay(new Rectangle(82, 159, 12, 40));
                }
            }

            //Set shot type
            if (fireRightButton.PressedRight())
            {
                GM.eventM.AddTimer(tiReloadLeft = new Event(10, "Reload Cooldown Right"));
                switch (shotTypeRight)
                {
                    case 0:
                        shotTypeRight = 1;
                        fireRightButton.SetDisplay(new Rectangle(190, 164, 28, 12));
                        break;
                    case 1:
                        shotTypeRight = 2;
                        fireRightButton.SetDisplay(new Rectangle(238, 152, 20, 28));
                        break;
                    case 2:
                        shotTypeRight = 3;
                        fireRightButton.SetDisplay(new Rectangle(278, 156, 28, 28));
                        break;
                    case 3:
                        shotTypeRight = 4;
                        fireRightButton.SetDisplay(new Rectangle(326, 160, 20, 20));
                        break;
                    case 4:
                        shotTypeRight = 0;
                        fireRightButton.SetDisplay(new Rectangle(150, 160, 20, 20));
                        break;
                }
            }
            if (fireLeftButton.PressedRight())
            {
                GM.eventM.AddTimer(tiReloadRight = new Event(10, "Reload Cooldown Left"));
                switch (shotTypeLeft)
                {
                    case 0:
                        shotTypeLeft = 1;
                        fireLeftButton.SetDisplay(new Rectangle(190, 164, 28, 12));
                        break;
                    case 1:
                        shotTypeLeft = 2;
                        fireLeftButton.SetDisplay(new Rectangle(238, 152, 20, 28));
                        break;
                    case 2:
                        shotTypeLeft = 3;
                        fireLeftButton.SetDisplay(new Rectangle(278, 156, 28, 28));
                        break;
                    case 3:
                        shotTypeLeft = 4;
                        fireLeftButton.SetDisplay(new Rectangle(326, 160, 20, 20));
                        break;
                    case 4:
                        shotTypeLeft = 0;
                        fireLeftButton.SetDisplay(new Rectangle(150, 160, 20, 20));
                        break;
                }
            }

            //Fire cannons
            if (fireRightButton.PressedLeft())
            {
                Fire(true, shotTypeRight);
            }
            if (fireLeftButton.PressedLeft())
            {
                Fire(false, shotTypeLeft);
            }

            if (fireRightButton.Hover())
            {
                fireZone.Visible = true;
                fireZone.Position2D = Position2D - RotationHelper.Direction2DFromAngle(RotationAngle, 90) * 220;
                fireZone.RotationAngle = RotationAngle;
            }
            else if (fireLeftButton.Hover())
            {
                fireZone.Visible = true;
                fireZone.Position2D = Position2D + RotationHelper.Direction2DFromAngle(RotationAngle, 90) * 220;
                fireZone.RotationAngle = RotationAngle;
            }
            else
            {
                fireZone.Visible = false;
            }

            //Repair ship
            if (RepairButton.PressedLeft())
            {
                IsRepairing = true;
            }
            if (RepairButton.PressedRight())
            {
                IsRepairing = false;
                if (!leftLoaded)
                {
                    tiReloadLeft.Paused = false;
                }
                if (!rightLoaded)
                {
                    tiReloadRight.Paused = false;
                }
            }
            if (IsRepairing)
            {
                fireLeftButton.Faded = true;
                fireRightButton.Faded = true;
            }
            else
            {
                fireLeftButton.Faded = false;
                fireRightButton.Faded = false;
            }

            //Cut boarding ropes
            if (cutRopeButton.PressedLeft() && (isBoarded || isBoarding))
            {
                cutBoardingRopes();
            }
            if(!isBoarded && !isBoarding)
                cutRopeButton.Faded = true;
            else
                cutRopeButton.Faded = false;

            //Movement orders
            //Check that no clicks are made on UI background
            for (int i = 0; i < UIBackgroundElements.Length; i++)
            {
                if (UIBackgroundElements[i].PressedLeft() || UIBackgroundElements[i].PressedRight() || UIBackgroundElements[i].HeldLeft() || UIBackgroundElements[i].HeldRight()) { }
            }
            
            if (buttonPressed == false)
            {
                if (GM.inputM.MouseRightButtonPressed())
                {
                    cursor.Mode = 1;
                    movePath = new Queue<Point>(100);
                    moveTo = PointHelper.PointFromVector2(cursor.Position2D + Position2D + new Vector2(-800, -350));
                    movePath.Enqueue(moveTo);
                    moveTargetReached = false;
                    arrowQueue.Reset();
                }
                else if (GM.inputM.MouseRightButtonHeld() && 
                    PointHelper.DistanceSquared(
                        new Point((int)(GameSetup.PlayerView.ViewPortOutline.Left + GM.inputM.MouseLocation.X), (int)(GameSetup.PlayerView.ViewPortOutline.Top + GM.inputM.MouseLocation.Y)), 
                        moveTo) 
                        > 1000)
                {
                    cursor.Mode = 2;
                    Vector2 newMoveTo = cursor.Position2D + Position2D + new Vector2(-800, -350);
                    if (movePath.Count == 1)
                    {
                        arrowQueue = new ArrowQueue(PointHelper.Vector2FromPoint(moveTo), newMoveTo);
                    }
                    else
                    {
                        arrowQueue.Enqueue(newMoveTo);
                    }
                    moveTo = PointHelper.PointFromVector2(newMoveTo);
                    movePath.Enqueue(moveTo);
                }
                else
                {
                    cursor.Mode = 0;
                }
            }
            else buttonPressed = false;

            if (moveTargetReached == false)
            {
                if (movePath.Count > 0)
                {
                    MoveToPoint(movePath.Peek());
                }
                else
                {
                    moveTargetReached = true;
                }
            }
        }
    }
}