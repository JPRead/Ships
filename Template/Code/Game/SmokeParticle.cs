using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine7;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Template.Game;

namespace Template
{
    internal class SmokeParticle : Sprite
    {
        Event tiLifetime;

        /// <summary>
        /// Constructor for smoke particle
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="spawnVel"></param>
        /// <param name="spawnAngle"></param>
        /// <param name="lifetime">Minimum lifetime for particle in seconds</param>
        public SmokeParticle(Vector2 spawnPos, Vector3 spawnVel, float spawnAngle, float lifetime)
        {
            GM.eventM.AddEvent(tiLifetime = new Event(lifetime + GM.r.FloatBetween(0,2), "Lifetime Counter"));

            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            ScaleBoth = 10;
            Wash = Color.WhiteSmoke;
            
            Velocity = spawnVel;

            float rotRan = GM.r.FloatBetween(-10f, 10f);

            Vector3 spawnRot = RotationHelper.Direction3DFromAngle(spawnAngle, 0);
            RotationHelper.FaceDirection(this, spawnRot, DirectionAccuracy.free, rotRan);

            Position2D = spawnPos;
            float xRan = GM.r.FloatBetween(-10f, 10f);
            float yRan = GM.r.FloatBetween(-10f, 10f);
            Position2D += new Vector2(xRan, yRan);

            UpdateCallBack += LifeCountdown;
        }

        private void LifeCountdown()
        {
            //Delete after lifeTime runs out
            if (GM.eventM.Elapsed(tiLifetime))
            {
                Kill();
            }

            //Fade over time
            Alpha = 1-(tiLifetime.ElapsedSoFar/tiLifetime.Interval);
        }
    }
}