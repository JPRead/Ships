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
        private Ship parent;
        private Vector2 offset;
        private int damageType;
        private float offsetMagnitude;
        private float offsetAngle;
        private int health;

        internal Ship Parent
        {
            get
            {
                return parent;
            }

            //set
            //{
            //    parent = value;
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

        /// <summary>
        /// Constructor for hitbox
        /// </summary>
        /// <param name="owner">Parent of the hitbox</param>
        /// <param name="offsetFromPlayer">Offset from player centre when facing up - this can never be zero!</param>
        /// <param name="dimensions">Height and Width of hitbox</param>
        /// <param name="type">Type of damage taken by hitbox - 0 hull, 1 sail</param>
        public HitBox(Ship owner, Vector2 offsetFromPlayer, Vector2 dimensions, int type)
        {
            damageType = type;
            health = 100;
            parent = owner;

            offset = new Vector2(offsetFromPlayer.X, -offsetFromPlayer.Y);
            offsetMagnitude = offset.Length();
            offset.Normalize();
            offsetAngle = RotationHelper.AngleFromDirection(offset);

            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            CollisionBoxVisible = true;
            CollisionActive = true;
            Wash = Color.Red;
            Alpha = 0.25f;
            WashCollision = Color.Red;
            SpriteHelper.ScaleToThisSize(this, dimensions.X, dimensions.Y);
            Visible = true;

            UpdateCallBack += Move;
        }

        private void Move()
        {
            offset = RotationHelper.Direction2DFromAngle(offsetAngle, parent.RotationAngle);
            offset = offset * offsetMagnitude;
            Position2D = parent.Position2D + offset;
            RotationAngle = parent.RotationAngle;
            //RotationHelper.FaceDirection(this, RotationHelper.MyDirection(parent, 0), DirectionAccuracy.free, 0);
        }
    }
}