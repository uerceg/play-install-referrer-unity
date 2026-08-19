//
//  PlayInstallReferrerAndroid.cs
//  PlayInstallReferrer
//
//  Created by Uglješa Erceg (@uerceg) on 12th April 2020.
//  Copyright © 2020-Present Uglješa Erceg. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Ugi.PlayInstallReferrerPlugin
{
#if UNITY_ANDROID
    public class PlayInstallReferrerAndroid
    {
        private static InstallReferrerStateListener installReferrerStateProxy;
        private static Dictionary<int, string> installReferrerResponseCodes;

        // response code for failing to construct the client at all
        private const int ResponseServiceUnavailable = 1;

        // public API
        public static void GetInstallReferrerInfo(Action<PlayInstallReferrerDetails> callback)
        {
            try
            {
                AndroidJavaObject ajoInstallReferrerClient = GetInstallReferrerClient();
                if (ajoInstallReferrerClient == null)
                {
                    Debug.LogError("Unable to obtain InstallReferrerClient instance");
                    callback(new PlayInstallReferrerDetails(new PlayInstallReferrerError(ResponseServiceUnavailable, null)));
                    return;
                }

                installReferrerStateProxy = new InstallReferrerStateListener(ajoInstallReferrerClient, callback);
                ajoInstallReferrerClient.Call("startConnection", installReferrerStateProxy);
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception while starting connection with referrer client: " + ex);
                callback(new PlayInstallReferrerDetails(new PlayInstallReferrerError(ResponseServiceUnavailable, ex)));
            }
        }

        // private API
        private static AndroidJavaObject GetInstallReferrerClient()
        {
            AndroidJavaObject ajoCurrentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass ajcInstallReferrerClient = new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient");
            AndroidJavaObject ajoInstallReferrerClient = ajcInstallReferrerClient.CallStatic<AndroidJavaObject>("newBuilder", ajoCurrentActivity).Call<AndroidJavaObject>("build");

            if (installReferrerResponseCodes == null)
            {
                installReferrerResponseCodes = new Dictionary<int, string>();
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("OK"), "OK");
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("FEATURE_NOT_SUPPORTED"), "FEATURE_NOT_SUPPORTED");
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("SERVICE_UNAVAILABLE"), "SERVICE_UNAVAILABLE");
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("DEVELOPER_ERROR"), "DEVELOPER_ERROR");
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("SERVICE_DISCONNECTED"), "SERVICE_DISCONNECTED");
                installReferrerResponseCodes.Add(new AndroidJavaClass("com.android.installreferrer.api.InstallReferrerClient$InstallReferrerResponse").GetStatic<int>("PERMISSION_ERROR"), "PERMISSION_ERROR");
            }

            return ajoInstallReferrerClient;
        }

        [Preserve]
        private class InstallReferrerStateListener : AndroidJavaProxy
        {
            private AndroidJavaObject ajoInstallReferrerClient;
            private Action<PlayInstallReferrerDetails> callback;

            public InstallReferrerStateListener(AndroidJavaObject pInstallReferrerClient, Action<PlayInstallReferrerDetails> pCallback) : base("com.android.installreferrer.api.InstallReferrerStateListener")
            {
                this.ajoInstallReferrerClient = pInstallReferrerClient;
                this.callback = pCallback;
            }

            [Preserve]
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
                            PingClientCallback(new PlayInstallReferrerDetails(new PlayInstallReferrerError(
                                responseCode, new Exception("getInstallReferrer returned null AndroidJavaObject"))));
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
                        PingClientCallback(installReferrerDetails);
                    }
                    else if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "FEATURE_NOT_SUPPORTED").Key)
                    {
                        Debug.LogError("InstallReferrerResponse.FEATURE_NOT_SUPPORTED status code received");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, null);
                        PingClientCallback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                    else if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "SERVICE_UNAVAILABLE").Key)
                    {
                        Debug.LogError("InstallReferrerResponse.SERVICE_UNAVAILABLE status code received");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, null);
                        PingClientCallback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                    else if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "DEVELOPER_ERROR").Key)
                    {
                        Debug.LogError("InstallReferrerResponse.DEVELOPER_ERROR status code received");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, null);
                        PingClientCallback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                    else if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "SERVICE_DISCONNECTED").Key)
                    {
                        Debug.LogError("InstallReferrerResponse.SERVICE_DISCONNECTED status code received");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, null);
                        PingClientCallback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                    else if (responseCode == installReferrerResponseCodes.FirstOrDefault(x => x.Value == "PERMISSION_ERROR").Key)
                    {
                        Debug.LogError("InstallReferrerResponse.PERMISSION_ERROR status code received");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, null);
                        PingClientCallback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                    else
                    {
                        Debug.LogError("Unexpected response code arrived!");
                        Debug.LogError("Response: " + responseCode);
                        Exception exception = new Exception("Unexpected response code arrived");
                        PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, exception);
                        PingClientCallback(new PlayInstallReferrerDetails(installReferrerError));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception: " + e);
                    PlayInstallReferrerError installReferrerError = new PlayInstallReferrerError(responseCode, e);
                    PingClientCallback(new PlayInstallReferrerDetails(installReferrerError));
                }
            }

            [Preserve]
            public void onInstallReferrerServiceDisconnected()
            {
                Debug.Log("onInstallReferrerServiceDisconnected invoked");
            }

            private void PingClientCallback(PlayInstallReferrerDetails installReferrerDetails)
            {
                try
                {
                    ajoInstallReferrerClient.Call("endConnection");
                }
                catch (Exception ex)
                {
                    // nothing to recover here - this client is single use either way
                    // and the next call builds its own, so a failure shouldn't affect anything
                    Debug.LogWarning("Exception while ending connection with referrer client: " + ex);
                }
                this.callback(installReferrerDetails);
            }
        }
    }
#endif
}
