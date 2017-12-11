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
    /// Rope sprite used for boarding
    /// </summary>
    internal class Rope : Sprite
    {
        Sprite origin;
        Sprite target;

        /// <summary>
        /// Constructor for Rope
        /// </summary>
        /// <param name="Origin"></param>
        /// <param name="Target"></param>
        public Rope(Sprite Origin, Sprite Target)
        {
            origin = Origin;
            target = Target;
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            SX = 2;
            Wash = Color.Brown;
            UpdateCallBack += Tick;
        }

        /// <summary>
        /// Code to run each tick.
        /// </summary>
        private void Tick()
        {
            SY = Vector2.Distance(origin.Position2D, target.Position2D);
            RotationAngle = RotationHelper.AngleFromDirection(Vector2.Normalize(target.Position2D - origin.Position2D));
            Position2D = origin.Position2D - ((origin.Position2D - target.Position2D) * 0.5f);
        }
    }
}