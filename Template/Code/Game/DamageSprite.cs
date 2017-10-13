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
    internal class DamageSprite : Sprite
    {
        /// <summary>
        /// HitBox to represent
        /// </summary>
        private HitBox target;
        /// <summary>
        /// Variable used to store target's health
        /// </summary>
        private int health;

        /// <summary>
        /// Constructor for damageSprite
        /// </summary>
        /// <param name="position">Coordinates of centre of sprite</param>
        /// <param name="dimensions">X and Y scale</param>
        /// <param name="hitbox">Hitbox to target</param>
        public DamageSprite(Vector2 position, Vector2 dimensions, HitBox hitbox)
        {
            target = hitbox;

            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            Position2D = position;
            SX = dimensions.X;
            SY = dimensions.Y;
            Wash = new Color(0, 255, 0);
            Layer += 2;
            TimerInitialise();

            UpdateCallBack += Tick;
        }

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Tick()
        {
            health = target.Health;

            //Health can't be less than or equal to zero
            bool destroyed = false;
            if (health <= 0)
            {
                destroyed = true;
                health = 1;
            }
            float colorMul = 0.01f * health;
            Wash.G = (byte)(colorMul * 250);
            Wash.R = (byte)(255 - Wash.G);
            Wash.B = 0;

            if (destroyed)
            {
               Timer.FlashContinuous(0.5f, 0.5f);
            }
            else
            {
                Timer.Off(true);
            }
        }
    }
}