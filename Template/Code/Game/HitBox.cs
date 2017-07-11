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
        private Sprite owner;
        private Vector2 offset;
        private int damageType;
        private float offsetMagnitude;
        private float offsetAngle;
        private int health;

        internal Sprite Owner
        {
            get
            {
                return owner;
            }

            //set
            //{
            //    owner = value;
            //}
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

            //set
            //{
            //    damageType = value;
            //}
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

        /// <summary>
        /// Constructor for hitbox
        /// </summary>
        /// <param name="hitBoxOwner">Parent of the hitbox</param>
        /// <param name="offsetFromPlayer">Offset from player centre when facing up - this can never be zero!</param>
        /// <param name="dimensions">Height and Width of hitbox</param>
        /// <param name="type">Type of damage taken by hitbox - 0 hull, 1 sail</param>
        public HitBox(Sprite hitBoxOwner, Vector2 offsetFromPlayer, Vector2 dimensions, int type)
        {

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
                CollisionBoxVisible = true;
                Visible = true;
            }
            else
            {
                isParent = true;
                CollisionActive = false;
                Visible = true;
                Wash = Color.Red;
                Alpha = 0.25f;
                WashCollision = Color.Red;
                CreateCollisionArea(5);
            }
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
                    new HitBox(this, new Vector2(Position2D.X - SX / 2 + xOffset, Position2D.Y - SY / 2 + yOffset), new Vector2(colResolution, colResolution), damageType);
                }
            }
        }

        private void Move()
        {
            offset = RotationHelper.Direction2DFromAngle(offsetAngle, owner.RotationAngle);
            offset = offset * offsetMagnitude;
            Position2D = owner.Position2D + offset;
            RotationAngle = owner.RotationAngle;
            //RotationHelper.FaceDirection(this, RotationHelper.MyDirection(parent, 0), DirectionAccuracy.free, 0);
        }
    }
}