using System;
using Engine7;
using Microsoft.Xna.Framework;

namespace Template.Game
{
    internal class CannonBall : Sprite
    {
        private Sprite player;
        private int damage;
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
            
            this.player = player;
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Circle4by4);
            Wash = Color.DarkGray;

            //Create direction vector and normalise
            Vector2 direction = fireTowards - Position2D;
            direction = Vector2.Normalize(direction);

            //Face direction vector
            RotationHelper.FaceDirection(this, direction, DirectionAccuracy.free, 0);
            //RotationHelper.VelocityInCurrentDirection(this, 750, 0);
            //Velocity += new Vector3(GM.r.FloatBetween(-20, 20), GM.r.FloatBetween(-20, 20), 0);
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
            //Timer.KillAfter(5f + fireDelay);
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
                //Splash particle
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
                    new SmokeParticle(Position2D, Velocity/100, new Vector2(RotationHelper.MyDirection(this, 0).X, RotationHelper.MyDirection(this, 0).Y), 5);
                }

                velApplied = true;
            }
        }

        private void Hit(Sprite hit)
        {

        }

        private void AfterHit(Sprite hit)
        {
            RotationHelper.FaceVelocity(this, DirectionAccuracy.free, false, 0f);
            splash = false;
        }
    }
}