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
    /// Ship used as a base for the player and AI
    /// </summary>
    internal class Ship : Sprite
    {
        /// <summary>
        /// 0 stopped, 1 half speed, 2 full speed
        /// </summary>
        internal int sailAmount;

        public Ship()
        {
            sailAmount = 0;
            GM.engineM.AddSprite(this);
            GM.eventM.DelayCall(0.5f, setup);
            UpdateCallBack += Move;
        }

        private void Move()
        {
            if (sailAmount == 0)
            {
                Velocity = Vector3.Zero;
            }
            else if (sailAmount == 1)
            {
                RotationHelper.VelocityInCurrentDirection(this, 25, 0);
            }
            else
            {
                RotationHelper.VelocityInCurrentDirection(this, 50, 0);
            }
        }

        private void setup()
        {
            Visible = true;
            Frame.Define(Tex.SingleWhitePixel);
            SY = 100;
            SX = 40;
        }

        internal void moveToPoint(Point point)
        {
            Vector2 movePos = PointHelper.Vector2FromPoint(point);

            if(Vector2.DistanceSquared(Position2D, movePos) > Height * Height + 10)
            {
                int dirMul = (int)RotationHelper.AngularDirectionTo(this, new Vector3(movePos, 0), 2, false);
                this.RotationVelocity = 10 * dirMul;
            }
        }
    }
}