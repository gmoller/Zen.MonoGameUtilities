using Microsoft.Xna.Framework.Graphics;

namespace Zen.MonoGameUtilities.ExtensionMethods
{ 
    public static class Texture2DExtensions
    {
        public static bool HasValue(this Texture2D texture)
        {
            var hasValue = texture != null;

            return hasValue;
        }
    }
}