/**
 * Code from Xenko GameTime
 */

using System;

namespace NETCoreTest.Framework
{
    /// <summary>
    /// Current timing used for variable-step (real time) or fixed-step (game time) games.
    /// </summary>
    public class GameTime
    {
        private TimeSpan _accumulatedElapsedTime;

        private int _accumulatedFrameCountPerSecond;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTime" /> class.
        /// </summary>
        public GameTime()
        {
            _accumulatedElapsedTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTime" /> class.
        /// </summary>
        /// <param name="totalTime">The total game time since the start of the game.</param>
        /// <param name="elapsedTime">The elapsed game time since the last update.</param>
        public GameTime(TimeSpan totalTime, TimeSpan elapsedTime)
        {
            Total = totalTime;
            Elapsed = elapsedTime;
            _accumulatedElapsedTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTime" /> class.
        /// </summary>
        /// <param name="totalTime">The total game time since the start of the game.</param>
        /// <param name="elapsedTime">The elapsed game time since the last update.</param>
        /// <param name="isRunningSlowly">True if the game is running unexpectedly slowly.</param>
        public GameTime(TimeSpan totalTime, TimeSpan elapsedTime, bool isRunningSlowly)
        {
            Total = totalTime;
            Elapsed = elapsedTime;
            IsRunningSlowly = isRunningSlowly;
            _accumulatedElapsedTime = TimeSpan.Zero;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the elapsed game time since the last update
        /// </summary>
        /// <value>The elapsed game time.</value>
        public TimeSpan Elapsed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the game is running slowly than its TargetElapsedTime. This can be used for example to render less details...etc.
        /// </summary>
        /// <value><c>true</c> if this instance is running slowly; otherwise, <c>false</c>.</value>
        public bool IsRunningSlowly { get; private set; }

        /// <summary>
        /// Gets the amount of game time since the start of the game.
        /// </summary>
        /// <value>The total game time.</value>
        public TimeSpan Total { get; private set; }

        /// <summary>
        /// Gets the current frame count since the start of the game.
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Gets the number of frame per second (FPS) for the current running game.
        /// </summary>
        /// <value>The frame per second.</value>
        public float FramePerSecond { get; private set; }

        /// <summary>
        /// Gets the time per frame.
        /// </summary>
        /// <value>The time per frame.</value>
        public TimeSpan TimePerFrame { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="FramePerSecond"/> and <see cref="TimePerFrame"/> were updated for this frame.
        /// </summary>
        /// <value><c>true</c> if the <see cref="FramePerSecond"/> and <see cref="TimePerFrame"/> were updated for this frame; otherwise, <c>false</c>.</value>
        public bool FramePerSecondUpdated { get; private set; }

        internal void Update(TimeSpan totalGameTime, TimeSpan elapsedGameTime, TimeSpan elapsedUpdateTime, bool isRunningSlowly, bool incrementFrameCount)
        {
            Total = totalGameTime;
            Elapsed = elapsedGameTime;
            IsRunningSlowly = isRunningSlowly;
            FramePerSecondUpdated = false;

            if (incrementFrameCount)
            {
                _accumulatedElapsedTime += elapsedGameTime;
                var accumulatedElapsedGameTimeInSecond = _accumulatedElapsedTime.TotalSeconds;
                if (_accumulatedFrameCountPerSecond > 0 && accumulatedElapsedGameTimeInSecond > 1.0)
                {
                    TimePerFrame = TimeSpan.FromTicks(_accumulatedElapsedTime.Ticks / _accumulatedFrameCountPerSecond);
                    FramePerSecond = (float) (_accumulatedFrameCountPerSecond / accumulatedElapsedGameTimeInSecond);
                    _accumulatedFrameCountPerSecond = 0;
                    _accumulatedElapsedTime = TimeSpan.Zero;
                    FramePerSecondUpdated = true;
                }

                _accumulatedFrameCountPerSecond++;
                FrameCount++;
            }
        }

        internal void Reset(TimeSpan totalGameTime)
        {
            Update(totalGameTime, TimeSpan.Zero, TimeSpan.Zero, false, false);
            _accumulatedElapsedTime = TimeSpan.Zero;
            _accumulatedFrameCountPerSecond = 0;
            FrameCount = 0;
        }

        #endregion
    }
}