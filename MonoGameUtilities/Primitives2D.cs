using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zen.MonoGameUtilities
{
    public static class Primitives2D
    {

        #region Private Members

        private static readonly Dictionary<string, List<Vector2>> CircleCache = new Dictionary<string, List<Vector2>>(); // not thread-safe
        private static readonly Dictionary<string, List<Vector2>> ArcCache = new Dictionary<string, List<Vector2>>(); // not thread-safe
        private static Texture2D _pixel;

        #endregion

        #region Private Methods

        private static void CreateThePixel(SpriteBatch spriteBatch)
        {
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Draws a list of connecting points
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// /// <param name="position">Where to position the points</param>
        /// <param name="points">The points to connect with lines</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// <param name="layerDepth">The layer depth</param>
        private static void DrawPoints(SpriteBatch spriteBatch, Vector2 position, List<Vector2> points, Color color, float thickness, float layerDepth = 0.0f)
        {
            if (points.Count < 2) return;

            for (var i = 1; i < points.Count; i++)
            {
                DrawLine(spriteBatch, points[i - 1] + position, points[i] + position, color, thickness, layerDepth);
            }
        }

        /// <summary>
        /// Creates a list of vectors that represents a circle
        /// </summary>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <returns>A list of vectors that, if connected, will create a circle</returns>
        private static List<Vector2> CreateCircle(float radius, int sides)
        {
            // Look for a cached version of this circle
            var circleKey = $"{radius}.{sides}";
            if (CircleCache.ContainsKey(circleKey))
            {
                return CircleCache[circleKey];
            }

            var points = new List<Vector2>();

            const double max = 2.0 * Math.PI;
            var step = max / sides;

            for (var theta = 0.0; theta < max; theta += step)
            {
                points.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta))));
            }

            // then add the first vector again so it's a complete loop
            points.Add(new Vector2(radius, 0.0f)); //new Vector2((float)(radius * Math.Cos(0)), (float)(radius * Math.Sin(0)))

            // Cache this circle so that it can be quickly drawn next time
            CircleCache.Add(circleKey, points);

            return points;
        }

        /// <summary>
        /// Creates a list of vectors that represents an arc
        /// </summary>
        /// <param name="radius">The radius of the arc</param>
        /// <param name="sides">The number of sides to generate in the circle that this will cut out from</param>
        /// <param name="startingAngle">The starting angle of arc, 0 being to the east, increasing as you go clockwise</param>
        /// <param name="radians">The radians to draw, clockwise from the starting angle</param>
        /// <returns>A list of vectors that, if connected, will create an arc</returns>
        private static List<Vector2> CreateArc(float radius, int sides, float startingAngle, float radians)
        {
            // Look for a cached version of this arc
            var arcKey = $"{radius}.{sides}.{startingAngle}.{radians}";
            if (ArcCache.ContainsKey(arcKey))
            {
                return ArcCache[arcKey];
            }

            var points = new List<Vector2>();
            points.AddRange(CreateCircle(radius, sides));
            points.RemoveAt(points.Count - 1); // remove the last point because it's a duplicate of the first

            // The circle starts at (radius, 0)
            var curAngle = 0.0f;
            var anglePerSide = MathHelper.TwoPi / sides;

            // "Rotate" to the starting point
            while (curAngle + anglePerSide / 2.0f < startingAngle)
            {
                curAngle += anglePerSide;

                // move the first point to the end
                points.Add(points[0]);
                points.RemoveAt(0);
            }

            // Add the first point, just in case we make a full circle
            points.Add(points[0]);

            // Now remove the points at the end of the circle to create the arc
            var sidesInArc = (int)(radians / anglePerSide + 0.5f);
            points.RemoveRange(sidesInArc + 1, points.Count - sidesInArc - 1);

            // Cache this arc so that it can be quickly drawn next time
            ArcCache.Add(arcKey, points);

            return points;
        }

        #endregion

        #region FillRectangle

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float layerDepth)
        {
            FillRectangle(spriteBatch, location, size, color, 0.0f, layerDepth);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The X coordinate of the left side</param>
        /// <param name="y">The Y coordinate of the upper side</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float w, float h, Color color, float layerDepth)
        {
            FillRectangle(spriteBatch, new Vector2(x, y), new Vector2(w, h), color, 0.0f, layerDepth);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The X coordinate of the left side</param>
        /// <param name="y">The Y coordinate of the upper side</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="angle">The angle of the rectangle in radians</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float w, float h, Color color, float angle, float layerDepth)
        {
            FillRectangle(spriteBatch, new Vector2(x, y), new Vector2(w, h), color, angle, layerDepth);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float layerDepth)
        {
            FillRectangle(spriteBatch, rect, color, 0.0f, layerDepth);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="angle">The angle in radians to draw the rectangle at</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float angle, float layerDepth)
        {
            if (_pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            spriteBatch.Draw(_pixel, rect, null, color, angle, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="angle">The angle in radians to draw the rectangle at</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float angle, float layerDepth)
        {
            if (_pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            // stretch the pixel between the two vectors
            spriteBatch.Draw(_pixel, location, null, color, angle, Vector2.Zero, size, SpriteEffects.None, layerDepth);
        }

        #endregion

        #region DrawRectangle

        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float thickness = 1.0f, float layerDepth = 0.0f)
        {
            DrawRectangle(spriteBatch, new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color, thickness, layerDepth);
        }

        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness = 1.0f, float layerDepth = 0.0f)
        {
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.Right, rect.Y), color, thickness, layerDepth); // top
            DrawLine(spriteBatch, new Vector2(rect.X + 1.0f, rect.Y), new Vector2(rect.X + 1.0f, rect.Bottom + thickness), color, thickness, layerDepth); // left
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Bottom), new Vector2(rect.Right, rect.Bottom), color, thickness, layerDepth); // bottom
            DrawLine(spriteBatch, new Vector2(rect.Right + 1.0f, rect.Y), new Vector2(rect.Right + 1.0f, rect.Bottom + thickness), color, thickness, layerDepth); // right
        }

        #endregion

        #region DrawLine

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x1">The X coordinate of the first point</param>
        /// <param name="y1">The Y coordinate of the first point</param>
        /// <param name="x2">The X coordinate of the second point</param>
        /// <param name="y2">The Y coordinate of the second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float layerDepth)
        {
            DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, 1.0f, layerDepth);
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x1">The X coordinate of the first point</param>
        /// <param name="y1">The Y coordinate of the first point</param>
        /// <param name="x2">The X coordinate of the second point</param>
        /// <param name="y2">The Y coordinate of the second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness, float layerDepth)
        {
            DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, thickness, layerDepth);
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float layerDepth)
        {
            DrawLine(spriteBatch, point1, point2, color, 1.0f, layerDepth);
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point">The starting point</param>
        /// <param name="length">The length of the line</param>
        /// <param name="angle">The angle of this line from the starting point in radians</param>
        /// <param name="color">The color to use</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float layerDepth)
        {
            DrawLine(spriteBatch, point, length, angle, color, 1.0f, layerDepth);
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(spriteBatch, point1, distance, angle, color, thickness, layerDepth);
        }

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point">The starting point</param>
        /// <param name="length">The length of the line</param>
        /// <param name="angle">The angle of this line from the starting point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness, float layerDepth)
        {
            if (_pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            // stretch the pixel between the two vectors
            spriteBatch.Draw(_pixel, point, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, layerDepth);
        }

        #endregion

        #region DrawCircle

        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the circle</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness = 1.0f, float layerDepth = 0.0f)
        {
            DrawCircle(spriteBatch, center.X, center.Y, radius,sides, color, thickness, layerDepth);
        }

        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The center X of the circle</param>
        /// <param name="y">The center Y of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the circle</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, float radius, int sides, Color color, float thickness = 1.0f, float layerDepth = 0.0f)
        {
            var circlePoints = CreateCircle(radius, sides);
            DrawPoints(spriteBatch, new Vector2(x, y), circlePoints, color, thickness, layerDepth);
        }

        #endregion

        #region DrawArc

        /// <summary>
        /// Draw an arc
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the arc</param>
        /// <param name="radius">The radius of the arc</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="startingAngle">The starting angle of arc, 0 being to the east, increasing as you go clockwise</param>
        /// <param name="radians">The number of radians to draw, clockwise from the starting angle</param>
        /// <param name="color">The color of the arc</param>
        /// <param name="thickness">The thickness of the arc</param>
        /// <param name="layerDepth">The layer depth</param>
        public static void DrawArc(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, float startingAngle, float radians, Color color, float thickness = 1.0f, float layerDepth = 0.0f)
        {
            var arcPoints = CreateArc(radius, sides, startingAngle, radians);
            //var arc = CreateArc2(radius, sides, startingAngle, degrees);
            DrawPoints(spriteBatch, center, arcPoints, color, thickness, layerDepth);
        }

        #endregion

        #region DrawPoint

        public static void DrawPoint(this SpriteBatch spriteBatch, float x, float y, Color color, float layerDepth = 0.0f)
        {
            DrawPoint(spriteBatch, new Vector2(x, y), color, layerDepth);
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color, float layerDepth = 0.0f)
        {
            if (_pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            spriteBatch.Draw(_pixel, position, null, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        #endregion

    }
}