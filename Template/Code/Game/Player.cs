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
        private Button fireLeftButton;
        private Button fireRightButton;
        private Button userInterfaceBackground;
        private bool moveTargetReached;
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
            UpdateCallBack += Move;
            cursor = new Cursor(GM.screenSize.Center);
            moveTo = PointHelper.PointFromVector2(Position2D);

            //User interface setup
            speedButton = new Button(new Rectangle(GM.screenSize.Center.X, GM.screenSize.Bottom - 150, 50, 50), true);
            speedButton.SetDisplay(new Rectangle(75, 159, 6, 40));

            fireLeftButton = new Button(new Rectangle(GM.screenSize.Center.X - 75, GM.screenSize.Bottom - 150, 50, 50), true);
            fireRightButton = new Button(new Rectangle(GM.screenSize.Center.X + 75, GM.screenSize.Bottom - 150, 50, 50), true);

            userInterfaceBackground = new Button(new Rectangle(GM.screenSize.Center.X, GM.screenSize.Bottom - 100, 250, 200), false);
        }

        private void Move()
        {
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