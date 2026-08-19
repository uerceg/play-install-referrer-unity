## Migrate play-install-referrer plugin to v4.0.0

Version 4.0.0 stops bundling Google's Play Install Referrer Library as an AAR file inside the plugin and declares it as a Gradle dependency instead.

The reason is dependency conflicts. When the library ships as a binary inside the plugin, Gradle has no version to reason about, so a project that also pulls the same library through another SDK (Facebook, Firebase, AdMob and others do) ends up with two or more copies of the same classes and the build fails. Declared as a coordinate, Gradle reconciles the versions on its own.

Three things are needed to migrate:

1. Make sure the [External Dependency Manager for Unity](https://github.com/googlesamples/unity-jar-resolver) (EDM4U) is present in your project. It resolves the dependency declared by the plugin. If your project already uses Firebase, AdMob, Facebook or a similar SDK, it is very likely there already. Without it the library never makes it into your build and the plugin fails at runtime with a `ClassNotFoundException`.

2. **Delete the whole Assets/PlayInstallReferrer directory before importing v4.0.0.** Plugin scripts moved out of the **Android** and **Unity** directories into a single **Runtime** directory, so importing on top of an older version leaves the old copies behind and your project stops compiling with duplicate type definitions. The stale AAR would linger too, giving you both the bundled copy and the resolved one.

The example app is no longer part of the plugin either - it lives beside it in the repository, at **Assets/Example**, and is not shipped. Nothing to do here unless you were relying on it, in which case grab it from the repository.

The v3.0.0 layout that needs to go:

- **Assets/PlayInstallReferrer/Android/installreferrer-2.1.aar**
- **Assets/PlayInstallReferrer/Android/PlayInstallReferrerAndroid.cs**
- **Assets/PlayInstallReferrer/Unity/PlayInstallReferrer.cs**
- **Assets/PlayInstallReferrer/Unity/PlayInstallReferrerDetails.cs**
- **Assets/PlayInstallReferrer/Unity/PlayInstallReferrerEditor.cs**
- **Assets/PlayInstallReferrer/Unity/PlayInstallReferrerError.cs**

(together with their **.meta** files and any directory left empty afterwards)

3. **Only if your own scripts live in an assembly definition of their own.** Plugin scripts now compile into their own assembly named **Ugi.PlayInstallReferrer** instead of landing in **Assembly-CSharp**. If your scripts sit in **Assembly-CSharp** (which is the default and where they are unless you added an **.asmdef** yourself), nothing changes - predefined assemblies reference every assembly definition automatically. If you did add your own **.asmdef**, add **Ugi.PlayInstallReferrer** to its list of assembly references, otherwise your scripts will no longer find the `Ugi.PlayInstallReferrerPlugin` namespace.

Three more things worth knowing about this version, none of which requires a change to your code:

- The minimum supported Unity version is now **2022.3 LTS**.
- `PERMISSION_ERROR` is a new value you can receive in `PlayInstallReferrerError.ResponseCode`, since the plugin now uses Install Referrer Library v2.2 which added it.
- If you build with **Minify** enabled, the plugin now needs R8 keep rules in order to work. They were always needed, but earlier plugin versions neither shipped nor documented them. See the **Minification** chapter of the [README](README.md).

There are no API changes in this version.

---

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
