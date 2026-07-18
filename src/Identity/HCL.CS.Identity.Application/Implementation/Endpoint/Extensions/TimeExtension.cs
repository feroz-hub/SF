/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Service.Implementation.Endpoint.Extensions;

public static class TimeExtension
{
    public static long ToUnixTime(this DateTime time)
    {
        var unixEpoch = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
        var totalSeconds = time.Subtract(unixEpoch).TotalSeconds;
        if (totalSeconds < 0) return 0;

        return Convert.ToInt32(totalSeconds);
    }

    public static DateTime ToDateTime(this long unixTime)
    {
        var unixEpoch = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
        return unixEpoch.Add(TimeSpan.FromSeconds(unixTime));
    }
}
