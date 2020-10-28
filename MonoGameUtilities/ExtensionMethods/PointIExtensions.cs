using Microsoft.Xna.Framework;
using Zen.Utilities;

namespace Zen.MonoGameUtilities.ExtensionMethods
{
    public static class PointIExtensions
    {
        public static Vector2 ToVector2(this PointI p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static Point ToPoint(this PointI p)
        {
            return new Point(p.X, p.Y);
        }
    }
}