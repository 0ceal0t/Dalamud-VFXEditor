# VFXEditor
A VFX editor plugin for FINAL FANTASY XIV which runs inside of the game

<img align="center" src="docs/aspbene_akhrai.png" width="500px">

## Installation
1. This plugin requires [XIV Quick Launcher](https://github.com/goatcorp/FFXIVQuickLauncher) to run
2. Build `VFXEditor`
3. Place the files inside of `AppData\Roaming\XIVLauncher\devPlugins`, or wherever the `devPlugins` folder of your QuickLauncher installation is located
4. Once in-game, open with `/vfxedit`

### Notes
* This plugin does not currently work with `LivePluginLoader`
* It **probably** gets broken by Penumbra for now, since they use the same hooks

## Features
* Load and preview in-game VFXs (weapons, actions, status effects)
* Edit and export
* Preview textures
* Add, remove, and edit particles, emitters, etc.

## TODO
- [ ] Get it to work with LPL (FASM is being annoying)
- [ ] Export/Import Textools Modpack
- [ ] Export/Import  Penumbra Mod
- [ ] Raw extract
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
- [ ] Better TBM parsing
- [x] ~~Verify on each load + show output with icon~~

## Contributing
If a VFX is behaving unexpectedly, or you are getting log messages incidating that it is not being parsed properly, please open an [Issue](https://github.com/mkaminsky11/Dalamud-VFXEditor/issues).

A lot of the data in `.avfx` files is not fully understood, and I'm regularly finding new fields, so any help is appreciated.