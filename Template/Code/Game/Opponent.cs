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
        /// <summary>
        /// Aggressiveness of the AI. Should be a value between 0 and 1.
        /// </summary>
        private float aggressiveness;
        /// <summary>
        /// Current state of the AI - 0 idle, 1 attacking, 2 retreating, 3 boarding, 4 ramming
        /// </summary>
        private int state;

        /// <summary>
        /// Contains state machine for the AI
        /// </summary>
        /// <param name="startPos">Position to spawn at</param>
        public Opponent(Vector2 startPos)
        {
            //Init values
            Position2D = startPos;
            aggressiveness = 0.5f;

            if (GM.eventM.Elapsed(tiOneSecond))
            {
                state = StateMachine();
            }

            UpdateCallBack += Tick;
            FuneralCallBack += Death;
        }

        private int StateMachine()
        {
            if(state == 0)
                return 1;

            if(state == 1)
            {
                float playerHealth = 0;
                float totalHealth = 0;
                for(int i = 0; i <= 6; i++)
                {
                    playerHealth += GameSetup.Player.hitBoxArray[i].Health;
                    totalHealth += hitBoxArray[i].Health;
                }
                if (totalHealth < playerHealth - (200 * aggressiveness))
                    return 2;
                if(CrewNum > GameSetup.Player.CrewNum + (50 * (1 - aggressiveness)) + 10)
                    return 3;
            }
            if(state == 2)
            {
                float totalHealth = 0;
                for (int i = 0; i <= 6; i++)
                {
                    totalHealth += hitBoxArray[i].Health;
                }
                if (totalHealth >= 600 + 100 * aggressiveness)
                    return 0;
            }
            if(state == 3)
            {
                //Leave this state when boarding is complete
            }
            return 0;
        }

        private void Death()
        {
            GameSetup.BackToTitle("You win.");
        }

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Tick()
        {
            //Debug text
            GM.textM.Draw(FontBank.arcadePixel, "Hull Front  " + hitBoxHullFront.Health + "~Hull Back   " + hitBoxHullBack.Health +
                "~Hull Left   " + hitBoxHullLeft.Health + "~Hull Right  " + hitBoxHullRight.Health +
                "~Sail Front  " + hitBoxSailFront.Health + "~Sail Middle " + hitBoxSailMiddle.Health + "~Sail Back   " + hitBoxSailBack.Health, GM.screenSize.Width - 100, 100, TextAtt.TopRight);
            GM.textM.Draw(FontBank.arcadePixel, "Crew: " + CrewNum, GM.screenSize.Width - 150, 50, TextAtt.TopRight);

            Fire(true, 0);
        }
    }
}