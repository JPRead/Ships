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
        internal int shotTypeLeft;
        internal int shotTypeRight;
        private int crewNum;
        internal bool isPlayer;
        internal Sprite moveLocSprite;
        internal HitBox hitBoxHullLeft;
        internal HitBox hitBoxHullRight;
        internal HitBox hitBoxHullFront;
        internal HitBox hitBoxHullBack;
        internal HitBox hitBoxSailFront;
        internal HitBox hitBoxSailMiddle;
        internal HitBox hitBoxSailBack;
        internal Event tiReloadRight;
        internal Event tiReloadLeft;

        internal int CrewNum
        {
            get
            {
                return crewNum;
            }

            set
            {
                crewNum = value;
            }
        }

        public Ship()
        {
            //Init values
            sailAmount = 0;
            shotTypeLeft = 0;
            shotTypeRight = 0;
            crewNum = 100;

            GM.engineM.AddSprite(this);
            GM.eventM.DelayCall(0.5f, setup);
            UpdateCallBack += Move;

            GM.eventM.AddTimer(tiReloadRight = new Event(10, "Reload Cooldown Left"));
            GM.eventM.AddTimer(tiReloadLeft = new Event(10, "Reload Cooldown Right"));

            moveLocSprite = new Sprite();
            GM.engineM.AddSprite(moveLocSprite);
            moveLocSprite.Frame.Define(Tex.Circle8by8);
            CollisionActive = true;

            Friction = 0.25f;

            Visible = true;
            Frame.Define(Tex.SingleWhitePixel);
            SY = 100;
            SX = 40;
            RotationAngle = 0;

            //Hitboxes
            hitBoxHullLeft = new HitBox(this, new Vector2(-10, 0), new Vector2(25, 70), 1, 0);
            hitBoxHullLeft.Wash = Color.Red;
            hitBoxHullRight = new HitBox(this, new Vector2(10, 0), new Vector2(25, 70), 1, 0);
            hitBoxHullRight.Wash = Color.Blue;
            hitBoxHullFront = new HitBox(this, new Vector2(0, 40), new Vector2(45, 25), 0.5f, 0);
            hitBoxHullFront.Wash = Color.Green;
            hitBoxHullBack = new HitBox(this, new Vector2(0, -40), new Vector2(45, 25), 0.5f, 0);
            hitBoxHullBack.Wash = Color.Yellow;
            hitBoxSailFront = new HitBox(this, new Vector2(0, 25), new Vector2(60, 5), 1, 1);
            hitBoxSailFront.Wash = Color.Violet;
            hitBoxSailMiddle = new HitBox(this, new Vector2(0, 1), new Vector2(70, 5), 1, 1);
            hitBoxSailMiddle.Wash = Color.Violet;
            hitBoxSailBack = new HitBox(this, new Vector2(0, -30), new Vector2(65, 5), 1, 1);
            hitBoxSailBack.Wash = Color.Violet;
        }

        private void Move()
        {
            //Stop reload timers once reload is complete
            if(GM.eventM.Elapsed(tiReloadRight))
            {
                tiReloadRight.Paused = true;
            }
            if (GM.eventM.Elapsed(tiReloadLeft))
            {
                tiReloadLeft.Paused = true;
            }
        }

        private void setup()
        {

        }

        /// <summary>
        /// Fire cannons
        /// </summary>
        /// <param name="right">If true fire from right side else left side</param><param name="type">Type of shot to use - 0 ball shot, 1 bar shot, 2 grape shot, 3 carcass shot</param>
        internal void Fire(bool right, int type)
        {
            if ((right && tiReloadRight.Paused) || (right == false && tiReloadLeft.Paused))
            {
                Vector3 fireDir;
                float rightMul;
                if (right)
                    rightMul = 1;
                else
                    rightMul = -1;

                fireDir = Position + RotationHelper.MyDirection(this, rightMul * 90);

                Vector3 offsetAlongDeck = Vector3.Zero;

                offsetAlongDeck -= RotationHelper.MyDirection(this, 0) * 5;

                int multiply = 2;
                if (type == 3) { multiply = 5; } //Cannons fire multiple times - for use with grape shot

                for (int i = 0; i <= 20; i++)
                {
                    for (int i2 = 0; i2 <= multiply; i2++)
                    {
                        new CannonBall(this,
                            new Vector2(Position2D.X - (Width) * RotationHelper.MyDirection(this, 0).X + offsetAlongDeck.X, Position2D.Y - (Width) * RotationHelper.MyDirection(this, 0).Y + offsetAlongDeck.Y),
                            new Vector2(fireDir.X - (Width) * RotationHelper.MyDirection(this, 0).X + offsetAlongDeck.X, fireDir.Y - (Width) * RotationHelper.MyDirection(this, 0).Y + offsetAlongDeck.Y),
                            type);
                    }

                    offsetAlongDeck += RotationHelper.MyDirection(this, 0) * 5;
                }

                if (right == true)
                {
                    tiReloadRight.Paused = false;
                }
                else
                {
                    tiReloadLeft.Paused = false;
                }
            }
        }

        internal void MoveToPoint(Point point)
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