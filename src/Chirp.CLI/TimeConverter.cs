using System;

namespace Chirp.CLI
{
    public static class TimeConverter
    {
        public static long UCTToUCT2(long unixTimestamp)
        {
            return unixTimestamp + 7200;
        }

        public static string ToReadable(long unixTimestamp)
        {
            // Format as "dd/MM/yy HH:mm:ss"
            return DateTime.UnixEpoch.AddSeconds(unixTimestamp).ToString("dd/MM/yy HH:mm:ss").Replace("-", "/");
        }
    }
}
