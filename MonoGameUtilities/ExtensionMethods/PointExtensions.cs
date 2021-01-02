using Microsoft.Xna.Framework;
using Zen.Hexagons;
using Zen.Utilities;

namespace Zen.MonoGameUtilities.ExtensionMethods
{
    public static class PointExtensions
    {
        public static PointI ToPointI(this Point p)
        {
            var point = new PointI(p.X, p.Y);

            return point;
        }

        public static bool IsWithinRectangle(this Point p1, Rectangle rectangle)
        {
            var isWithinRectangle = rectangle.Contains(p1);

            return isWithinRectangle;
        }

        public static bool IsWithinRectangle(this Point p1, Rectangle rectangle, Matrix transform)
        {
            var worldPositionPointedAtByMouseCursor = p1.ToWorldPosition(transform);
            var worldRectangle = rectangle.Transform(transform);
            var isWithinRectangle = worldRectangle.Contains(worldPositionPointedAtByMouseCursor);

            return isWithinRectangle;
        }

        public static bool IsWithinHex(this Point p1, PointI p2, Matrix transform, HexLibrary hexLibrary)
        {
            var hexOffsetCoordinates = new HexOffsetCoordinates(p2.X, p2.Y);
            var isWithinHex = p1.IsWithinHex(hexOffsetCoordinates, transform, hexLibrary);

            return isWithinHex;
        }

        public static bool IsWithinHex(this Point p1, HexOffsetCoordinates p2, Matrix transform, HexLibrary hexLibrary)
        {
            var hexPoint = p1.ToWorldHex(transform, hexLibrary);
            var p3 = new PointI(p2.Col, p2.Row);
            var isWithinHex = p3.X == hexPoint.X && p3.Y == hexPoint.Y;

            return isWithinHex;
        }

        public static PointI ToWorldHex(this Point p, Matrix transform, HexLibrary hexLibrary)
        {
            var worldPositionPointedAtByMouseCursor = p.ToWorldPosition(transform);
            var worldHex = hexLibrary.FromPixelToOffsetCoordinates((int)worldPositionPointedAtByMouseCursor.X, (int)worldPositionPointedAtByMouseCursor.Y);
            var worldHexPoint = new PointI(worldHex.Col, worldHex.Row);

            return worldHexPoint;
        }

        public static Vector2 ToWorldPosition(this Point p, Matrix transform)
        {
            var worldPositionPointedAtByMouseCursor = Vector2.Transform(p.ToVector2(), Matrix.Invert(transform));
            var worldPositionPointedAtByMouseCursorVector = new Vector2(worldPositionPointedAtByMouseCursor.X, worldPositionPointedAtByMouseCursor.Y);

            return worldPositionPointedAtByMouseCursorVector;
        }
    }
}