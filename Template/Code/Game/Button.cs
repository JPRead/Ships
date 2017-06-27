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


namespace Template.Game
{
    internal class Button : Sprite
    {
        private Sprite display;
        public Sprite Display
        {
            get
            {
                return display;
            }

            set
            {
                display = value;
            }
        }

        public Button(Rectangle rect)
        {
            GM.engineM.AddSprite(this);
            WorldCoordinates = false;
            Frame.Define(Tex.SingleWhitePixel);
            CollisionBoxVisible = true;
            Shape = Shape.rectangle;
            SpriteHelper.ScaleToThisSize(this, rect);
            X = rect.X;
            Y = rect.Y;

            display = new Sprite();
            GM.engineM.AddSprite(display);
            display.Frame.Define(GM.txSprite, new Rectangle(1,1,1,1));
            display.Position2D = Centre2D;

            UpdateCallBack += Tick;
        }

        private void Tick()
        {
            if (PressedLeft())
            {
                Wash = Color.Red;
            }
            else
            {
                Wash = Color.Blue;
            }
        }

        /// <summary>
        /// Returns true if left clicked
        /// </summary>
        /// <returns></returns>
        internal bool PressedLeft()
        {
            if (GM.inputM.MouseLeftButtonHeld())
            {
                Vector2 mouseLoc = GM.inputM.MouseLocation;
                
                if(mouseLoc.X > Sides.X && mouseLoc.X < Sides.Y &&
                    mouseLoc.Y > Sides.Z && mouseLoc.Y < Sides.W)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if right clicked
        /// </summary>
        /// <returns></returns>
        internal bool PressedRight()
        {
            if (GM.inputM.MouseRightButtonHeld())
            {
                Vector2 mouseLoc = GM.inputM.MouseLocation;

                if (mouseLoc.X > Sides.X && mouseLoc.X < Sides.Y &&
                    mouseLoc.Y > Sides.Z && mouseLoc.Y < Sides.W)
                {
                    return true;
                }
            }
            return false;
        }

        internal void SetDisplay(Rectangle tile)
        {
            display = new Sprite();
            GM.engineM.AddSprite(display);
            display.Frame.Define(GM.txSprite, tile);
            display.Position2D = Centre2D;
            
        }
    }
}
