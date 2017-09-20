using System;
using Engine7;
using Microsoft.Xna.Framework;

namespace Template.Game
{
    internal class CannonBall : Sprite
    {
        private Sprite player;
        private int damage;
        private int shotType;
        private float fireDelay;
        bool velApplied;
        bool splash;

        internal Sprite Player
        {
            get
            {
                return player;
            }
        }

        /// <summary>
        /// Constructor for bullet class
        /// </summary>
        /// <param name="player">The object that fired the bullet</param>
        /// <param name="fireTowards">2D vector to travel towards</param>
        /// <param name="type">Type of shot to use - 0 ball shot, 1 bar shot, 2 carcass shot, 3 grape shot</param>
        public CannonBall(Sprite player, Vector2 fireFrom, Vector2 fireTowards, int type)
        {
            Position2D = fireFrom;
            shotType = type;
            
            this.player = player;
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
                    ScaleBoth = 1.25f;
                    break;
                case 3:
                    Frame.Define(GM.txSprite, new Rectangle(143, 159, 2, 2));
                    ScaleBoth = 2;
                    break;
                default:
                    Frame.Define(Tex.Circle4by4);
                    Wash = Color.DarkGray;
                    break;
            }
            Friction = 0.25f;

            //Create direction vector and normalise
            Vector2 direction = fireTowards - Position2D;
            direction = Vector2.Normalize(direction);

            //Face direction vector
            RotationHelper.FaceDirection(this, direction, DirectionAccuracy.free, 0);
            Position += RotationHelper.MyDirection(this, 0) * 32;

            //collision setup
            CollisionActive = true;
            CollisionPrimary = true;
            PrologueCallBack += Hit;
            EpilogueCallBack += AfterHit;
            Moving = true;
            
            //kill after 5 seconds and delay firing
            TimerInitialise();
            fireDelay = GM.r.FloatBetween(0, 0.5f);
            if (type != 3)
            {
                Timer.ShowAfterKillAfter(fireDelay, GM.r.FloatBetween(0.5f, 1f));
            }
            else {
                Timer.ShowAfterKillAfter(fireDelay, GM.r.FloatBetween(0.2f, 0.4f));
            }

            velApplied = false;
            UpdateCallBack += Move;

            splash = true;
            FuneralCallBack += Death;
        }

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

        private void Move()
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
                    new FadingParticle(Position2D, Velocity/100, RotationAngle, 5);
                }

                velApplied = true;
            }
        }

        private void Hit(Sprite hit)
        {
            if (hit is HitBox)
            {
                HitBox hitBox = (HitBox)hit; //Get owner of hitbox
                if (hitBox.Owner is HitBox)
                {
                    hitBox = (HitBox)hitBox.Owner;
                }

                if (hitBox.Owner == player)
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
                            if(GM.r.FloatBetween(0, 1) > 0.90f)
                            {
                                ship.CrewNum -= 1;
                            }
                        }
                        else if (shotType == 2)//Carcass
                        {
                            hitBox.Health -= (int)(2 * hitBox.DamageMul);
                            if (GM.r.FloatBetween(0, 1) > 0.95)
                            {
                                hitBox.IsBurning = true;
                            }
                        }
                        else
                        {
                            hitBox.Health -= (int)(1 * hitBox.DamageMul);
                        }
                        if (shotType == 3 && GM.r.FloatBetween(0,1) > 0.95) //Grape
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
                        else if(shotType == 0) //Ball
                        {
                            hitBox.Health -= (int)(1 * hitBox.DamageMul);
                        }
                        else if (shotType == 2)//Carcass
                        {
                            if(GM.r.FloatBetween(0,1) > 0.90)
                            {
                                hitBox.IsBurning = true;
                            }
                        }
                        else
                        {
                            CollisionAbandonResponse = true;
                            hitBox.Health -= 1;
                        }
                    }
                }
            }
        }

        private void AfterHit(Sprite hit)
        {
            splash = false;
            Kill();
        }
    }
}