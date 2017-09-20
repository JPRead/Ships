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
    /// Ship for the AI
    /// </summary>
    internal class Opponent : Ship
    {
        public Opponent(Vector2 startPos)
        {
            moveLocSprite.Visible = false;
            Position2D = startPos;
            RotationAngle = 45;

            UpdateCallBack += Move;
        }

        private void Move()
        {
            //Debug text
            GM.textM.Draw(FontBank.arcadePixel, "Hull Front  " + hitBoxHullFront.Health + "~Hull Back   " + hitBoxHullBack.Health +
                "~Hull Left   " + hitBoxHullLeft.Health + "~Hull Right  " + hitBoxHullRight.Health +
                "~Sail Front  " + hitBoxSailFront.Health + "~Sail Middle " + hitBoxSailMiddle.Health + "~Sail Back   " + hitBoxSailBack.Health, GM.screenSize.Width - 100, 100, TextAtt.TopRight);

            GM.textM.Draw(FontBank.arcadePixel, "Crew: " + CrewNum, GM.screenSize.Width - 150, 50, TextAtt.TopRight);
        }
    }
}