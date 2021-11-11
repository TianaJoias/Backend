using System;

namespace Domain
{
    public class DefaultTimeProvider : TimeProvider
    {
        private readonly static DefaultTimeProvider instance =
           new DefaultTimeProvider();

        private DefaultTimeProvider() { }

        public override DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        public static DefaultTimeProvider Instance
        {
            get { return DefaultTimeProvider.instance; }
        }
    }

    public abstract class TimeProvider
    {
        private static TimeProvider _current =
            DefaultTimeProvider.Instance;

        public static TimeProvider Current
        {
            get { return _current; }
            set
            {
                _current = value ?? throw new ArgumentNullException("value");
            }
        }

        public abstract DateTime UtcNow { get; }

        public static void ResetToDefault()
        {
            _current = DefaultTimeProvider.Instance;
        }
    }
    public static class Clock
    {
        /// <summary> Normally this is a pass-through to DateTime.Now, but it can be overridden with SetDateTime( .. ) for testing or debugging.
        /// </summary>
        private static Func<DateTime> _now = () => DateTime.Now;

        /// <summary> Set time to return when SystemTime.Now() is called.
        /// </summary>
        public static void SetDateTime(DateTime dateTimeNow)
        {
            _now = () => dateTimeNow;
        }

        /// <summary> Resets SystemTime.Now() to return DateTime.Now.
        /// </summary>
        public static void ResetDateTime()
        {
            _now = () => DateTime.Now;
        }

        public static DateTime Now
        {
            get => _now();
        }
    }
}
