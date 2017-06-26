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

        public Player(Vector2 startPos)
        {
            Position2D = startPos;
            UpdateCallBack += Move;
            cursor = new Cursor(GM.screenSize.Center);
            moveTo = PointHelper.PointFromVector2(Position2D);
        }

        private void Move()
        {
            //Mouse inputs
            //Movement
            if (GM.inputM.MouseRightButtonHeld())
            {
                cursor.Mode = 1;
                moveTo = PointHelper.PointFromVector2(cursor.Position2D);
            }
            //Default
            else
            {
                cursor.Mode = 0;
            }

            //Sail amount
            if (GM.inputM.KeyPressed(Keys.Down))
            {
                if (sailAmount > 0) sailAmount--;
            }
            if (GM.inputM.KeyPressed(Keys.Up))
            {
                if (sailAmount < 2) sailAmount++;
            }

            moveToPoint(moveTo);
        }
    }
}