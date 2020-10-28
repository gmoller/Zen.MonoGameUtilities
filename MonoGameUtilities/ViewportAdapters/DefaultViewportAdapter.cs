using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zen.MonoGameUtilities.ViewportAdapters
{
    public class DefaultViewportAdapter : ViewportAdapter
    {
        #region State
        private readonly GraphicsDevice _graphicsDevice;
        #endregion

        protected override int VirtualWidth => _graphicsDevice.Viewport.Width;
        protected override int VirtualHeight => _graphicsDevice.Viewport.Height;
        protected override int ViewportWidth => _graphicsDevice.Viewport.Width;
        protected override int ViewportHeight => _graphicsDevice.Viewport.Height;

        public DefaultViewportAdapter(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }
    }
}