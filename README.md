# VFXEditor
A VFX editor plugin for FINAL FANTASY XIV which runs inside of the game

<img align="center" src="docs/aspbene_akhrai.png" width="500px">

## Installation
1. This plugin requires [XIV Quick Launcher](https://github.com/goatcorp/FFXIVQuickLauncher) to run
2. Build `VFXEditor`
3. Place the files inside of `AppData\Roaming\XIVLauncher\devPlugins`, or wherever the `devPlugins` folder of your QuickLauncher installation is located

### Notes
* This plugin does not currently work with `LivePluginLoader`
* It **probably** gets broken by Penumbra for now, since they use the same hooks

## Features
* Load and preview in-game VFXs (weapons, actions, status effects)
* Edit and export
* Preview textures
* Add, remove, and edit particles, emitters, etc.

## TODO
- [ ] Export/Import Textools Modpack
- [ ] Export/Import  Penumbra Mod
- [ ] Cutscene VFX selector
- [ ] Zone VFX selector
- [ ] Emote VFX selector
- [ ] Equipment (not weapons) VFX selector
- [ ] Better error / log display
- [ ] Open recent
- [ ] Open from equipped gear
- [ ] Penumbra integration(??)
- [ ] More settings
- [ ] Model replacement

## Contributing
If a VFX is behaving unexpectedly, or you are getting log messages incidating that it is not being parsed properly, please open an [Issue](https://github.com/mkaminsky11/Dalamud-VFXEditor/issues).

A lot of the data in `.avfx` files is not fully understood, and I'm regularly finding new fields, so any help is appreciated.