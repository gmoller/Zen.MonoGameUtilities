using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zen.MonoGameUtilities.ViewportAdapters
{
    public abstract class ViewportAdapter
    {
        #region State
        protected GraphicsDevice GraphicsDevice { get; }

        protected abstract int VirtualWidth { get; }
        protected abstract int VirtualHeight { get; }
        protected abstract int ViewportWidth { get; }
        protected abstract int ViewportHeight { get; }
        #endregion

        public Viewport Viewport => GraphicsDevice.Viewport;
        private Rectangle BoundingRectangle => new Rectangle(0, 0, VirtualWidth, VirtualHeight);
        public Point Center => BoundingRectangle.Center;

        protected ViewportAdapter(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public abstract Matrix GetScaleMatrix();

        public Point PointToScreen(Point point)
        {
            return PointToScreen(point.X, point.Y);
        }

        protected virtual Point PointToScreen(int x, int y)
        {
            var scaleMatrix = GetScaleMatrix();
            var invertedMatrix = Matrix.Invert(scaleMatrix);
            return Vector2.Transform(new Vector2(x, y), invertedMatrix).ToPoint();
        }

        public virtual void Reset()
        {
        }
    }
}