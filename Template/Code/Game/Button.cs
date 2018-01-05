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
        /// <summary>
        /// Sprite to display within the button
        /// </summary>
        private Sprite display;
        /// <summary>
        /// True if UI element can be clicked
        /// </summary>
        private bool enabled;
        /// <summary>
        /// True if UI element needs to be faded out
        /// </summary>
        private bool faded;
        /// <summary>
        /// Description of primary function
        /// </summary>
        private string priTooltip;
        /// <summary>
        /// Description of secondary function
        /// </summary>
        private string secTooltip;
        /// <summary>
        /// Shortcut for primary function
        /// </summary>
        private Shortcut priShortcut;
        /// <summary>
        /// Shortcut for secondary function
        /// </summary>
        private Shortcut secShortcut;


        public bool Faded
        {
            get
            {
                return faded;
            }

            set
            {
                faded = value;
            }
        }

        /// <summary>
        /// Button is used as a UI element that can be interacted with using the mouse
        /// </summary>
        /// <param name="rect">Dimensions for button</param>
        /// <param name="startEnabled">Is the button enabled to begin with - set to false for UI backgrounds</param>
        /// <param name="primaryTooltip">"Description of primary function"</param>
        /// <param name="primaryShortcut">"Shortcut for primary function"</param>
        /// <param name="secondaryTooltip">"Description of secondary function"</param>
        /// <param name="secondaryShortcut">"Shortcut for secondary function"</param>
        public Button(Rectangle rect, bool startEnabled, string primaryTooltip = "", Shortcut primaryShortcut = null, string secondaryTooltip = "", Shortcut secondaryShortcut = null)
        {
            GM.engineM.AddSprite(this);

            //Init values
            enabled = startEnabled;
            priTooltip = primaryTooltip;
            secTooltip = secondaryTooltip;
            priShortcut = primaryShortcut;
            secShortcut = secondaryShortcut;
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
            Layer++;

            if (enabled)
            {
                Layer++;
                display.Layer += 2 ;
            }

            UpdateCallBack += Tick;
        }

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Tick()
        {
            if (enabled && !faded)
            {
                if (HeldLeft() || HeldRight())
                {
                    Wash = Color.Red;
                }
                else
                {
                    Wash = Color.Blue;
                }
            }
            if (faded)
            {
                Wash = Color.Black;
            }
        }

        /// <summary>
        /// Returns true if left clicked
        /// </summary>
        /// <returns></returns>
        internal bool PressedLeft()
        {
            if (GM.inputM.MouseLeftButtonPressed())
            {
                Vector2 mouseLoc = GM.inputM.MouseLocation;
                
                if(mouseLoc.X > Sides.X && mouseLoc.X < Sides.Y &&
                    mouseLoc.Y > Sides.Z && mouseLoc.Y < Sides.W)
                {
                    GameSetup.Player.ButtonPressed = true;
                    return true;
                }
            }
            if (priShortcut != null && priShortcut.Pressed())
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if left held
        /// </summary>
        /// <returns></returns>
        internal bool HeldLeft()
        {
            if (GM.inputM.MouseLeftButtonHeld())
            {
                Vector2 mouseLoc = GM.inputM.MouseLocation;

                if (mouseLoc.X > Sides.X && mouseLoc.X < Sides.Y &&
                    mouseLoc.Y > Sides.Z && mouseLoc.Y < Sides.W)
                {
                    GameSetup.Player.ButtonPressed = true;
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
            if (GM.inputM.MouseRightButtonPressed())
            {
                Vector2 mouseLoc = GM.inputM.MouseLocation;

                if (mouseLoc.X > Sides.X && mouseLoc.X < Sides.Y &&
                    mouseLoc.Y > Sides.Z && mouseLoc.Y < Sides.W)
                {
                    GameSetup.Player.ButtonPressed = true;
                    return true;
                }
            }
            if (secShortcut != null && secShortcut.Pressed())
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if right held
        /// </summary>
        /// <returns></returns>
        internal bool HeldRight()
        {
            if (GM.inputM.MouseRightButtonHeld())
            {
                Vector2 mouseLoc = GM.inputM.MouseLocation;

                if (mouseLoc.X > Sides.X && mouseLoc.X < Sides.Y &&
                    mouseLoc.Y > Sides.Z && mouseLoc.Y < Sides.W)
                {
                    GameSetup.Player.ButtonPressed = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if mouse is hovered over button
        /// </summary>
        /// <returns></returns>
        internal bool Hover()
        {
            Vector2 mouseLoc = GM.inputM.MouseLocation;

            if (mouseLoc.X > Sides.X && mouseLoc.X < Sides.Y &&
                mouseLoc.Y > Sides.Z && mouseLoc.Y < Sides.W)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Displays a rectangle in Sprites.png within the button
        /// </summary>
        /// <param name="tile">Rectangle to select from tilemap</param>
        internal void SetDisplay(Rectangle tile)
        {
            display.Kill();
            display = new Sprite();
            GM.engineM.AddSprite(display);
            display.Frame.Define(GM.txSprite, tile);
            display.WorldCoordinates = false;
            display.Position2D = Centre2D;
            if (enabled)
            {
                display.Layer += 2;
            }
        }
    }
}
