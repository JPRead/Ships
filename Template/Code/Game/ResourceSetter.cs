using Microsoft.Xna.Framework;
using Engine7;

namespace Template.Game
{
    class ResourceSetter : Sprite
    {
        private string ResourceName;
        private string DisplayText;

        public ResourceSetter(int x, int y, string resourceName, string displayText)
        {
            ResourceName = resourceName;
            DisplayText = displayText;
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
            GM.textM.Draw(FontBank.arcadePixel, DisplayText + ": " + (int)property.GetValue(GM.active, null), X, Y, TextAtt.Right);
        }
    }
}
