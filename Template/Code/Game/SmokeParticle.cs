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

        public SmokeParticle(Vector2 spawnPos, Vector3 spawnVel, Vector2 spawnRot, float lifetime)
        {
            GM.eventM.AddEvent(tiLifetime = new Event(lifetime, "Lifetime Counter"));

            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            ScaleBoth = 10;
            Wash = Color.WhiteSmoke;

            float xRan = GM.r.FloatBetween(-10, 10);
            float yRan = GM.r.FloatBetween(-10, 10);
            RotationHelper.VelocityInThisDirection(this, new Vector3(spawnVel.X + xRan, spawnVel.Y + yRan, 0), 100);

            float rotRan = GM.r.FloatBetween(-10, 10);
            RotationHelper.FaceDirection(this, spawnRot, DirectionAccuracy.free, rotRan);

            Position2D = spawnPos;

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