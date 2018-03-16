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
    /// <summary>
    /// Sprite used to display a path.
    /// </summary>
    internal class Arrow : Sprite
    {
        /// <summary>
        /// Location to point to
        /// </summary>
        private Vector2 target;

        public Vector2 Target
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
            }
        }

        /// <summary>
        /// Constructor for Rope
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="Target"></param>
        public Arrow(Vector2 origin, Vector2 Target)
        {
            target = Target;

            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            SX = 2;
            Wash = Color.Beige;
            Alpha = 0.75f;

            SY = Vector2.Distance(origin, target);
            RotationAngle = RotationHelper.AngleFromDirection(Vector2.Normalize(target - origin));
            Position2D = origin - ((origin - target) * 0.5f);
        }
    }
}