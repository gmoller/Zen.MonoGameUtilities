using Microsoft.Xna.Framework;
using Zen.Utilities;

namespace Zen.MonoGameUtilities.ExtensionMethods
{
    public static class ColorExtensions
    {
        public static Color ToColor(this ColorRgba color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }
    }
}