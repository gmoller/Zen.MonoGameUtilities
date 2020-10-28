using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zen.MonoGameUtilities
{
    public class FramesPerSecondCounter
    {
        private static readonly TimeSpan OneSecondTimeSpan = new TimeSpan(0, 0, 1);

        private int _updateFramesCounter;
        private int _drawFramesCounter;
        private TimeSpan _timer = OneSecondTimeSpan;

        public int UpdateFramesPerSecond { get; private set; }
        public int DrawFramesPerSecond { get; private set; }

        public void Update(GameTime gameTime, Viewport viewport)
        {
            _updateFramesCounter++;

            _timer += gameTime.ElapsedGameTime;
            if (_timer <= OneSecondTimeSpan) return;

            UpdateFramesPerSecond = _updateFramesCounter;
            DrawFramesPerSecond = _drawFramesCounter;
            _updateFramesCounter = 0;
            _drawFramesCounter = 0;
            _timer -= OneSecondTimeSpan;
        }

        public void Draw()
        {
            _drawFramesCounter++;
        }
    }
}