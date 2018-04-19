using Microsoft.Xna.Framework;
using Engine7;
using Template.Game;

namespace Template
{
    internal class TextButton : Button
    {
        private string displayText;
        /// <summary>
        /// TextButton inherits from Button and is used to display text rather than a sprite
        /// </summary>
        /// <param name="rect">Dimensions for button</param>
        /// <param name="text">Text to display within the button</param>
        public TextButton(Rectangle rect, string text) : base(rect, true)
        {
            displayText = text;
            UpdateCallBack += Display;
        }

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Display()
        {
            GM.textM.Draw(FontBank.arcadePixel, displayText, Centre2D.X, Centre2D.Y, TextAtt.Centred);
        }

        /// <summary>
        /// Changes the text displayed
        /// </summary>
        /// <param name="text">Text to display</param>
        internal void SetText(string text)
        {
            displayText = text;
        }
    }
}