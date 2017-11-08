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
        /// <summary>
        /// True if HitBox is parent of other HitBoxes
        /// </summary>
        bool isParent;
        /// <summary>
        /// True if HitBox is burning, causing damage over time
        /// </summary>
        bool isBurning;
        /// <summary>
        /// Owner of HitBox, for child HitBoxes this is the parent HitBox, for parent HitBoxes this is the Ship it is created by
        /// </summary>
        private Sprite owner;
        /// <summary>
        /// Type of damage taken by hitbox - 0 hull, 1 sail
        /// </summary>
        private int damageType;
        /// /// <summary>
        /// Normalised offset vector from centre of owner
        /// </summary>
        private Vector2 offsetVector;
        /// <summary>
        /// Value to multiply offsetVector by to get a position
        /// </summary>
        private float offsetMagnitude;
        /// <summary>
        /// Extra rotation for offsetVector to account for owner turning
        /// </summary>
        private float offsetAngle;
        /// <summary>
        /// Health of HitBox
        /// </summary>
        private int health;
        /// <summary>
        /// Value to multiply damage by when subtracting from health
        /// </summary>
        private float damageMul;
        /// <summary>
        /// Delay for damage over time when burning
        /// </summary>
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
        /// <param name="offsetFromOwner">Offset from owner centre when facing up - this can never be zero!</param>
        /// <param name="dimensions">Height and Width of hitbox</param>
        /// <param name="damageMultiplier">Damage taken is multiplied by this amount</param>
        /// <param name="type">Type of damage taken by hitbox - 0 hull, 1 sail</param>
        public HitBox(Sprite hitBoxOwner, Vector2 offsetFromOwner, Vector2 dimensions, float damageMultiplier, int type)
        {
            damageMul = damageMultiplier;
            damageType = type;
            health = 100;
            owner = hitBoxOwner;
            
            offsetVector = new Vector2(offsetFromOwner.X, -offsetFromOwner.Y);
            offsetMagnitude = offsetVector.Length();
            offsetVector.Normalize();
            offsetAngle = RotationHelper.AngleFromDirection(offsetVector);

            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            SpriteHelper.ScaleToThisSize(this, dimensions.X, dimensions.Y);

            if (owner is HitBox)
            {
                isParent = false;
                CollisionActive = true;
                CollisionPrimary = true;
                Visible = false;

                //Debug
                //CollisionBoxVisible = true;
                //Visible = true;
            }
            else
            {
                isParent = true;
                CollisionActive = false;
                CollisionPrimary = false;
                Visible = true;
                Wash = Color.Red;
                Alpha = 0.75f;
                WashCollision = Color.Red;
                CreateCollisionArea(10);
            }

            GM.eventM.AddTimer(tiBurnTick = new Event(1, "Burn Tick"));
            PrologueCallBack += Hit;
            UpdateCallBack += Tick;
        }

        /// <summary>
        /// Code to run upon hitting a sprite
        /// </summary>
        /// <param name="hit">The sprite collided with</param>
        private void Hit(Sprite hit)
        {
            if (hit is HitBox)
            {
                //Get Ship of collided HitBox, then Ship of this HitBox
                HitBox hitBox = (HitBox)hit;
                HitBox hitBoxOwner = (HitBox)hitBox.Owner;
                Ship hitShip = (Ship)hitBoxOwner.Owner;
                hitBoxOwner = (HitBox)owner;
                Ship thisShip = (Ship)hitBoxOwner.Owner;
                if (thisShip == hitShip)
                {
                    CollisionAbandonResponse = true;
                }
                else if(damageType == hitBox.DamageType)
                {
                    thisShip.hasCollided = true;
                    hitBoxOwner.Health-= 1;
                }
            }
            else
            {
                CollisionAbandonResponse = true;
            }
        }

        /// <summary>
        /// Creates collisions for HitBox that can approximate a rotation using smaller HitBoxes
        /// </summary>
        /// <param name="colResolution">Resolution of HitBoxes, higher values are more accurate but slower</param>
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

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Tick()
        {
            offsetVector = RotationHelper.Direction2DFromAngle(offsetAngle, owner.RotationAngle);
            offsetVector = offsetVector * offsetMagnitude;
            Position2D = owner.Position2D + offsetVector;
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