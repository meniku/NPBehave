/**
 * Code from Xenko GameBase
 */

using System;
using System.Linq;

namespace NETCoreTest.Framework
{
    /// <summary>
    /// 固定间隔更新器
    /// </summary>
    public class FixedUpdate
    {
        private readonly GameTime _updateTime;

        private readonly TimerTick _playTimer;

        private readonly TimerTick _updateTimer;

        private readonly int[] _lastUpdateCount;

        private readonly float _updateCountAverageSlowLimit;

        private TimeSpan _singleFrameUpdateTime;

        private TimeSpan _totalUpdateTime;

        private readonly TimeSpan _maximumElapsedTime;

        private TimeSpan _accumulatedElapsedGameTime;

        private TimeSpan _lastFrameElapsedGameTime;

        private int _nextLastUpdateCountIndex;

        private bool _drawRunningSlowly;

        private bool _forceElapsedTimeToZero;

        private readonly TimerTick _timer;

        internal object TickLock = new object();
        public FixedUpdate()
        {
            // Internals
            _updateTime = new GameTime();
            _playTimer = new TimerTick();
            _updateTimer = new TimerTick();
            _totalUpdateTime = new TimeSpan();
            _timer = new TimerTick();
            _maximumElapsedTime = TimeSpan.FromMilliseconds(500.0);
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60); // target elapsed time is by default 60Hz
            _lastUpdateCount = new int[4];
            _nextLastUpdateCountIndex = 0;

            // Calculate the updateCountAverageSlowLimit (assuming moving average is >=3 )
            // Example for a moving average of 4:
            // updateCountAverageSlowLimit = (2 * 2 + (4 - 2)) / 4 = 1.5f
            const int BadUpdateCountTime = 2; // number of bad frame (a bad frame is a frame that has at least 2 updates)
            var maxLastCount = 2 * Math.Min(BadUpdateCountTime, _lastUpdateCount.Length);
            _updateCountAverageSlowLimit = (float) (maxLastCount + (_lastUpdateCount.Length - maxLastCount)) / _lastUpdateCount.Length;
        }

        /// <summary>
        /// Gets the current update time from the start of the game.
        /// </summary>
        /// <value>The current update time.</value>
        public GameTime UpdateTime => _updateTime;

        /// <summary>
        /// Gets the play time, can be changed to match to the time of the current rendering scene.
        /// </summary>
        /// <value>The play time.</value>
        public TimerTick PlayTime => _playTimer;

        /// <summary>
        /// 每次更新的时间间隔,默认60Hz<br/>
        /// Gets or sets the target elapsed time, this is the duration of each tick/update 
        /// </summary>
        /// <value>The target elapsed time.</value>
        public TimeSpan TargetElapsedTime { get; set; }

        /// <summary>
        /// Resets the elapsed time counter.
        /// </summary>
        public void ResetElapsedTime()
        {
            _forceElapsedTimeToZero = true;
            _drawRunningSlowly = false;
            Array.Clear(_lastUpdateCount, 0, _lastUpdateCount.Length);
            _nextLastUpdateCountIndex = 0;
        }

        internal void InitializeBeforeRun()
        {
            _timer.Reset();
            _updateTime.Reset(_totalUpdateTime);

            // Run the first time an update
            _updateTimer.Reset();
            Update(_updateTime);

            _updateTimer.Tick();
            _singleFrameUpdateTime += _updateTimer.ElapsedTime;

            // Reset PlayTime
            _playTimer.Reset();
        }

        /// <summary>
        /// Updates the game's clock and calls Update and Draw.
        /// </summary>
        public void Tick()
        {
            lock (TickLock)
            {
                TickInternal();
            }
        }

        private void TickInternal()
        {
            // Update the timer
            _timer.Tick();

            // Update the playTimer timer
            _playTimer.Tick();

            // Measure updateTimer
            _updateTimer.Reset();

            var elapsedAdjustedTime = _timer.ElapsedTimeWithPause;

            if (_forceElapsedTimeToZero)
            {
                elapsedAdjustedTime = TimeSpan.Zero;
                _forceElapsedTimeToZero = false;
            }

            if (elapsedAdjustedTime > _maximumElapsedTime)
            {
                elapsedAdjustedTime = _maximumElapsedTime;
            }

            int updateCount = 1;

            // If the rounded TargetElapsedTime is equivalent to current ElapsedAdjustedTime
            // then make ElapsedAdjustedTime = TargetElapsedTime. We take the same internal rules as XNA
            if (Math.Abs(elapsedAdjustedTime.Ticks - TargetElapsedTime.Ticks) < (TargetElapsedTime.Ticks >> 6))
            {
                elapsedAdjustedTime = TargetElapsedTime;
            }

            // Update the accumulated time
            _accumulatedElapsedGameTime += elapsedAdjustedTime;

            // Calculate the number of update to issue

            updateCount = (int) (_accumulatedElapsedGameTime.Ticks / TargetElapsedTime.Ticks);
            if (updateCount == 0)
            {
                // If there is no need for update, then exit
                return;
            }

            // Calculate a moving average on updateCount
            _lastUpdateCount[_nextLastUpdateCountIndex] = updateCount;
            var updateCountMean = _lastUpdateCount.Aggregate<int, float>(0, (current, t) => current + t);

            updateCountMean /= _lastUpdateCount.Length;
            _nextLastUpdateCountIndex = (_nextLastUpdateCountIndex + 1) % _lastUpdateCount.Length;

            // Test when we are running slowly
            _drawRunningSlowly = updateCountMean > _updateCountAverageSlowLimit;

            // We are going to call Update updateCount times, so we can substract this from accumulated elapsed game time
            _accumulatedElapsedGameTime = new TimeSpan(_accumulatedElapsedGameTime.Ticks - (updateCount * TargetElapsedTime.Ticks));
            var singleFrameElapsedTime = TargetElapsedTime;

            // Reset the time of the next frame
            for (_lastFrameElapsedGameTime = TimeSpan.Zero; updateCount > 0; updateCount--)
            {
                _updateTime.Update(_totalUpdateTime, singleFrameElapsedTime, _singleFrameUpdateTime, _drawRunningSlowly, true);
                try
                {
                    UpdateAndProfile(_updateTime);
                }
                finally
                {
                    _lastFrameElapsedGameTime += singleFrameElapsedTime;
                    _totalUpdateTime += singleFrameElapsedTime;
                }
            }

            // End measuring update time
            _updateTimer.Tick();
            _singleFrameUpdateTime = TimeSpan.Zero;
        }

        #region Methods

        public Action UpdateCallback;

        /// <summary>
        /// Reference page contains links to related conceptual articles.
        /// </summary>
        /// <param name="gameTime">
        /// Time passed since the last call to Update.
        /// </param>
        protected virtual void Update(GameTime gameTime)
        {
            UpdateCallback?.Invoke();
            _lastFrameElapsedGameTime = TimeSpan.Zero;
        }

        private void UpdateAndProfile(GameTime gameTime)
        {
            _updateTimer.Reset();
            Update(gameTime);
            _updateTimer.Tick();
            _singleFrameUpdateTime += _updateTimer.ElapsedTime;
            _lastFrameElapsedGameTime = TimeSpan.Zero;
        }

        #endregion
    }
}