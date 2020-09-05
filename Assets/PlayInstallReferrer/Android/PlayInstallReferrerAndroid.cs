//
//  PlayInstallReferrerAndroid.cs
//  PlayInstallReferrer
//
//  Created by Uglješa Erceg (@ugi) on 12th April 2020.
//  Copyright © 2020 Uglješa Erceg. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Ugi.PlayInstallReferrerPlugin
{
#if UNITY_ANDROID
    public class PlayInstallReferrerAndroid
    {
        private static InstallReferrerStateListener installReferrerStateProxy;
        private static AndroidJavaObject ajoInstallReferrerClient;
        private static Dictionary<int, string> installReferrerResponseCodes;

        // public API
        public static void GetInstallReferrerInfo(Action<PlayInstallReferrerDetails> callback)
        {
            ajoInstallReferrerClient = GetInstallReferrerClient();
            if (ajoInstallReferrerClient == null)
            {
                Debug.LogError("Unable to obtain InstallReferrerClient instance");
                return;
            }

            installReferrerStateProxy = new InstallReferrerStateListener(callback);
            ajoInstallReferrerClient.Call("startConnection", installReferrerStateProxy);
        }

        // private API
        private static AndroidJavaObject GetInstallReferrerClient()
        {
            if (ajoInstallReferrerClient == null)
            {
                AndroidJavaObject ajoCurrentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass ajcInstallReferrerClient = new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient");
                ajoInstallReferrerClient = ajcInstallReferrerClient.CallStatic<AndroidJavaObject>("newBuilder", ajoCurrentActivity).Call<AndroidJavaObject>("build");
            }
            if (installReferrerResponseCodes == null)
            {
                installReferrerResponseCodes = new Dictionary<int, string>();
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("OK"), "OK");
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("FEATURE_NOT_SUPPORTED"), "FEATURE_NOT_SUPPORTED");
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("SERVICE_UNAVAILABLE"), "SERVICE_UNAVAILABLE");
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("DEVELOPER_ERROR"), "DEVELOPER_ERROR");
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("SERVICE_DISCONNECTED"), "SERVICE_DISCONNECTED");
            }

            return ajoInstallReferrerClient;
        }

        private class InstallReferrerStateListener : AndroidJavaProxy
        {
            private Action<PlayInstallReferrerDetails> callback;

            public InstallReferrerStateListener(Action<PlayInstallReferrerDetails> pCallback) : base("com.android.installreferrer.api.InstallReferrerStateListener")
            {
                this.callback = pCallback;
            }

            public void onInstallReferrerSetupFinished(int responseCode)
            {
                try
                {
                    if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "OK").Key)
                    {
                        Debug.Log("InstallReferrerResponse.OK status code received");
                        AndroidJavaObject ajoReferrerDetails = ajoInstallReferrerClient.Call<AndroidJavaObject>("getInstallReferrer");
                        if (ajoReferrerDetails == null)
                        {
                            Debug.LogError("getInstallReferrer returned null AndroidJavaObject!");
                            return;
                        }

                        String installReferrer = ajoReferrerDetails.Call<string>("getInstallReferrer");
                        long installBeginTimestampSeconds = ajoReferrerDetails.Call<long>("getInstallBeginTimestampSeconds");
                        long referrerClickTimestampSeconds = ajoReferrerDetails.Call<long>("getReferrerClickTimestampSeconds");
                        long installBeginTimestampServerSeconds = ajoReferrerDetails.Call<long>("getInstallBeginTimestampServerSeconds");
                        long referrerClickTimestampServerSeconds = ajoReferrerDetails.Call<long>("getReferrerClickTimestampServerSeconds");
                        String installVersion = ajoReferrerDetails.Call<string>("getInstallVersion");
                        bool googlePlayInstant = ajoReferrerDetails.Call<bool>("getGooglePlayInstantParam");

                        PlayInstallReferrerDetails installReferrerDetails = new PlayInstallReferrerDetails(
                            installReferrer,
                            referrerClickTimestampSeconds,
                            installBeginTimestampSeconds,
                            referrerClickTimestampServerSeconds,
                            installBeginTimestampServerSeconds,
                            installVersion,
                            googlePlayInstant);
                        this.callback(installReferrerDetails);
                    }
                    else if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "FEATURE_NOT_SUPPORTED").Key)
                    {
                        Debug.LogError("InstallReferrerResponse.FEATURE_NOT_SUPPORTED status code received");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, null);
                        this.callback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                    else if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "SERVICE_UNAVAILABLE").Key)
                    {
                        Debug.LogError("InstallReferrerResponse.SERVICE_UNAVAILABLE status code received");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, null);
                        this.callback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                    else if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "DEVELOPER_ERROR").Key)
                    {
                        Debug.LogError("InstallReferrerResponse.DEVELOPER_ERROR status code received");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, null);
                        this.callback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                    else if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "SERVICE_DISCONNECTED").Key)
                    {
                        Debug.LogError("InstallReferrerResponse.SERVICE_DISCONNECTED status code received");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, null);
                        this.callback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                    else
                    {
                        Debug.LogError("Unexpected response code arrived!");
                        Debug.LogError("Response: " + responseCode);
                        Exception exception = new Exception("Unexpected response code arrived");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, exception);
                        this.callback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception: " + e);
                    PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, e);
                    this.callback(new PlayInstallReferrerDetails(installReferrerError));
                }
            }

            public void onInstallReferrerServiceDisconnected()
            {
                Debug.Log("onInstallReferrerServiceDisconnected invoked");
            }
        }
    }
#endif
}
