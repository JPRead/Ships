using Engine7;

namespace Template
{
    internal class Marker : MenuTag
    {
        public Marker()
        {
            boss = new Sprite();
            boss.Frame.Define(Tex.Rectangle50by50);
            Align = Align.centre;
        }

    }
}