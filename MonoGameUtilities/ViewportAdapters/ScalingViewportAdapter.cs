using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zen.MonoGameUtilities.ViewportAdapters
{
    public class ScalingViewportAdapter : ViewportAdapter
    {
        #region State
        protected override int VirtualWidth { get; }
        protected override int VirtualHeight { get; }
        protected override int ViewportWidth => GraphicsDevice.Viewport.Width;
        protected override int ViewportHeight => GraphicsDevice.Viewport.Height;
        #endregion

        public ScalingViewportAdapter(GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight)
            : base(graphicsDevice)
        {
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
        }

        public override Matrix GetScaleMatrix()
        {
            var scaleX = (float)ViewportWidth / VirtualWidth;
            var scaleY = (float)ViewportHeight / VirtualHeight;
            return Matrix.CreateScale(scaleX, scaleY, 1.0f);
        }
    }
}