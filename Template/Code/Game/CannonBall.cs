using System;
using Engine7;
using Microsoft.Xna.Framework;

namespace Template.Game
{
    internal class CannonBall : Sprite
    {
        /// <summary>
        /// Ship that fired the CannonBall
        /// </summary>
        private Sprite owner;
        /// <summary>
        /// Type of this CannonBall - 0 ball shot, 1 bar shot, 2 carcass shot, 3 grape shot
        /// </summary>
        private int shotType;
        /// <summary>
        /// Time to wait before firing
        /// </summary>
        private float fireDelay;
        /// <summary>
        /// True if velocity has been added after spawning
        /// </summary>
        bool velApplied;
        /// <summary>
        /// True if CannonBall doesnt hit anything and needs to splash as though it landed in water
        /// </summary>
        bool splash;

        //internal Sprite Owner
        //{
        //    get
        //    {
        //        return owner;
        //    }
        //}

        /// <summary>
        /// Constructor for CannonBall class
        /// </summary>
        /// <param name="owner">The ship that fired the CannonBall</param>
        /// <param name="fireFrom">2D position to fire from</param>
        /// <param name="fireDir">2D position to fire towards</param>
        /// <param name="type">Type of shot to use - 0 ball shot, 1 bar shot, 2 carcass shot, 3 grape shot, 4 grapple shot</param>
        public CannonBall(Sprite owner, Vector3 fireFrom, Vector3 fireDir, int type)
        {
            //Init values
            Position = fireFrom;
            shotType = type;
            this.owner = owner;

            //Find sprite to render
            GM.engineM.AddSprite(this);
            switch (type)
            {
                case 0:
                    Frame.Define(GM.txSprite, new Rectangle(111, 160, 5, 5));
                    break;
                case 1:
                    Frame.Define(GM.txSprite, new Rectangle(121, 161, 7, 3));
                    ScaleBoth = 1.25f;
                    break;
                case 2:
                    Frame.Define(GM.txSprite, new Rectangle(133, 160, 5, 7));
                    break;
                case 3:
                    Frame.Define(GM.txSprite, new Rectangle(143, 159, 2, 2));
                    ScaleBoth = 4;
                    break;
                case 4:
                    Frame.Define(GM.txSprite, new Rectangle(133, 170, 5, 5));
                    ScaleBoth = 1.5f;
                    break;
                default:
                    Frame.Define(Tex.Circle4by4);
                    Wash = Color.DarkGray;
                    break;
            }
            Friction = 0.25f;

            //Face along firDir
            RotationHelper.FaceDirection(this, fireDir, DirectionAccuracy.free, 0);
            Position += RotationHelper.MyDirection(this, 0) * 50;

            //Collision setup
            CollisionActive = true;
            CollisionPrimary = true;
            PrologueCallBack += Hit;
            EpilogueCallBack += AfterHit;
            Moving = true;
            
            //kill after a random time and delay firing
            TimerInitialise();
            fireDelay = GM.r.FloatBetween(0, 0.5f);
            if (type != 3)
            {
                Timer.ShowAfterKillAfter(fireDelay, GM.r.FloatBetween(0.5f, 0.7f));
            }
            else
            {
                Timer.ShowAfterKillAfter(fireDelay, GM.r.FloatBetween(0.2f, 0.4f));
            }

            velApplied = false;
            UpdateCallBack += Tick;

            splash = true;
            FuneralCallBack += Death;
        }

        /// <summary>
        /// Creates particles when deleted
        /// </summary>
        private void Death()
        {
            if (splash)
            {
                //Splash
                int maxParts = 10;
                int minParts = 5;

                if(shotType == 3) { maxParts = 2; minParts = 1; }
                
                for (int i = 0; i <= GM.r.FloatBetween(minParts, maxParts); i++)
                {
                    float spawnRot = RotationAngle + GM.r.FloatBetween(-15, 15);
                    Vector3 spawnVel = RotationHelper.Direction3DFromAngle(spawnRot, 0) * 200;
                    FadingParticle splash = new FadingParticle(Position2D, spawnVel, spawnRot, 0.25f);
                    splash.Wash = Color.Aqua;
                    splash.SX = 1f;
                    splash.SY = 5f;
                }

                for (int i = 0; i <= GM.r.FloatBetween(0, 5); i++)
                {
                    float spawnRot = -RotationAngle + GM.r.FloatBetween(-20, 20);
                    Vector3 spawnVel = RotationHelper.Direction3DFromAngle(spawnRot, 0) * -10;
                    FadingParticle splash = new FadingParticle(Position2D, spawnVel, spawnRot, 0.1f);
                    splash.Wash = Color.Aqua;
                    splash.SX = 1f;
                    splash.SY = 2.5f;
                }
            }
        }

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Tick()
        {
            if (Visible && velApplied == false)
            {
                //Add velocity
                RotationHelper.VelocityInCurrentDirection(this, 750, 0);

                Velocity += new Vector3(GM.r.FloatBetween(-20, 20), GM.r.FloatBetween(-20, 20), 0);

                //Play sound effect
                GM.audioM.PlayEffect("shoot");

                //Release smoke
                int rAmount = (int)GM.r.FloatBetween(2, 4);
                for(int i = 0; i < rAmount; i++)
                {
                    new FadingParticle(Position2D, Velocity * 0.01f, RotationAngle, 5);
                }

                velApplied = true;
            }
        }

