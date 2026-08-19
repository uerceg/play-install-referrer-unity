//
//  PlayInstallReferrerEditor.cs
//  PlayInstallReferrer
//
//  Created by Uglješa Erceg (@ugi) on 11th July 2020.
//  Copyright © 2020 Uglješa Erceg. All rights reserved.
//

using System;
using UnityEngine;

namespace Ugi.PlayInstallReferrerPlugin
{
#if UNITY_EDITOR
    public class PlayInstallReferrerEditor
    {
        // public API
        public static void GetInstallReferrerInfo(Action<PlayInstallReferrerDetails> callback)
        {
            PlayInstallReferrerDetails installReferrerDetails = new PlayInstallReferrerDetails(
                "test-install-referrer",
                123456,
                123456,
                123456,
                123456,
                "1.2.3.4.5.6",
                false);
                callback(installReferrerDetails);
        }
    }
#endif
}
