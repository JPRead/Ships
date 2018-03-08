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
        /// True if ship is repairing - USE IsRepairing PROPERTY INSTEAD OF THIS
        /// </summary>
        private bool _isRepairing;
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
        /// Array of the ship's HitBoxes, maximum index is 7
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
        /// <summary>
        /// True if boarding enemy ship
        /// </summary>
        internal bool isBoarding;
        /// <summary>
        /// True if being boarded by enemy ship
        /// </summary>
        internal bool isBoarded;
        /// <summary>
        /// True if attempting to cut boarding ropes
        /// </summary>
        internal bool isCuttingRopes;
        /// <summary>
        /// Round shot remaining
        /// </summary>
        internal int roundShotNum;
        /// <summary>
        /// Grape shot remaining
        /// </summary>
        internal int grapeShotNum;
        /// <summary>
        /// Carcass shot remaining
        /// </summary>
        internal int carcassShotNum;
        /// <summary>
        /// Bar shot remaining
        /// </summary>
        internal int barShotNum;
        /// <summary>
        /// Grapple shot remaining
        /// </summary>
        internal int grappleShotNum;

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

        internal bool IsRepairing
        {
            get
            {
                return _isRepairing;
            }

            set
            {
                if (isPlayer)
                {
                    if (value == false)
                    {
                        GameSetup.Player.RepairButton.SetDisplay(new Rectangle(67, 200, 42, 38));
                    }
                    else
                    {
                        GameSetup.Player.RepairButton.SetDisplay(new Rectangle(110, 200, 42, 38));
                    }
                }
                _isRepairing = value;
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
            IsRepairing = false;
            hitBoxArray = new HitBox[7];
            leftLoaded = false;
            rightLoaded = false;
            isBoarding = false;
            isBoarded = false;
            isCuttingRopes = false;
            
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

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Tick()
        {
            //Run code in this function every second
            if (GM.eventM.Elapsed(tiOneSecond))
            {
                OneSecond();
            }

            //Kill if no crew
            if(crewNum <= 0)
            {
                Kill();
            }

            //Keep rotation angle between -180 and 180 degrees
            if(RotationAngle < -180)
            {
                RotationAngle += 360;
            }
            else if(RotationAngle > 180)
            {
                RotationAngle -= 360;
            }

            //Stop reload timers once reload is complete or if repairing
            if (GM.eventM.Elapsed(tiReloadRight) || IsRepairing)
            {
                tiReloadRight.Paused = true;
                if(!IsRepairing)
                    rightLoaded = true;
            }
            if (GM.eventM.Elapsed(tiReloadLeft) || IsRepairing)
            {
                tiReloadLeft.Paused = true;
                if (!IsRepairing)
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
                velVector /= angleDifference;
            
                Velocity2D += velVector * 30 * (Velocity2D - otherShip.Velocity2D);
            }
        }

        /// <summary>
        /// Code to run each second
        /// </summary>
        internal void OneSecond()
        {
            //Repairing
            if (IsRepairing)
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
                    IsRepairing = false;
                }
                else
                {
                    float repairPerPart = (repairAmount * 0.025f) / repairNum;
                    for (int i = 0; i < repairNum; i++)
                    {
                        repairArray[i].Health += repairPerPart;
                        if (repairArray[i].Health > 100)
                            repairArray[i].Health = 100;

                        if (repairArray[i].IsBurning && GM.r.FloatBetween(0, 1) > 0.9 * (0.9 - GameSetup.WeatherController.RainAmount))
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
            sinkAmount += (int)(hullHealthMissing * 0.15);
            if (GameSetup.WeatherController.RainAmount > 0.75f)
            {
                sinkAmount -= 4;
            }
            else
            {
                sinkAmount -= 6;
            }
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
            sailDamageSpeedMul *= 0.003f;

            //Boarding checks
            if (isBoarded && isCuttingRopes)
            {
                if(GM.r.FloatBetween(0,1) > 0.75f)
                {
                    isCuttingRopes = false;
                    isBoarded = false;
                    GameSetup.BoardingInProgress = false;
                    if (isPlayer)
                    {
                        GameSetup.Opponent.isBoarding = false;
                    }
                    else
                    {
                        GameSetup.Player.isBoarding = false;
                    }
                }
            }
        }

        /// <summary>
        /// Fire cannons
        /// </summary>
        /// <param name="right">If true fire from right side else left side</param><param name="type">Type of shot to use - 0 ball shot, 1 bar shot, 2 grape shot, 3 carcass shot, 4 grapple shot</param>
        internal void Fire(bool right, int type)
        {
            right = !right;
            bool hasAmmo = true;
            if ((right && rightLoaded) || (!right && leftLoaded) && !IsRepairing)
            {
                if (type == 0 && roundShotNum > 0)
                {
                    roundShotNum--;
                }
                else if (type == 1 && barShotNum > 0)
                {
                    barShotNum--;
                }
                else if (type == 2 && grapeShotNum > 0)
                {
                    grapeShotNum--;
                }
                else if (type == 3 && carcassShotNum > 0)
                {
                    carcassShotNum--;
                }
                else if (type == 4 && grappleShotNum > 0)
                {
                    grappleShotNum--;
                }
                else
                {
                    hasAmmo = false;
                }
            }
            else
            {
                hasAmmo = false;
            }
            if (hasAmmo)
            {
                Vector3 fireDir;
                Vector3 deckDir = Vector3.Zero;
                int rightMul;

                if (right)
                    rightMul = 1;
                else
                    rightMul = -1;

                fireDir = RotationHelper.MyDirection(this, 90 * rightMul);
                deckDir = RotationHelper.MyDirection(this, 0);

                int multiply = 2;
                if (type == 3) { multiply = 4; }
                if (type == 4) { multiply = 1; }
                for (int i = 0; i < crewNum * 0.1f; i++)
                {
                    for (int i2 = 0; i2 < multiply; i2++)
                    {
                        new CannonBall(this, Position + (deckDir * i * 10) - (deckDir * 48), fireDir, type);
                    }
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

        internal void cutBoardingRopes()
        {
            if (isBoarding)
            {
                isBoarding = false;
                GameSetup.BoardingInProgress = false;
                if (isPlayer)
                    GameSetup.Opponent.isBoarded = false;
                else
                    GameSetup.Player.isBoarded = false;
            }
            else
            {
                isCuttingRopes = true;
            }
        }

        /// <summary>
        /// Accelerates the ship towards point and keeps the ship from sliding sideways
        /// </summary>
        /// <param name="point">Point to move towards</param>
        internal void MoveToPoint(Point point)
        {
            Vector2 movePos = PointHelper.Vector2FromPoint(point);

            //Stop moving once close enough to movePos, or when boarding is in progress
            if ((Vector2.DistanceSquared(Position2D, movePos) + 5000 > Height * Height) && !isBoarded && !isBoarding)
            {
                if (isPlayer)
                    moveLocSprite.Visible = true;
                moveLocSprite.Position2D = movePos;

                //DEBUG
                if (!isPlayer)
                {
                    moveLocSprite.Visible = true;
                    moveLocSprite.Wash = Color.Red;
                }


                if (sailAmount == 0)
                {
                    if (smoothRotationVelocity > 0)
                    {
                        smoothRotationVelocity -= 0.1f;
                        RotationVelocity = smoothRotationVelocity;
                    }
                    if (smoothRotationVelocity < 0)
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
                    float velFromWindAngle = (RotationAngle - GameSetup.WeatherController.WindDir) % 360;
                    if (velFromWindAngle < 0) //Absolute value
                        velFromWindAngle = -velFromWindAngle;
                    if (velFromWindAngle > 180) //Keep between 0 and 180
                    {
                        velFromWindAngle = 360 - velFromWindAngle;
                    }//Create multiplier that's <1
                    velFromWindAngle = (1 / (velFromWindAngle + 100) * 50);

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
            else if (isBoarding || isBoarded)
            {
                Ship target = GameSetup.Opponent;
                if (!isPlayer)
                {
                    target = GameSetup.Player;
                }

                float angleBetween = RotationAngle - target.RotationAngle;
                bool opposite = true;
                if (angleBetween > -90 && angleBetween < 90)
                {
                    opposite = false;
                }

                //Angle ships parallel to eachother.
                if (opposite &&
                    Vector2.DistanceSquared(hitBoxHullFront.Position2D, target.hitBoxHullBack.Position2D)
                    > Vector2.DistanceSquared(hitBoxHullBack.Position2D, target.hitBoxHullFront.Position2D))
                {
                    RotationVelocity = 2;
                }
                else if (!opposite &&
                    Vector2.DistanceSquared(hitBoxHullFront.Position2D, target.hitBoxHullFront.Position2D)
                    > Vector2.DistanceSquared(hitBoxHullBack.Position2D, target.hitBoxHullBack.Position2D))
                {
                    RotationVelocity = 2;
                }
                else
                    RotationVelocity = -2;

                //Move ships towards eachother
                if (Vector2.DistanceSquared(this.Position2D, target.Position2D) > 10000)
                {
                    Vector2 velDir = Vector2.Normalize(Position2D - target.Position2D);
                    Velocity2D -= 0.01f * velDir;
                }
                else
                {
                    GameSetup.BoardingInProgress = true;
                    Velocity2D = Vector2.Zero;
                }

                //DEBUG
                GM.textM.Draw(FontBank.arcadePixel, Convert.ToString(opposite), 200, 200);
            }
            else
            {
                moveTargetReached = true;
                moveLocSprite.Visible = false;
                RotationVelocity = 0;
            }
        }

        public void Board(Ship target)
        {
            isBoarding = true;
            target.isBoarded = true;

            float angleBetween = RotationAngle - target.RotationAngle;
            bool opposite = true;
            if(angleBetween > -90 && angleBetween < 90)
            {
                opposite = false;
            }

            new Rope(this, target);
            if (opposite)
            {
                new Rope(hitBoxHullBack, target.hitBoxHullFront);
                new Rope(hitBoxHullFront, target.hitBoxHullBack);
            }
            else
            {
                new Rope(hitBoxHullFront, target.hitBoxHullFront);
                new Rope(hitBoxHullBack, target.hitBoxHullBack);
            }
            MoveToPoint(new Point(0, 0));
        }
    }
}