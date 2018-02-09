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
        private int variable;
        private int mul;

        /// <summary>
        /// TextButton inherits from Button and is used to display text rather than a sprite
        /// </summary>
        /// <param name="position">Position for button</param>
        /// <param name="inc">True if used to increment, false if used to decrement</param>
        /// <param name="variable">Variable to increment/decrement</param>
        public IncrementButton(Vector2 position, bool inc, ref int variable) : base(new Rectangle((int)position.X, (int)position.Y, 25, 25), "-")
        {
            mul = -1;
            if (inc)
            {
                mul = 1;
                SetText("+");
            }
            UpdateCallBack += tick;
        }

        private void tick()
        {
            if (PressedLeft())
            {
                if (GM.inputM.KeyDown(Keys.LeftControl))
                {
                    variable += mul;
                }
                else if (GM.inputM.KeyDown(Keys.LeftShift))
                {
                    variable += 100 * mul;
                }
                else
                {
                    variable += 10 * mul;
                }
            }
        }
    }
}