        /// <summary>
        /// Code to run upon hitting a sprite
        /// </summary>
        /// <param name="hit">The sprite collided with</param>
        private void Hit(Sprite hit)
        {
            if (hit is HitBox && GM.r.FloatBetween(0, 1) > 0.75)
            {
                HitBox hitBox = (HitBox)hit; //Get owner of hitbox
                if (hitBox.Owner is HitBox)
                {
                    hitBox = (HitBox)hitBox.Owner;
                }

                if (hitBox.Owner == owner)
                {
                    CollisionAbandonResponse = true;
                }
                else
                {
                    Ship ship = (Ship)hitBox.Owner;

                    //Debris
                    for (int i = 0; i <= GM.r.FloatBetween(0, 5); i++)
                    {
                        //float spawnRot = RotationAngle + 90 + GM.r.FloatBetween(-20, 20);
                        float spawnRot = RotationHelper.AngleFromDirection(RotationHelper.MyDirection(this, 0)) + GM.r.FloatBetween(-40, 40);
                        Vector3 spawnVel = RotationHelper.Direction3DFromAngle(spawnRot, 0) * -200;
                        FadingParticle debris = new FadingParticle(Position2D, spawnVel, spawnRot, 0.1f);
                        debris.Wash = Color.Brown;
                        debris.SX = 3f + GM.r.FloatBetween(-2f, 2f);
                        debris.SY = 3f + GM.r.FloatBetween(-2f, 2f);
                    }

                    if (hitBox.DamageType == 0)//Hull
                    {
                        if (shotType == 0)//Ball
                        {
                            hitBox.Health -= (int)(10 * hitBox.DamageMul);
                            if(GM.r.FloatBetween(0, 1) > 0.90)
                            {
                                ship.CrewNum -= 1;
                            }
                        }
                        else if (shotType == 2)//Carcass
                        {
                            hitBox.Health -= (int)(2.5f * hitBox.DamageMul);
                            if (GM.r.FloatBetween(0, 1) > 0.95)
                            {
                                hitBox.IsBurning = true;
                            }
                            if (GM.r.FloatBetween(0, 1) > 0.90)
                            {
                                ship.CrewNum -= 1;
                            }
                        }
                        else if(shotType == 4 && GM.r.FloatBetween(0,1) > 0.5f)//Grapple
                        {
                            Ship firedFrom = (Ship)owner;
                            firedFrom.Board(ship);
                        }
                        else
                        {
                            hitBox.Health -= (int)(1 * hitBox.DamageMul);
                        }
                        if (shotType == 3 && GM.r.FloatBetween(0,1) > 0.4f) //Grape
                        {
                            ship.CrewNum -= (int)GM.r.FloatBetween(1, 5);
                        }
                    }
                    else if (hitBox.DamageType == 1)//Sail
                    {
                        if (shotType == 1)//Bar
                        {
                            hitBox.Health -= (int)(10 * hitBox.DamageMul);
                        }
                        else if (shotType == 2)//Carcass
                        {
                            hitBox.Health -= (int)(5 * hitBox.DamageMul);
                            if (GM.r.FloatBetween(0,1) > 0.5)
                            {
                                hitBox.IsBurning = true;
                            }
                        }
                        else
                        {
                            //CollisionAbandonResponse = true;
                            hitBox.Health -= 1;
                        }
                    }
                    //Kill();
                }
            }
        }

        /// <summary>
        /// Code to run after resolving hit
        /// </summary>
        /// <param name="hit">Sprite that was hit</param>
        private void AfterHit(Sprite hit)
        {
            splash = false;
            Kill();
        }
    }
}