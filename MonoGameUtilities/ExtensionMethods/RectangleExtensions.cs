using Microsoft.Xna.Framework;

namespace Zen.MonoGameUtilities.ExtensionMethods
{
    public static class RectangleExtensions
    {
        public static Rectangle Transform(this Rectangle rectangle, Matrix transform)
        {
            var loc = rectangle.Location.ToWorldPosition(transform);
            var bottomRight = new Point(rectangle.Right, rectangle.Bottom);
            var foo = bottomRight.ToWorldPosition(transform);
            var transformedRectangle = new Rectangle((int)loc.X, (int)loc.Y, (int)(foo.X - loc.X), (int)(foo.Y - loc.Y));

            return transformedRectangle;
        }
    }
}