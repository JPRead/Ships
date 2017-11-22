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
    internal class Ship : Sprite
    {
        /// <summary>
        /// 0 stopped, 1 half speed, 2 full speed
        /// </summary>
        internal int sailAmount;
        /// <summary>
        /// Type of shot to be fired from left cannons
        /// </summary>
        internal int shotTypeLeft;
        /// <summary>
        /// Type of shot to be fired from right cannons
        /// </summary>
        internal int shotTypeRight;
        /// <summary>
        /// Number of crew remaining
        /// </summary>
        private int crewNum;
        /// <summary>
        /// True if ship has collided in the last tick
        /// </summary>
        internal bool hasCollided;
        /// <summary>
        /// True if ship is player
        /// </summary>
        internal bool isPlayer;
        /// <summary>
        /// True if ship is repairing
        /// </summary>
        internal bool isRepairing;
        /// <summary>
        /// True if right cannons are loaded
        /// </summary>
        internal bool rightLoaded;
        /// <summary>
        /// True if left cannons are loaded
        /// </summary>
        internal bool leftLoaded;
        /// <summary>
        /// Sprite used to display player's move orders
        /// </summary>
        private Sprite moveLocSprite;
        /// <summary>
        /// Array of the ship's HitBoxes, maximum index is 7.
        /// </summary>
        internal HitBox[] hitBoxArray;
        /// <summary>
        /// HitBox for the left hull
        /// </summary>
        internal HitBox hitBoxHullLeft;
        /// <summary>
        /// HitBox for the right hull
        /// </summary>
        internal HitBox hitBoxHullRight;
        /// <summary>
        /// HitBox for the front hull
        /// </summary>
        internal HitBox hitBoxHullFront;
        /// <summary>
        /// HitBox for the back hull
        /// </summary>
        internal HitBox hitBoxHullBack;
        /// <summary>
        /// HitBox for the front sail
        /// </summary>
        internal HitBox hitBoxSailFront;
        /// <summary>
        /// Hitbox for the middle sail
        /// </summary>
        internal HitBox hitBoxSailMiddle;
        /// <summary>
        /// Hitbox for the back sail
        /// </summary>
        internal HitBox hitBoxSailBack;
        /// <summary>
        /// Timer for right cannon reload
        /// </summary>
        internal Event tiReloadRight;
        /// <summary>
        /// Timer for left cannon reload
        /// </summary>
        internal Event tiReloadLeft;
        /// <summary>
        /// Timer for 1 second delay
        /// </summary>
        internal Event tiOneSecond;
        /// <summary>
        /// Used to keep track of and smooth out RotationVelocity, since RotationVelocity only applies for a single tick
        /// </summary>
        private float smoothRotationVelocity;
        /// <summary>
        /// Returns true if ship is close enough the the moveTo point
        /// </summary>
        internal bool moveTargetReached;
        /// <summary>
        /// Amount of water a ship has taken on
        /// </summary>
        internal int sinkAmount;
        /// <summary>
        /// Multiplier to speed based on sail damage
        /// </summary>
        private float sailDamageSpeedMul;

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

        /// <summary>
        /// Ship contains all the functions used by both the player and the AI
        /// </summary>
        public Ship()
        {
            //Init values
            sailAmount = 0;
            shotTypeLeft = 0;
            shotTypeRight = 0;
            crewNum = 100;
            isRepairing = false;
            hitBoxArray = new HitBox[7];
            leftLoaded = false;
            rightLoaded = false;
            

            GM.engineM.AddSprite(this);
            UpdateCallBack += Tick;

            GM.eventM.AddTimer(tiReloadRight = new Event(10, "Reload Cooldown Left"));
            GM.eventM.AddTimer(tiReloadLeft = new Event(10, "Reload Cooldown Right"));
            GM.eventM.AddTimer(tiOneSecond = new Event(1, "Repair Tick"));

            moveLocSprite = new Sprite();
            GM.engineM.AddSprite(moveLocSprite);
            moveLocSprite.Frame.Define(Tex.Circle8by8);
            moveLocSprite.Visible = false;
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
            hitBoxArray[0] = hitBoxHullLeft;
            hitBoxHullRight = new HitBox(this, new Vector2(10, 0), new Vector2(25, 70), 1, 0);
            hitBoxHullRight.Wash = Color.Blue;
            hitBoxArray[1] = hitBoxHullRight;
            hitBoxHullFront = new HitBox(this, new Vector2(0, 40), new Vector2(45, 25), 0.5f, 0);
            hitBoxHullFront.Wash = Color.Green;
            hitBoxArray[2] = hitBoxHullFront;
            hitBoxHullBack = new HitBox(this, new Vector2(0, -40), new Vector2(45, 25), 0.5f, 0);
            hitBoxHullBack.Wash = Color.Yellow;
            hitBoxArray[3] = hitBoxHullBack;
            hitBoxSailFront = new HitBox(this, new Vector2(0, 25), new Vector2(60, 5), 1, 1);
            hitBoxSailFront.Wash = Color.Violet;
            hitBoxArray[4] = hitBoxSailFront;
            hitBoxSailMiddle = new HitBox(this, new Vector2(0, 1), new Vector2(70, 5), 1, 1);
            hitBoxSailMiddle.Wash = Color.Violet;
            hitBoxArray[5] = hitBoxSailMiddle;
            hitBoxSailBack = new HitBox(this, new Vector2(0, -30), new Vector2(65, 5), 1, 1);
            hitBoxSailBack.Wash = Color.Violet;
            hitBoxArray[6] = hitBoxSailBack;

            //DEBUG
            hitBoxHullBack.Health = 100;
            hitBoxHullFront.Health = 100;
            hitBoxHullLeft.Health = 100;
            hitBoxHullRight.Health = 100;
            hitBoxSailFront.Health = 100;
            hitBoxSailMiddle.Health = 100;
            hitBoxSailBack.Health = 100;
            crewNum = 100;
        }

        private void Tick()
        {
            //Run code in this function every second
            if (GM.eventM.Elapsed(tiOneSecond))
            {
                OneSecond();
            }

            //Stop reload timers once reload is complete or if repairing
            if (GM.eventM.Elapsed(tiReloadRight) || isRepairing)
            {
                tiReloadRight.Paused = true;
                if(!isRepairing)
                    rightLoaded = true;
            }
            if (GM.eventM.Elapsed(tiReloadLeft) || isRepairing)
            {
                tiReloadLeft.Paused = true;
                if (!isRepairing)
                    leftLoaded = true;
            }

            //Checking for collisions
            if (hasCollided)
            {
                hasCollided = false;
                Ship otherShip;
                if (isPlayer)
                {
                    otherShip = GameSetup.Opponent;
                }
                else
                {
                    otherShip = GameSetup.Player;
                }
                Vector2 velVector = Position2D - otherShip.Position2D;
                velVector.Normalize();
                //Multiply based on angle to velVector (reduce if sideways)
                float angleDifference = Math.Abs(RotationHelper.AngleFromDirection(velVector) - RotationAngle + 90);
                velVector = (360*velVector) / angleDifference;

                Velocity2D += velVector;
                //otherShip.Velocity2D -= (velVector * 2);
            }
            //DEBUG
            GM.textM.Draw(FontBank.arcadePixel, Convert.ToString(sailDamageSpeedMul), 200, 200);
        }

        /// <summary>
        /// Runs every second
        /// </summary>
        internal void OneSecond()
        {
            //Repairing
            if (isRepairing)
            {
                float repairAmount = crewNum;

                //Spread repair amount amongst each part
                HitBox[] repairArray = new HitBox[7];
                int repairNum = 0;
                for (int i = 0; i <= 6; i++)
                {
                    if (hitBoxArray[i].Health < 100 || hitBoxArray[i].IsBurning)
                    {
                        repairArray[repairNum] = hitBoxArray[i];
                        repairNum++;
                    }
                }
                if (repairNum == 0)
                {
                    if (!rightLoaded)
                    {
                        tiReloadRight.Paused = false;
                    }
                    if (!leftLoaded)
                    {
                        tiReloadLeft.Paused = false;
                    }

                    isRepairing = false;
                }
                else
                {
                    float repairPerPart = (repairAmount * 0.05f) / repairNum;
                    for (int i = 0; i < repairNum; i++)
                    {
                        repairArray[i].Health += repairPerPart;
                        if (repairArray[i].Health > 100)
                            repairArray[i].Health = 100;

                        if (repairArray[i].IsBurning && GM.r.FloatBetween(0, 1) > 0.9)
                        {
                            repairArray[i].IsBurning = false;
                        }
                    }
                }
            }

            //Calculations for sinking
            float hullHealthMissing = 0;
            for (int i = 0; i <= 3; i++)
            {
                hullHealthMissing += (100 - hitBoxArray[i].Health);
            }
            sinkAmount += (int)(hullHealthMissing/10);
            sinkAmount -= 6;
            if(sinkAmount >= 1000)
            {
                //Splash
                for (int i = 0; i <= GM.r.FloatBetween(200, 400); i++)
                {
                    float spawnRot = GM.r.FloatBetween(0, 360);
                    Vector3 spawnVel = RotationHelper.Direction3DFromAngle(spawnRot, 0) * 200;
                    FadingParticle splash = new FadingParticle(Position2D, spawnVel, spawnRot, 0.25f);
                    splash.Wash = Color.Aqua;
                    splash.SX = 1f;
                    splash.SY = 5f;
                }
                Kill();
            }

            //Calculations for sail damage speed multiplier
            sailDamageSpeedMul = 0;
            for (int i = 4; i <= 6; i++)
            {
                sailDamageSpeedMul += hitBoxArray[i].Health;
            }
            sailDamageSpeedMul /= 300;
        }

        /// <summary>
        /// Fire cannons
        /// </summary>
        /// <param name="right">If true fire from right side else left side</param><param name="type">Type of shot to use - 0 ball shot, 1 bar shot, 2 grape shot, 3 carcass shot</param>
        internal void Fire(bool right, int type)
        {
            if (((right && tiReloadRight.Paused) || (!right && tiReloadLeft.Paused)) && !isRepairing)
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

                int multiply = 1;
                if (type == 3) { multiply = 3; } //Cannons fire multiple times - for use with grape shot

                for (int i = 0; i <= (crewNum); i++)
                {
                    for (int i2 = 0; i2 <= multiply; i2++)
                    {
                        //Travelling along the deck of the ship
                        new CannonBall(this,
                            new Vector2(Position2D.X - (Width) * RotationHelper.MyDirection(this, 0).X + offsetAlongDeck.X, Position2D.Y - (Width) * RotationHelper.MyDirection(this, 0).Y + offsetAlongDeck.Y),
                            new Vector2(fireDir.X - (Width) * RotationHelper.MyDirection(this, 0).X + offsetAlongDeck.X, fireDir.Y - (Width) * RotationHelper.MyDirection(this, 0).Y + offsetAlongDeck.Y),
                            type);
                    }

                    offsetAlongDeck += RotationHelper.MyDirection(this, 0) * (100/crewNum);
                }

                if (right == true)
                {
                    tiReloadRight.Paused = false;
                    rightLoaded = false;
                }
                else
                {
                    tiReloadLeft.Paused = false;
                    leftLoaded = false;
                }
            }
        }

        /// <summary>
        /// Accelerates the ship towards point and keeps the ship from sliding sideways
        /// </summary>
        /// <param name="point">Point to move towards</param>
        internal void MoveToPoint(Point point)
        {
            Vector2 movePos = PointHelper.Vector2FromPoint(point);

            //Stop moving once close enough to movePos
            if(Vector2.DistanceSquared(Position2D, movePos) + 5000 > Height * Height)
            {
                if (isPlayer)
                    moveLocSprite.Visible = true;
                moveLocSprite.Position2D = movePos;
                
                if (sailAmount == 0)
                {
                    if(smoothRotationVelocity > 0)
                    {
                        smoothRotationVelocity -= 0.1f;
                        RotationVelocity = smoothRotationVelocity;                        
                    }
                    if(smoothRotationVelocity < 0)
                    {
                        smoothRotationVelocity += 0.1f;
                        RotationVelocity = smoothRotationVelocity;
                    }
                }
                else
                {
                    //Calculations for turning
                    int dirMul = (int)RotationHelper.AngularDirectionTo(this, new Vector3(movePos, 0), 0, false);
                    smoothRotationVelocity += 0.05f * dirMul;
                    if ((dirMul > 0 && smoothRotationVelocity > 5 * dirMul) || (dirMul < 0 && smoothRotationVelocity < 5 * dirMul))
                    {
                        smoothRotationVelocity = 5 * dirMul;
                    }
                    RotationVelocity = smoothRotationVelocity;

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
                    }//Create multiplier that's <1
                    velFromWindAngle = (1/(velFromWindAngle+100)*100);

                    //Keep from sliding to the side
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
                        Velocity += RotationHelper.MyDirection(this, 0) * 0.1f * velFromWindAngle * sailDamageSpeedMul;
                    }
                    if (sailAmount == 2)
                    {
                        Velocity += RotationHelper.MyDirection(this, 0) * 0.2f * velFromWindAngle * sailDamageSpeedMul;//Sort this out so it doesnt divide by 0
                    }
                }
            }
            else
            {
                moveTargetReached = true;
                moveLocSprite.Visible = false;
                RotationVelocity = 0;
            }
        }
    }
}