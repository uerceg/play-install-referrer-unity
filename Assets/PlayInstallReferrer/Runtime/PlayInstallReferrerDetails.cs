//
//  PlayInstallReferrerDetails.cs
//  PlayInstallReferrer
//
//  Created by Uglješa Erceg (@ugi) on 12th April 2020.
//  Copyright © 2020 Uglješa Erceg. All rights reserved.
//

using System;
using System.Collections.Generic;

namespace Ugi.PlayInstallReferrerPlugin
{
    public class PlayInstallReferrerDetails
    {
        public string InstallReferrer { get; }
        public long? ReferrerClickTimestampSeconds { get; }
        public long? InstallBeginTimestampSeconds { get; }
        public long? ReferrerClickTimestampServerSeconds { get; }
        public long? InstallBeginTimestampServerSeconds { get; }
        public string InstallVersion { get; }
        public bool? GooglePlayInstant { get; }

        public PlayInstallReferrerError Error { get; }

        internal PlayInstallReferrerDetails(
            string installReferrer,
            long referrerClickTimestampSeconds,
            long installBeginTimestampSeconds,
            long referrerClickTimestampServerSeconds,
            long installBeginTimestampServerSeconds,
            string installVersion,
            bool googlePlayInstant)
        {
            InstallReferrer = installReferrer;
            ReferrerClickTimestampSeconds = referrerClickTimestampSeconds;
            InstallBeginTimestampSeconds = installBeginTimestampSeconds;
            ReferrerClickTimestampServerSeconds = referrerClickTimestampServerSeconds;
            InstallBeginTimestampServerSeconds = installBeginTimestampServerSeconds;
            InstallVersion = installVersion;
            GooglePlayInstant = googlePlayInstant;
        }

        internal PlayInstallReferrerDetails(PlayInstallReferrerError error)
        {
            Error = error;
        }
    }
}
