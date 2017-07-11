﻿using System;
using Engine7;
using Microsoft.Xna.Framework;

namespace Template.Game
{
    internal class CannonBall : Sprite
    {
        private Sprite player;
        private HitBox collided;
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

            //set
            //{
            //    player = value;
            //}
        }

        /// <summary>
        /// Constructor for bullet class
        /// </summary>
        /// <param name="player">The object that fired the bullet</param>
        /// <param name="fireTowards">2D vector to travel towards</param>
        /// <param name="bulletDamage">Damage on hit</param>
        /// /// <param name="type">Type of shot to use - 0 ball shot, 1 chain shot, 2 grape shot, 3 carcass shot</param>
        public CannonBall(Sprite player, Vector2 fireFrom, Vector2 fireTowards, int bulletDamage, int type)
        {
            damage = bulletDamage;
            Position2D = fireFrom;
            shotType = type;
            
            this.player = player;
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Circle4by4);
            Wash = Color.DarkGray;
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
            Timer.ShowAfterKillAfter(fireDelay, GM.r.FloatBetween(0.5f, 1f));

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
                for (int i = 0; i <= GM.r.FloatBetween(5, 10); i++)
                {
                    float spawnRot = RotationAngle + GM.r.FloatBetween(-15, 15);
                    Vector3 spawnVel = RotationHelper.Direction3DFromAngle(spawnRot, 0) * 200;
                    SmokeParticle splash = new SmokeParticle(Position2D, spawnVel, spawnRot, 0.5f);
                    splash.Wash = Color.Aqua;
                    splash.SX = 1f;
                    splash.SY = 5f;
                }

                for (int i = 0; i <= GM.r.FloatBetween(0, 5); i++)
                {
                    float spawnRot = -RotationAngle + GM.r.FloatBetween(-20, 20);
                    Vector3 spawnVel = RotationHelper.Direction3DFromAngle(spawnRot, 0) * -10;
                    SmokeParticle splash = new SmokeParticle(Position2D, spawnVel, spawnRot, 0.5f);
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
                    new SmokeParticle(Position2D, Velocity/100, RotationAngle, 5);
                }

                velApplied = true;
            }
        }

        private void Hit(Sprite hit)
        {
            if (hit is HitBox)
            {
                HitBox hitBox = (HitBox)hit;
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
                    //Debris
                    for (int i = 0; i <= GM.r.FloatBetween(0, 5); i++)
                    {
                        //float spawnRot = RotationAngle + 90 + GM.r.FloatBetween(-20, 20);
                        float spawnRot = RotationHelper.AngleFromDirection(RotationHelper.MyDirection(this, 0)) + GM.r.FloatBetween(-20, 20);
                        Vector3 spawnVel = RotationHelper.Direction3DFromAngle(spawnRot, 0) * -200;
                        SmokeParticle debris = new SmokeParticle(Position2D, spawnVel, spawnRot, 0.25f);
                        debris.Wash = Color.Brown;
                        debris.SX = 1f + GM.r.FloatBetween(-0.5f, 1f);
                        debris.SY = 4f + GM.r.FloatBetween(-3.5f, 1f);
                    }

                    if (hitBox.DamageType == 0)//Hull
                    {
                        if (shotType == 0)//Ball
                        {
                            hitBox.Health -= 10;
                        }
                        else if (shotType == 3)//Carcass
                        {
                            //Damage over time
                        }
                        else
                        {
                            hitBox.Health -= 1;
                        }
                    }
                    else if (hitBox.DamageType == 1)//Sail
                    {
                        if (shotType == 1)//Chain
                        {
                            hitBox.Health -= 10;
                        }
                        else if (shotType == 3)//Carcass
                        {
                            //Damage over time
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
            ////Debris
            //for (int i = 0; i <= GM.r.FloatBetween(0, 5); i++)
            //{
            //    float spawnRot = -RotationAngle - 90 + GM.r.FloatBetween(-20, 20);
            //    Vector3 spawnVel = RotationHelper.Direction3DFromAngle(spawnRot, 0) * -300;
            //    SmokeParticle splash = new SmokeParticle(Position2D, spawnVel, spawnRot, 0.25f);
            //    splash.Wash = Color.Brown;
            //    splash.SX = 1f + GM.r.FloatBetween(-0.5f, 1f);
            //    splash.SY = 4f + GM.r.FloatBetween(-3.5f, 1f);
            //}

            Kill();
        }
    }
}