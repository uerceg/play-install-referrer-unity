## Migrate play-install-referrer plugin to v3.0.0

If you are migrating from version 1.0.0, please make sure to follow steps for migrating the plugin to v2.0.0 and then perform the changes needed for v3.0.0 migration.

Version 3.0.0 brought one not really necessary change - API namespace got changed from **BlackBox** to **Ugi**. 

In general chosing name for namespace as an individual is not an easy task and as you can see and in both cases I ended up picking pretty lame names. But decided to change it to honour my GitHub username renaming. Sorry, but this is the change you will need to make.

Instead of:

```csharp
using BlackBox.PlayInstallReferrerPlugin;
```

as of v3.0.0, please use:

```csharp
using Ugi.PlayInstallReferrerPlugin;
```

## Migrate play-install-referrer plugin to v2.0.0

Version 1.0.0 unfortunately brought one not really well thought through thing - all the directories containing plugin's source files were added directly into project's root **Assets** folder. Which probably no one is a fan of. Apologies for that. Version 2.0.0 fixes that and in order to migrate from v1.0.0 to v2.0.0, please make sure to completely remove the plugin prior to adding plugin version 2.0.0 to your app.

In order to completely remove plugin version 1.0.0 from your project, make sure you delete following files (and any directory which might be left empty after files deletion):

- **Assets/Android/PlayInstallReferrerAndroid.cs**
- **Assets/Android/PlayInstallReferrerAndroid.cs.meta**
- **Assets/Android/installreferrer-1.1.2.aar**
- **Assets/Android/installreferrer-1.1.2.aar.meta**
- **Assets/Example/Example.cs**
- **Assets/Example/Example.cs.meta**
- **Assets/Example/Example.prefab**
- **Assets/Example/Example.prefab.meta**
- **Assets/Example/Example.unity**
- **Assets/Example/Example.unity.meta**
- **Assets/Unity/PlayInstallReferrer.cs**
- **Assets/Unity/PlayInstallReferrer.cs.meta**
- **Assets/Unity/PlayInstallReferrerDetails.cs**
- **Assets/Unity/PlayInstallReferrerDetails.cs.meta**
- **Assets/Unity/PlayInstallReferrerError.cs**
- **Assets/Unity/PlayInstallReferrerError.cs.meta**

After these deletions, feel free to import **play-install-referrer-v2.0.0.unitypackage** to your app. Once added, all plugin directories and files will be placed under **Assets/PlayInstallReferrer** directory.
