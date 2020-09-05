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

**Note**: For migration to v3.0.0, please check [migration guide](docs/migration.md).

---

### Version 2.0.0 [18th May 2020]
#### Added
- Added **PlayInstallReferrer** directory to root **Assets** directory.
- Added plugin version number information to **PlayInstallReferrer.cs** header comment.
- Added [migration guide](docs/migration.md) document.

**Note**: For migration to v2.0.0, please check [migration guide](docs/migration.md).

---

### Version 1.0.0 [14th April 2020]
#### Added
- Initial release of **play-install-referrer** plugin.
