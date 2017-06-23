using Engine7;
using Microsoft.Xna.Framework;

namespace Template
{
    internal class MenuInitial : MenuItem
    {
        private Sprite sprite;

        public MenuInitial(Sprite sprite)
            :base(sprite.Text, sprite)
        {
            this.sprite = sprite;
        }

        public override void Select()
        {
            sprite.Wash = Color.Red;
            sprite.Z = 1000;
            sprite.Y = GM.screenSize.Bottom - 150;
            base.Select();
        }

        public override void Deselect(bool show)
        {
            sprite.Wash = Color.White;
            sprite.Z = 100;
            sprite.Y = GM.screenSize.Bottom - 100;
            base.Deselect(show);
        }
    }
}