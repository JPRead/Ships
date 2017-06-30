using System;
using Engine7;
using Microsoft.Xna.Framework;

namespace Template.Game
{
    internal class CannonBall : Sprite
    {
        private Sprite player;
        private int damage;

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

            //Sound effects
            GM.audioM.PlayEffect("shoot");

            //set postion of bullet and give velocity
            //X = player.Centre.X;
            //Y = player.Centre.Y;

            //Set rotation at player
            //Position2D = RotationHelper.RotateAround(player.Centre2D, player.Centre2D, 0);

            //Create direction vector and normalise
            Vector2 direction = fireTowards - Position2D;
            direction = Vector2.Normalize(direction);

            //Face direction vector
            RotationHelper.FaceDirection(this, direction, DirectionAccuracy.free, 0);
            RotationHelper.VelocityInCurrentDirection(this, 750, 0);
            Velocity += new Vector3(GM.r.FloatBetween(-20, 20), GM.r.FloatBetween(-20, 20), 0);
            Position += RotationHelper.MyDirection(this, 0) * 32;

            //collision setup
            CollisionActive = true;
            CollisionPrimary = true;
            PrologueCallBack += Hit;
            EpilogueCallBack += AfterHit;
            Moving = true;

            //Release smoke
            int rAmount = (int)GM.r.FloatBetween(5, 10);
            for(int i = 0; i < rAmount; i++)
            {
                new SmokeParticle(Position2D, Velocity/100, new Vector2(RotationHelper.MyDirection(this, 0).X, RotationHelper.MyDirection(this, 0).Y), 5);
            }

            //kill after 5 seconds
            TimerInitialise();
            Timer.KillAfter(5f);
        }

        private void Hit(Sprite hit)
        {

        }

        private void AfterHit(Sprite hit)
        {
            RotationHelper.FaceVelocity(this, DirectionAccuracy.free, false, 0f);
        }
    }
}