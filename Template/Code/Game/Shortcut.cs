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
using Template.Title;

namespace Template
{
    /// <summary>
    /// Data structure for multiple keys
    /// </summary>
    internal class Shortcut
    {
        /// <summary>
        /// Key to use for shortcut
        /// </summary>
        private Keys key1;
        /// <summary>
        /// Optional second key
        /// </summary>
        private Keys key2;
        /// <summary>
        /// Text for tooltips
        /// </summary>
        private string displayKeys;

        public string DisplayKeys
        {
            get
            {
                return displayKeys;
            }

            set
            {
                displayKeys = value;
            }
        }

        public Shortcut(Keys shortcutKey1, Keys shortcutKey2 = Keys.None)
        {
            key1 = shortcutKey1;
            key2 = shortcutKey2;

            displayKeys = shortcutKey1.ToString();
            if(shortcutKey2 != Keys.None)
            {
                displayKeys += "+" + shortcutKey2.ToString();
            }
        }
        internal bool Pressed()
        {
            if ((GM.inputM.KeyHeld(key1) && (key2 == Keys.None || GM.inputM.KeyPressed(key2))) || (GM.inputM.KeyPressed(key1) && (key2 == Keys.None || GM.inputM.KeyHeld(key2))))
            {
                return true;
            }
            return false;
        }
    }
}