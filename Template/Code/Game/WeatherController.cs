using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Engine7;
using Template.Title;

namespace Template.Game
{
    internal class WeatherController: Sprite
    {
        /// <summary>
        /// 0 if not raining, 1 if full rain
        /// </summary>
        private float rainAmount;
        /// <summary>
        /// The wind direction
        /// </summary>
        private float windDir;
        /// <summary>
        /// Sprite used to display wind direction
        /// </summary>
        private Sprite windDirSprite;
        /// <summary>
        /// Timer for 1 second delay
        /// </summary>
        private Event tiOneSecond;

        public float WindDir
        {
            get
            {
                return windDir;
            }
        }

        public float RainAmount
        {
            get
            {
                return rainAmount;
            }

            //set
            //{
            //    windDir = value;
            //}
        }

        /// <summary>
        /// Constructor for WeatherController
        /// </summary>
        public WeatherController()
        {
            //Init
            GM.eventM.AddTimer(tiOneSecond = new Event(1, "One Second Timer"));
            windDir = GM.r.FloatBetween(0, 360);
            windDirSprite = new Sprite();
            GM.engineM.AddSprite(windDirSprite);
            windDirSprite.Frame.Define(Tex.Triangle);
            windDirSprite.SY = 1.5f;
            windDirSprite.WorldCoordinates = false;
            windDirSprite.Layer++;
            windDirSprite.Position2D = new Vector2(GM.screenSize.Center.X, 50);
            windDirSprite.RotationAngle = windDir;
            rainAmount = 0;
            if (GM.r.FloatBetween(0, 1) > 0.75f)
            {
                rainAmount += GM.r.FloatBetween(0, 1);
            }

            UpdateCallBack += Tick;
        }

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Tick()
        {
            if (GM.eventM.Elapsed(tiOneSecond))
            {
                OneSecond();
            }
        }

        private void OneSecond()
        {
            //Change wind
            if(GM.r.FloatBetween(0, 1) > 0.95f)
            {
                windDir += GM.r.FloatBetween(-20, 20);
                if(windDir > 360)
                {
                    windDir -= 360;
                }
                else if(windDir < 0)
                {
                    windDir += 360;
                }
                windDirSprite.RotationAngle = windDir;
            }

            //Change rain
            if (GM.r.FloatBetween(0, 1) > 0.95f)
            {
                rainAmount += GM.r.FloatBetween(-0.15f, 0.3f);
                if (rainAmount < 0)
                    rainAmount = 0;
                else if (rainAmount > 1)
                    rainAmount = 1;
            }
        }
    }
}
