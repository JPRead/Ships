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
                    //Velocity = Vector3.Zero;
                }
                else
                {
                    Vector3 currentVel = Velocity;
                    currentVel.Normalize();
                    float velAngle = RotationHelper.AngleFromDirection(currentVel);
                    //Negative for travelling left of current direction
                    float velOffsetAngle = 0;
                    if(velAngle > RotationAngle)
                    {
                        velOffsetAngle = -1;
                    }
                    else
                    {
                        velOffsetAngle = 1;
                    }

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

                    if (sailAmount == 1)
                    {
                        //Velocity = RotationHelper.MyDirection(this, 0) * 10f * velFromWindAngle;

                        Velocity += RotationHelper.MyDirection(this, 0) * 0.1f * velFromWindAngle;

                        if (velOffsetAngle > 0)
                            Velocity += RotationHelper.MyDirection(this, 90) * 2f;

                        if (velOffsetAngle < 0)
                            Velocity += RotationHelper.MyDirection(this, -90) * 2f;
                    }
                    if (sailAmount == 2)
                    {
                        //Velocity = RotationHelper.MyDirection(this, 0) * 20f * velFromWindAngle;

                        Velocity += RotationHelper.MyDirection(this, 0) * 0.2f * velFromWindAngle;

                        if (velOffsetAngle > 0)
                            Velocity += RotationHelper.MyDirection(this, 90) * 2f;

                        if (velOffsetAngle < 0)
                            Velocity += RotationHelper.MyDirection(this, -90) * 2f;
                    }
                }
            }
            else
            {
                //Velocity = Vector3.Zero;
                GameSetup.Player.MoveTargetReached = true;
                moveLocSprite.Visible = false;
                RotationVelocity = 0;
            }
        }
    }
}