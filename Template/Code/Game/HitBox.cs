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
    internal class HitBox : Sprite
    {
        bool isParent;
        bool isBurning;
        private Sprite owner;
        private Vector2 offset;
        private int damageType;
        private float offsetMagnitude;
        private float offsetAngle;
        private int health;
        private float damageMul;
        private Event tiBurnTick;
        

        internal Sprite Owner
        {
            get
            {
                return owner;
            }
        }

        internal int Health
        {
            get
            {
                return health;
            }

            set
            {
                health = value;
            }
        }

        public int DamageType
        {
            get
            {
                return damageType;
            }
        }

        public bool IsParent
        {
            get
            {
                return isParent;
            }

            set
            {
                isParent = value;
            }
        }

        public float DamageMul
        {
            get
            {
                return damageMul;
            }
        }

        public bool IsBurning
        {
            get
            {
                return isBurning;
            }

            set
            {
                isBurning = value;
            }
        }

        /// <summary>
        /// Constructor for hitbox
        /// </summary>
        /// <param name="hitBoxOwner">Parent of the hitbox</param>
        /// <param name="offsetFromPlayer">Offset from player centre when facing up - this can never be zero!</param>
        /// <param name="dimensions">Height and Width of hitbox</param>
        /// <param name="type">Type of damage taken by hitbox - 0 hull, 1 sail</param>
        public HitBox(Sprite hitBoxOwner, Vector2 offsetFromPlayer, Vector2 dimensions, float damageMultiplier, int type)
        {
            damageMul = damageMultiplier;
            damageType = type;
            health = 100;
            owner = hitBoxOwner;

            offset = new Vector2(offsetFromPlayer.X, -offsetFromPlayer.Y);
            offsetMagnitude = offset.Length();
            offset.Normalize();
            offsetAngle = RotationHelper.AngleFromDirection(offset);

            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            SpriteHelper.ScaleToThisSize(this, dimensions.X, dimensions.Y);

            if (owner is HitBox)
            {
                isParent = false;
                CollisionActive = true;
                Visible = false;

                //Debug
                //CollisionBoxVisible = true;
                //Visible = true;
            }
            else
            {
                isParent = true;
                CollisionActive = false;
                Visible = true;
                Wash = Color.Red;
                Alpha = 0.75f;
                WashCollision = Color.Red;
                CreateCollisionArea(5);
            }

            GM.eventM.AddTimer(tiBurnTick = new Event(1, "Burn Tick"));
            UpdateCallBack += Move;
        }

        /// <summary>
        /// Creates collisions for hitbox that can effectively rotate
        /// </summary>
        /// <param name="colResolution"></param>
        private void CreateCollisionArea(float colResolution)
        {
            for(int xOffset = 0; xOffset <= SX; xOffset += (int)colResolution)
            {
                for (int yOffset = 0; yOffset <= SY; yOffset += (int)colResolution)
                {
                    new HitBox(this, new Vector2(Position2D.X - SX / 2 + xOffset, Position2D.Y - SY / 2 + yOffset), new Vector2(colResolution, colResolution), damageMul,  damageType);
                }
            }
        }

        private void Move()
        {
            offset = RotationHelper.Direction2DFromAngle(offsetAngle, owner.RotationAngle);
            offset = offset * offsetMagnitude;
            Position2D = owner.Position2D + offset;
            RotationAngle = owner.RotationAngle;

            //Burn damage
            if(isParent && isBurning && GM.eventM.Elapsed(tiBurnTick))
            {
                health--;
                for(int i = 0; i < GM.r.FloatBetween(1,2); i++)
                {
                    FadingParticle smokeParticle = new FadingParticle(new Vector2(Centre2D.X + GM.r.FloatBetween(-2, 2), Centre2D.Y + GM.r.FloatBetween(-2, 2)),
                        new Vector3(GM.r.FloatBetween(-4, 4), GM.r.FloatBetween(-4, 4), 0),
                        GM.r.FloatBetween(0, 360), GM.r.FloatBetween(5, 10));
                    smokeParticle.Wash = Color.DarkSlateGray;
                }
                if (GM.r.FloatBetween(0, 1) > 0.90)
                {
                    Ship ship = (Ship)owner;
                    ship.CrewNum -= 1;
                }
            }
        }
    }
}