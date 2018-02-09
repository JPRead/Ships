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
    internal class IncrementButton : TextButton
    {
        private string propertyName;
        private int pos;

        /// <summary>
        /// TextButton inherits from Button and is used to display text rather than a sprite
        /// </summary>
        /// <param name="position">Position for button</param>
        /// <param name="inc">True if used to increment, false if used to decrement</param>
        /// <param name="PropertyName">Name of property to increment/decrement</param>
        public IncrementButton(Vector2 position, bool inc, string PropertyName) : base(new Rectangle((int)position.X, (int)position.Y, 25, 25), "-")
        {
            propertyName = PropertyName;
            pos = -1;
            if (inc)
            {
                pos = 1;
                SetText("+");
            }
            UpdateCallBack += tick;
        }

        private void tick()
        {
            if (PressedLeft())
            {
                int mul = 10;
                if (GM.inputM.KeyDown(Keys.LeftControl))
                    mul = 1;
                if (GM.inputM.KeyDown(Keys.LeftShift))
                    mul = 100;

                //Set property
                System.Reflection.PropertyInfo property = GM.active.GetType().GetProperty(propertyName);
                property.SetValue(GM.active, (int)property.GetValue(GM.active, null) + (pos * mul), null);
            }
        }
    }
}