using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Engine7;
using Template.Game;

namespace Template
{
    /// <summary>
    /// Ship for the player
    /// </summary>
    internal class Player : Ship
    {
        private Cursor cursor;
        private Point moveTo;
        private Button speedButton;
        private Button fireRight;
        private Button fireLeft;
        private Button UIButtonsBackground;
        private Button UIDamageBackground;
        private Button[] UIBackgroundElements;
        private bool moveTargetReached;
        private Sprite fireZone;
        //Use Z values for health
        private DamageSprite damageHullFront;
        private DamageSprite damageHullBack;
        private DamageSprite damageHullLeft;
        private DamageSprite damageHullRight;
        private DamageSprite damageSailFront;
        private DamageSprite damageSailMiddle;
        private DamageSprite damageSailBack;
        private Sprite UIReloadRight;
        private Sprite UIReloadLeft;

        /// <summary>
        /// This must be set to true if a button has been pressed during a tick, else button presses could be interpreted as movement orders
        /// </summary>
        private bool buttonPressed;

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

        public Player(Vector2 startPos)
        {
            isPlayer = true;
            Position2D = startPos;
            cursor = new Cursor(GM.screenSize.Center);
            moveTo = PointHelper.PointFromVector2(Position2D);

            //User interface setup
            speedButton = new Button(new Rectangle(GM.screenSize.Center.X, GM.screenSize.Bottom - 150, 50, 50), true);
            speedButton.SetDisplay(new Rectangle(75, 159, 6, 40));

            fireRight = new Button(new Rectangle(GM.screenSize.Center.X - 75, GM.screenSize.Bottom - 150, 50, 50), true);
            fireRight.SetDisplay(new Rectangle(150, 160, 20, 20));

            fireLeft = new Button(new Rectangle(GM.screenSize.Center.X + 75, GM.screenSize.Bottom - 150, 50, 50), true);
            fireLeft.SetDisplay(new Rectangle(150, 160, 20, 20));

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
            UIReloadRight.Position2D = new Vector2(fireLeft.Position2D.X, fireLeft.Bottom);
            UIReloadRight.Alpha = 0.5f;
            UIReloadRight.Wash = Color.Beige;
            UIReloadRight.Layer += 2;
            GM.engineM.AddSprite(UIReloadRight);

            UIReloadLeft = new Sprite();
            UIReloadLeft.Frame.Define(Tex.SingleWhitePixel);
            UIReloadLeft.SX = 50;
            UIReloadLeft.SY = 50;
            UIReloadLeft.Align = Align.bottom;
            UIReloadLeft.Position2D = new Vector2(fireRight.Position2D.X, fireRight.Bottom);
            UIReloadLeft.Alpha = 0.5f;
            UIReloadLeft.Wash = Color.Beige;
            UIReloadLeft.Layer += 2;
            GM.engineM.AddSprite(UIReloadLeft);

            //Set up UI background
            UIButtonsBackground = new Button(new Rectangle(GM.screenSize.Center.X, GM.screenSize.Bottom - 100, 250, 200), false);
            UIDamageBackground = new Button(new Rectangle(GM.screenSize.Left + 75, GM.screenSize.Bottom - 75, 150, 150), false);
            UIBackgroundElements = new Button[] { UIButtonsBackground, UIDamageBackground };//Background elements must be in this array

            damageHullLeft = new DamageSprite(UIDamageBackground.Position2D + new Vector2(-10, 0), new Vector2(20, 65), hitBoxHullLeft);
            damageHullRight = new DamageSprite(UIDamageBackground.Position2D + new Vector2(10, 0), new Vector2(20, 65), hitBoxHullRight);
            damageHullFront = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, -40), new Vector2(40, 25), hitBoxHullFront);
            damageHullBack = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, 40), new Vector2(40, 25), hitBoxHullBack);
            damageSailFront = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, -25), new Vector2(60, 5), hitBoxSailFront);
            damageSailMiddle = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, -1), new Vector2(70, 5), hitBoxSailMiddle);
            damageSailBack = new DamageSprite(UIDamageBackground.Position2D + new Vector2(0, 30), new Vector2(65, 5), hitBoxSailBack);

            UpdateCallBack += Move;
        }

        private void Move()
        {
            //Debug text
            //GM.textM.Draw(FontBank.arcadePixel, "Hull Front  " + hitBoxHullFront.Health + "~Hull Back   " + hitBoxHullBack.Health +
            //    "~Hull Left   " + hitBoxHullLeft.Health + "~Hull Right  " + hitBoxHullRight.Health +
            //    "~Sail Front  " + hitBoxSailFront.Health + "~Sail Middle " + hitBoxSailMiddle.Health + "~Sail Back   " + hitBoxSailBack.Health, 100, 100, TextAtt.TopLeft);

            //UI updates
            if (tiReloadRight.Paused)
            {
                UIReloadRight.Visible = false;
            }
            else
            {
                UIReloadRight.Visible = true;
                float heightMul = (tiReloadRight.Interval - tiReloadRight.ElapsedSoFar) / tiReloadRight.Interval ;
                UIReloadRight.SY = 50 * heightMul;
            }

            if (tiReloadLeft.Paused)
            {
                UIReloadLeft.Visible = false;
            }
            else
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
            if (fireRight.PressedRight())
            {
                GM.eventM.AddTimer(tiReloadLeft = new Event(5, "Reload Cooldown Right"));
                switch (shotTypeRight)
                {
                    case 0:
                        shotTypeRight = 1;
                        fireRight.SetDisplay(new Rectangle(190, 164, 28, 12));
                        break;
                    case 1:
                        shotTypeRight = 2;
                        fireRight.SetDisplay(new Rectangle(238, 152, 20, 28));
                        break;
                    case 2:
                        shotTypeRight = 3;
                        fireRight.SetDisplay(new Rectangle(278, 156, 28, 28));
                        break;
                    case 3:
                        shotTypeRight = 0;
                        fireRight.SetDisplay(new Rectangle(150, 160, 20, 20));
                        break;
                }
            }
            if (fireLeft.PressedRight())
            {
                GM.eventM.AddTimer(tiReloadRight = new Event(5, "Reload Cooldown Left"));
                switch (shotTypeLeft)
                {
                    case 0:
                        shotTypeLeft = 1;
                        fireLeft.SetDisplay(new Rectangle(190, 164, 28, 12));
                        break;
                    case 1:
                        shotTypeLeft = 2;
                        fireLeft.SetDisplay(new Rectangle(238, 152, 20, 28));
                        break;
                    case 2:
                        shotTypeLeft = 3;
                        fireLeft.SetDisplay(new Rectangle(278, 156, 28, 28));
                        break;
                    case 3:
                        shotTypeLeft = 0;
                        fireLeft.SetDisplay(new Rectangle(150, 160, 20, 20));
                        break;
                }
            }

            //Fire cannons
            if (fireRight.PressedLeft())
            {
                fire(false, shotTypeRight);
            }
            if (fireLeft.PressedLeft())
            {
                fire(true, shotTypeLeft);
            }

            if (fireRight.Hover())
            {
                fireZone.Visible = true;
                fireZone.Position2D = Position2D - RotationHelper.Direction2DFromAngle(RotationAngle, 90) * 220;
                fireZone.RotationAngle = RotationAngle;
            }
            else if (fireLeft.Hover())
            {
                fireZone.Visible = true;
                fireZone.Position2D = Position2D +RotationHelper.Direction2DFromAngle(RotationAngle, 90) * 220;
                fireZone.RotationAngle = RotationAngle;
            }
            else
            {
                fireZone.Visible = false;
            }

            //Movement orders
            //Check that no clicks are made on UI background
            for(int i = 0; i < UIBackgroundElements.Length; i++)
            {
                if (UIBackgroundElements[i].PressedLeft() || UIBackgroundElements[i].PressedRight() || UIBackgroundElements[i].HeldLeft() || UIBackgroundElements[i].HeldRight()) { }
            }
            
            if (buttonPressed == false)
            {
                if (GM.inputM.MouseRightButtonPressed())
                {
                    cursor.Mode = 1;
                    moveTo = PointHelper.PointFromVector2(cursor.Position2D);
                    moveTargetReached = false;
                }
                //Default
                else
                {
                    cursor.Mode = 0;
                }
            }
            else buttonPressed = false;

            if (moveTargetReached == false)
                moveToPoint(moveTo);
        }
    }
}