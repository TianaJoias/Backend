using System;

namespace Domain.Catalog
{
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
