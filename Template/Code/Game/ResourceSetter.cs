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
    class ResourceSetter : Sprite
    {
        string ResourceName;

        public ResourceSetter(int x, int y, string resourceName)
        {
            ResourceName = resourceName;
            Position2D = new Vector2(x, y);
            new IncrementButton(new Vector2(x + 20, y -20), true, ResourceName);
            new IncrementButton(new Vector2(x + 20, y + 20), false, ResourceName);

            GM.engineM.AddSprite(this);
            UpdateCallBack += Tick;
        }

        private void Tick()
        {
            //Get property
            System.Reflection.PropertyInfo property = GM.active.GetType().GetProperty(ResourceName);
            GM.textM.Draw(FontBank.arcadePixel, ResourceName + ": " + (int)property.GetValue(GM.active, null), X, Y, TextAtt.Right);
        }
    }
}
