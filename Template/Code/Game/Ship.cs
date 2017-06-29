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

                //Wind multiplier - WIP
                Vector3 velMul;
                RotationHelper.Direction2DFromAngle(GameSetup.WindDir, 0);

                if (sailAmount == 0)
                {
                    //Velocity = Vector3.Zero;
                }
                else if (sailAmount == 1)
                {

                    

                    Vector3 currentVel = Velocity;
                    currentVel.Normalize();
                    float velAngle = RotationHelper.AngleFromDirection(currentVel);
                    //Negative for travelling left of current direction
                    float velOffsetAngle = velAngle - RotationAngle;

                    Velocity += RotationHelper.MyDirection(this, 0) * 0.1f;

                    if(velOffsetAngle > 0)
                        Velocity += RotationHelper.MyDirection(this, -90) * 0.1f;

                    if(velOffsetAngle < 0)
                        Velocity += RotationHelper.MyDirection(this, 90) * 0.1f;
                }
                else
                {
                    Vector3 currentVel = Velocity;
                    currentVel.Normalize();
                    float velAngle = RotationHelper.AngleFromDirection(currentVel);
                    //Negative for travelling left of current direction
                    float velOffsetAngle = velAngle - RotationAngle;

                    Velocity += RotationHelper.MyDirection(this, 0) * 0.2f;

                    if (velOffsetAngle > 0)
                        Velocity += RotationHelper.MyDirection(this, -90) * 0.1f;

                    if (velOffsetAngle < 0)
                        Velocity += RotationHelper.MyDirection(this, 90) * 0.1f;
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