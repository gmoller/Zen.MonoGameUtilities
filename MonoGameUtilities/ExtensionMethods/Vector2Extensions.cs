using Microsoft.Xna.Framework;
using Zen.Hexagons;
using Zen.Utilities;

namespace Zen.MonoGameUtilities.ExtensionMethods
{
    public static class Vector2Extensions
    {
        public static PointI ToPointI(this Vector2 v)
        {
            return new PointI((int)v.X, (int)v.Y);
        }

        public static Point2F ToPointF(this Vector2 v)
        {
            return new Point2F(v.X, v.Y);
        }
    }
}