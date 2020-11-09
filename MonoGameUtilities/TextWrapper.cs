using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Zen.MonoGameUtilities
{
    public static class TextWrapper
    {
        public static List<string> WrapText(string text, double pixels, SpriteFont spriteFont)
        {
            var originalWords = text.Split(new string[] { " " }, StringSplitOptions.None);

            var wrappedLines = new List<string>();

            var actualLine = new StringBuilder();
            var actualWidth = 0.0f;

            foreach (var item in originalWords)
            {
                var size = spriteFont.MeasureString(item);

                actualLine.Append(item + " ");
                actualWidth += size.X;

                if (actualWidth > pixels)
                {
                    wrappedLines.Add(actualLine.ToString());
                    actualLine.Clear();
                    actualWidth = 0;
                }
            }

            if (actualLine.Length > 0)
            {
                wrappedLines.Add(actualLine.ToString());
            }

            return wrappedLines;
        }
    }
}