### Version 4.0.1 [20th August 2026]
#### Fixed
- Reported a lost connection to the install referrer service through the callback. `onInstallReferrerServiceDisconnected` only logged, so a disconnect arriving before details had been delivered left the callback waiting forever.
- Pinged the callback exactly once per call. The service can report a disconnect after details have already been delivered, which used to be possible on top of a successful read.

---

### Version 4.0.0 [20th August 2026]
#### Added
- Added R8 / ProGuard keep rules needed by the plugin, together with a **Minification** chapter in README explaining them (https://github.com/uerceg/play-install-referrer-unity/issues/2).
- Added handling of `PERMISSION_ERROR` response code, introduced in Install Referrer Library v2.2.
- Added package manifest, so the plugin can be added through Unity's Package Manager instead of importing a **.unitypackage**.

#### Changed
- Play Install Referrer Library is no longer bundled as an AAR - it is declared as a Gradle dependency resolved by [External Dependency Manager for Unity](https://github.com/googlesamples/unity-jar-resolver), which lets Gradle reconcile its version with other SDKs pulling the same library.
- Updated native Play Install Referrer Library to **v2.2**.
- Bumped minimum supported Unity version to **2022.3 LTS**.
- Moved all plugin scripts from **Android** and **Unity** directories into a single **Runtime** directory.
- Moved migration guide from **docs/migration.md** to **MIGRATION.md** in the repository root.
- Plugin scripts now compile into their own assembly (**Ugi.PlayInstallReferrer**) via an assembly definition file instead of landing in **Assembly-CSharp**.
- Moved example app out of the plugin directory into **Assets/Example**, so it is no longer part of what the plugin ships into your project.
- Reworked example app layout - content is centred and kept inside the safe area, so it no longer sits under the status bar or a display cutout.

#### Fixed
- Failures while reading install referrer details are reported through the callback instead of being thrown into the caller or silently dropped.
- Connection to the install referrer service is ended once the details are read, and a fresh client is built per call (a client whose connection has ended cannot be reused).
- Kept the install referrer proxy callbacks from being removed by managed code stripping in release builds.
- Removed an unused `UnityEngine.UI` import from the example app, which made the plugin fail to compile in projects without the uGUI package.

**Note**: For migration to v4.0.0, please check [migration guide](MIGRATION.md).

---

### Version 3.0.0 [12th July 2020]
#### Added
- Added reading of 3 new fields introduced in Play Install Referrer library **v2.0** - `referrerClickTimestampServerSeconds`, `installBeginTimestampServerSeconds` and `installVersion`.
- Added support for running in Editor - dummy values will be returned.

#### Changed
- Changed API namespace from **BlackBox** to **Ugi** (hopefully made up my mind).
- Changed my GitHub username from @uerceg to @ugi.
- Updated Play Install Referrer library to **v2.1**.
- Updated example app scene to show newly read fields as well.
- Updated Unity IDE supported version from **2017.4.35f1** to **2017.4.39f1**.

**Note**: For migration to v3.0.0, please check [migration guide](MIGRATION.md).

---

### Version 2.0.0 [18th May 2020]
#### Added
- Added **PlayInstallReferrer** directory to root **Assets** directory.
- Added plugin version number information to **PlayInstallReferrer.cs** header comment.
- Added [migration guide](MIGRATION.md) document.

**Note**: For migration to v2.0.0, please check [migration guide](MIGRATION.md).

---

### Version 1.0.0 [14th April 2020]
#### Added
- Initial release of **play-install-referrer** plugin.
