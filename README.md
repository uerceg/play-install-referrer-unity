# Play Install Referrer Library wrapper for Unity

<table>
    <tr>
        <td align="left">Supported platforms:</td>
        <td align="left"><img src="https://images-fe.ssl-images-amazon.com/images/I/21EctgvtXUL.png" width="16"></td>
    </tr>
    <tr>
        <td align="left">Current version:</td>
        <td align="left"><b>3.0.0</b></td>
    </tr>
    <tr>
        <td align="left">Unity IDE support:</td>
        <td align="left"><b>2017.4.39f1 and later</b></td>
    </tr>
    <tr>
        <td align="left">Latest release:</td>
        <td align="left"><a href=../../../releases/tag/unity-v3.0.0><b>Download</b></a></td>
    </tr>
    <tr>
        <td align="left">Troubles?</td>
        <td align="left"><a href="../../../issues/new"><b>Report an issue</b></a></td>
    </tr>
</table>

**play-install-referrer** is a simple wrapper around Google's [Play Install Referrer Library](https://developer.android.com/google/play/installreferrer/library) which offers basic functionality of obtaining Android referrer information from Unity app.

More information about Play Install Referrer API can be found in [official Google documentation](https://developer.android.com/google/play/installreferrer/igetinstallreferrerservice).

Version of native Play Install Referrer Library which is being used inside of latest **play-install-referrer** plugin version is [2.1](https://mvnrepository.com/artifact/com.android.installreferrer/installreferrer/2.1).

## Usage

In order to obtain install referrer details, call [GetInstallReferrerInfo](#api-pir-getinstallreferrerinfo) static method of [PlayInstallReferrer](#api-playinstallreferrer) class:

```csharp
using Ugi.PlayInstallReferrerPlugin;

PlayInstallReferrer.GetInstallReferrerInfo((installReferrerDetails) =>
    {
        Debug.Log("Install referrer details received!");

        // check for error
        if (installReferrerDetails.Error != null)
        {
            Debug.LogError("Error occurred!");
            if (installReferrerDetails.Error.Exception != null)
            {
                Debug.LogError("Exception message: " + installReferrerDetails.Error.Exception.Message);
            }
            Debug.LogError("Response code: " + installReferrerDetails.Error.ResponseCode.ToString());
            return;
        }

        // print install referrer details
        if (installReferrerDetails.InstallReferrer != null)
        {
            Debug.Log("Install referrer: " + installReferrerDetails.InstallReferrer);
        }
        if (installReferrerDetails.ReferrerClickTimestampSeconds != null)
        {
            Debug.Log("Referrer click timestamp: " + installReferrerDetails.ReferrerClickTimestampSeconds);
        }
        if (installReferrerDetails.InstallBeginTimestampSeconds != null)
        {
            Debug.Log("Install begin timestamp: " + installReferrerDetails.InstallBeginTimestampSeconds);
        }
        if (installReferrerDetails.ReferrerClickTimestampServerSeconds != null)
        {
            Debug.Log("Referrer click timestamp server: " + installReferrerDetails.ReferrerClickTimestampServerSeconds);
        }
        if (installReferrerDetails.InstallBeginTimestampServerSeconds != null)
        {
            Debug.Log("Install begin timestamp server: " + installReferrerDetails.InstallBeginTimestampServerSeconds);
        }
        if (installReferrerDetails.InstallVersion != null)
        {
            Debug.Log("Install version: " + installReferrerDetails.InstallVersion);
        }
        if (installReferrerDetails.GooglePlayInstant != null)
        {
            Debug.Log("Google Play instant: " + installReferrerDetails.GooglePlayInstant);
        }
    });
```

Instance of [PlayInstallReferrerDetails](#api-playinstallreferrerdetails) object will be delivered into callback method. From that instance, you can get following install referrer details:

- Install referrer string value ([InstallReferrer](#api-pird-installreferrer) property).
- Timestamp of when user clicked on URL which redirected him/her to Play Store to download your app ([ReferrerClickTimestampSeconds](#api-pird-referrerclicktimestampseconds) property).
- Timestamp of when app installation on device begun ([InstallBeginTimestampSeconds](#api-pird-installbegintimestampseconds) property).
- Server timestamp of when user clicked on URL which redirected him/her to Play Store to download your app ([ReferrerClickTimestampServerSeconds](#api-pird-referrerclicktimestampserverseconds) property).
- Server timestamp of when app installation on device begun ([InstallBeginTimestampServerSeconds](#api-pird-installbegintimestampserverseconds) property).
- Original app version which was installed ([InstallVersion](#api-pird-installversion) property).
- Information if your app's instant version (if you have one) was launched in past 7 days ([GooglePlayInstant](#api-pird-googleplayinstant) property).

You should first check if [Error](#api-pird-error) property is **null** or not. If not, for some reason reading of install referrer details failed and those properties will be **null**. In case no error is reported, install referrer detail properties should reflect the values obtained from native Install Referrer Library response.

## Under the hood

Important thing to notice is that in order to work properly, Play Install Referrer Library requires following permission to be added to your app's `AndroidManifest.xml`:

```xml
<uses-permission android:name="com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE"/>
```

Play Install Referrer Library is added to **play-install-referrer** plugin as an [AAR library](./Assets/Android/installreferrer-2.1.aar) and it will automatically make sure that manifest file ends up with above mentioned permission added to it upon building your app.

## Todos

List of tasks to be done in this repository can be found in [here](./TODO.md).

## API reference
   * [PlayInstallReferrer class](#api-playinstallreferrer)
      * [GetInstallReferrerInfo](#api-pir-getinstallreferrerinfo)
   * [PlayInstallReferrerDetails](#api-playinstallreferrerdetails)
      * [InstallReferrer](#api-pird-installreferrer)
      * [ReferrerClickTimestampSeconds](#api-pird-referrerclicktimestampseconds)
      * [InstallBeginTimestampSeconds](#api-pird-installbegintimestampseconds)
      * [ReferrerClickTimestampServerSeconds](#api-pird-referrerclicktimestampserverseconds)
      * [InstallBeginTimestampServerSeconds](#api-pird-installbegintimestampserverseconds)
      * [InstallVersion](#api-pird-installversion)
      * [GooglePlayInstant](#api-pird-googleplayinstant)
      * [Error](#api-pird-error)
   * [PlayInstallReferrerError](#api-playinstallreferrererror)
      * [ResponseCode](#api-pire-responsecode)
      * [Exception](#api-pire-exception)
      
<a id="api-playinstallreferrer"></a>PlayInstallReferrer class
---

### <a id="api-pir-getinstallreferrerinfo"></a>GetInstallReferrerInfo

```csharp
public static void GetInstallReferrerInfo(Action<PlayInstallReferrerDetails> callback)
```

Static method for getting install referrer details.

| Parameters | Description |
| :------------- |:------------- |
| **callback** | **Action\<[PlayInstallReferrerDetails](#api-playinstallreferrerdetails)\>**: Callback to which install referrer information will be delivered. |

<a id="api-playinstallreferrerdetails"></a>PlayInstallReferrerDetails class
---

### <a id="api-pird-installreferrer"></a>InstallReferrer

```csharp
public string InstallReferrer { get; }
```

Public property containing information about install referrer string value.

### <a id="api-pird-referrerclicktimestampseconds"></a>ReferrerClickTimestampSeconds

```csharp
public long? ReferrerClickTimestampSeconds { get; }
```

Public property containing information about timestamp of when user clicked on URL which redirected him/her to Play Store.

### <a id="api-pird-installbegintimestampseconds"></a>InstallBeginTimestampSeconds

```csharp
public long? InstallBeginTimestampSeconds { get; }
```

Public property containing information about timestamp of when app installation on device begun.

### <a id="api-pird-referrerclicktimestampserverseconds"></a>ReferrerClickTimestampServerSeconds

```csharp
public long? ReferrerClickTimestampServerSeconds { get; }
```

Public property containing information about server timestamp of when user clicked on URL which redirected him/her to Play Store.

### <a id="api-pird-installbegintimestampserverseconds"></a>InstallBeginTimestampServerSeconds

```csharp
public long? InstallBeginTimestampServerSeconds { get; }
```

Public property containing information about server timestamp of when app installation on device begun.

### <a id="api-pird-installversion"></a>InstallVersion

```csharp
public string InstallVersion { get; }
```

Public property containing information about original app version which was installed.

### <a id="api-pird-googleplayinstant"></a>GooglePlayInstant

```csharp
public bool? GooglePlayInstant { get; }
```

Public property containing information if app's instant version was launched in past 7 days.

### <a id="api-pird-error"></a>Error

```csharp
public PlayInstallReferrerError Error { get; }
```

Public property containing information about error which might have occured during attempt to read install referrer details.

<a id="api-playinstallreferrererror"></a>PlayInstallReferrerError class
---

### <a id="api-pire-responsecode"></a>ResponseCode

```csharp
public int ResponseCode { get; }
```

Public property containing information about one of the error response codes which native Install Referrer Library might return. Full list of potential response codes can be found in [here](https://developer.android.com/reference/com/android/installreferrer/api/InstallReferrerClient.InstallReferrerResponse) (`OK` will never be reported in this property, since it's a success status code).

### <a id="api-pire-exception"></a>Exception

```csharp
public Exception Exception { get; }
```

Public property containing information about potential exception which might have occurred during attempt to read install referrer details.
