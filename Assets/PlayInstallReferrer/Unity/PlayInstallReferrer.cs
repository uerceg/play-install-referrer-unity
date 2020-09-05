//
//  PlayInstallReferrer.cs
//  PlayInstallReferrer
//  Version: 3.0.0
//
//  Created by Uglješa Erceg (@ugi) on 12th April 2020.
//  Copyright © 2020 Uglješa Erceg. All rights reserved.
//

using System;
using UnityEngine;

namespace Ugi.PlayInstallReferrerPlugin
{
    public class PlayInstallReferrer : MonoBehaviour
    {
        public static void GetInstallReferrerInfo(Action<PlayInstallReferrerDetails> callback)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            PlayInstallReferrerAndroid.GetInstallReferrerInfo(callback);
#elif UNITY_EDITOR
            PlayInstallReferrerEditor.GetInstallReferrerInfo(callback);
#else
            Debug.LogError("play-install-referrer plugin can only be used in Android apps.");
#endif
        }
    }
}
