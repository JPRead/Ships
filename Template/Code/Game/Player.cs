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
        private Button userInterfaceBackground;
        private bool moveTargetReached;
        private Sprite fireZone;

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
            fireLeft = new Button(new Rectangle(GM.screenSize.Center.X + 75, GM.screenSize.Bottom - 150, 50, 50), true);
            fireZone = new Sprite();
            GM.engineM.AddSprite(fireZone);
            fireZone.Frame.Define(Tex.SingleWhitePixel);
            fireZone.SY = 100;
            fireZone.SX = 400;
            fireZone.Wash = Color.Beige;
            fireZone.Alpha = 0.5f;

            userInterfaceBackground = new Button(new Rectangle(GM.screenSize.Center.X, GM.screenSize.Bottom - 100, 250, 200), false);

            UpdateCallBack += Move;
        }

        private void Move()
        {
            //Debug text
            GM.textM.Draw(FontBank.arcadePixel, "Hull Front  " + hitBoxHullFront.Health + "~Hull Back   " + hitBoxHullBack.Health +
                "~Hull Left   " + hitBoxHullLeft.Health + "~Hull Right  " + hitBoxHullRight.Health +
                "~Sail Front  " + hitBoxSailFront.Health + "~Sail Middle " + hitBoxSailMiddle.Health + "~Sail Back   " + hitBoxSailBack.Health, 100, 100, TextAtt.TopLeft);

            //Mouse inputs
            //Sail amount
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
            if (fireRight.PressedLeft())
            {
                fire(false);
            }
            if (fireLeft.PressedLeft())
            {
                fire(true);
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
            if (userInterfaceBackground.PressedLeft() || userInterfaceBackground.PressedRight() || userInterfaceBackground.HeldLeft() || userInterfaceBackground.HeldRight()){ }
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