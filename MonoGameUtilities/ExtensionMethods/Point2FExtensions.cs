using Microsoft.Xna.Framework;
using Zen.Utilities;

namespace Zen.MonoGameUtilities.ExtensionMethods
{
    public static class Point2FExtensions
    {
        public static Vector2 ToVector2(this PointI p)
        {
            return new Vector2(p.X, p.Y);
        }
    }
}