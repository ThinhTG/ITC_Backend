namespace ITC.Core.Utils
{
    public static class TimeHelper
    {
        private static readonly TimeZoneInfo VietnamTimeZone;

        static TimeHelper()
        {
            try
            {
                VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                VietnamTimeZone = null;
            }
        }

        /// <summary>
        /// Gets the current date and time in Vietnam's timezone (UTC+7).
        /// </summary>
        /// <returns>A DateTimeOffset representing the current time in Vietnam.</returns>
        public static DateTimeOffset GetVietnameseTime()
        {
            if (VietnamTimeZone != null)
            {
                return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, VietnamTimeZone);
            }
            // Fallback to UTC+7 if timezone not found
            return DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));
        }

        public static DateTimeOffset ConvertToUtcPlus7(DateTimeOffset dateTimeOffset)
        {
            // UTC+7 is 7 hours ahead of UTC
            TimeSpan utcPlus7Offset = new(7, 0, 0);
            return dateTimeOffset.ToOffset(utcPlus7Offset);
        }

        public static DateTimeOffset ConvertToUtcPlus7NotChanges(DateTimeOffset dateTimeOffset)
        {
            // UTC+7 is 7 hours ahead of UTC
            TimeSpan utcPlus7Offset = new(7, 0, 0);
            return dateTimeOffset.ToOffset(utcPlus7Offset).AddHours(-7);
        }

        public static long GetCurrentTimeStamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public static DateTimeOffset ConvertToVietnamTime(DateTimeOffset dateTimeOffset)
        {
            if (VietnamTimeZone != null)
            {
                return TimeZoneInfo.ConvertTime(dateTimeOffset, VietnamTimeZone);
            }
            // Fallback to UTC+7 if timezone not found
            return dateTimeOffset.ToOffset(TimeSpan.FromHours(7));
        }
    }
}
