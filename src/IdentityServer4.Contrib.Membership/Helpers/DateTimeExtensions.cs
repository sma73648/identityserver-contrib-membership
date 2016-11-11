namespace IdentityServer4.Contrib.Membership.Helpers
{
    using System;
    using IdentityModel;

    public static class DateTimeExtensions
    {
        public static DateTime ToUtc(this DateTime localDateTime)
        {
            return DateTime.SpecifyKind(localDateTime, DateTimeKind.Utc);
        }

        public static long ToUtcEpoch(this DateTime localDateTime)
        {
            return localDateTime.ToUtc().ToEpochTime();
        }
    }
}
