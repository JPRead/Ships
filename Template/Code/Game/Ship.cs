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
        internal bool isPlayer;
        internal Sprite moveLocSprite;

        public Ship()
        {
            sailAmount = 0;
            GM.engineM.AddSprite(this);
            GM.eventM.DelayCall(0.5f, setup);
            UpdateCallBack += Move;

            moveLocSprite = new Sprite();
            GM.engineM.AddSprite(moveLocSprite);
            moveLocSprite.Frame.Define(Tex.Circle8by8);

            Friction = 0.25f;
        }

        private void Move()
        {

        }

        private void setup()
        {
            Visible = true;
            Frame.Define(Tex.SingleWhitePixel);
            SY = 100;
            SX = 40;
        }

        /// <summary>
        /// Fire cannons
        /// </summary>
        /// <param name="left">If true fire from port side else starboard side</param>
        internal void fire(bool left)
        {
            Vector3 fireDir;
            float leftMul;
            if (left)
                leftMul = 1;
            else
                leftMul = -1;

            fireDir = Position + RotationHelper.MyDirection(this, leftMul * 90);

            Vector3 offsetAlongDeck = Vector3.Zero;

            offsetAlongDeck -= RotationHelper.MyDirection(this, 0) * 5;
            for (int i = 0; i <= 20; i++)
            {

                new CannonBall(this, 
                    new Vector2(Position2D.X - (Width) * RotationHelper.MyDirection(this, 0).X + offsetAlongDeck.X, Position2D.Y - (Width) * RotationHelper.MyDirection(this, 0).Y + offsetAlongDeck.Y), 
                    new Vector2(fireDir.X - (Width) * RotationHelper.MyDirection(this, 0).X + offsetAlongDeck.X, fireDir.Y - (Width) * RotationHelper.MyDirection(this, 0).Y + offsetAlongDeck.Y), 
                    1, 0);

                offsetAlongDeck += RotationHelper.MyDirection(this, 0) * 5;
            }
        }

        internal void moveToPoint(Point point)
        {
            Vector2 movePos = PointHelper.Vector2FromPoint(point);

            if(Vector2.DistanceSquared(Position2D, movePos) + 5000 > Height * Height)
            {
                if (isPlayer)
                    moveLocSprite.Visible = true;
                    moveLocSprite.Position2D = movePos;

                int dirMul = (int)RotationHelper.AngularDirectionTo(this, new Vector3(movePos, 0), 0, false);
                RotationVelocity = 10 * dirMul;

                if (sailAmount == 0)
                {
                }
                else
                {
                    Vector3 currentVel = Velocity;
                    currentVel.Normalize();
                    currentVel = Position + currentVel;
                    float velOffsetAngle = RotationHelper.AngularDirectionTo(this, currentVel, 0, false);

                    //Calculations for wind speed multiplier
                    float velFromWindAngle = (RotationAngle - GameSetup.WindDir) % 360;
                    if(velFromWindAngle < 0) //Absolute value
                        velFromWindAngle = -velFromWindAngle;
                    if(velFromWindAngle > 180) //Keep between 0 and 180
                    {
                        velFromWindAngle = 360 - velFromWindAngle;
                    }//Create multiplier that is <1
                    velFromWindAngle = (1/(velFromWindAngle+100)*100);

                    GM.textM.Draw(FontBank.arcadePixel, Convert.ToString(velOffsetAngle), 100, 100);

                    //Keep travelling forward
                    if (velOffsetAngle > 0)
                    {
                        Velocity += RotationHelper.MyDirection(this, -90) * 0.5f;
                    }
                    else
                    {
                        Velocity += RotationHelper.MyDirection(this, 90) * 0.5f;
                    }

                    //Add velocity
                    if (sailAmount == 1)
                    {
                        Velocity += RotationHelper.MyDirection(this, 0) * 0.1f * velFromWindAngle;
                    }
                    if (sailAmount == 2)
                    {
                        Velocity += RotationHelper.MyDirection(this, 0) * 0.2f * velFromWindAngle;
                    }
                }
            }
            else
            {
                GameSetup.Player.MoveTargetReached = true;
                moveLocSprite.Visible = false;
                RotationVelocity = 0;
            }
        }
    }
}