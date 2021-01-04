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

## Contributing
If a VFX is behaving unexpectedly, or you are getting log messages incidating that it is not being parsed properly, please open an [Issue](https://github.com/mkaminsky11/Dalamud-VFXEditor/issues).

A lot of the data in `.avfx` files is not fully understood, and I'm regularly finding new fields, so any help is appreciated.

## TODO
- [ ] Get it to work with LPL (FASM is being annoying)
- [ ] Export/Import Textools Modpack
- [ ] Export/Import  Penumbra Mod
- [x] ~~Raw extract~~
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
- [x] ~~UI Cleanup (fewer trees)~~
- [ ] Better key UI
- [ ] GOTO button
- [ ] Better placed "DELETE" button
- [x] ~~Fix `ItPr` / `ItEm` (emitter comes after particle, all counted as part of the same block? see `Flash` VFX)~~
- [ ] Bind Prp1/Prp2 (see `Thunder 2` VFX)
- [x] ~~Rework texture and model views~~
- [ ] Emitter sound
- [x] ~~Fix issue when adding / removing an item switches tabs. This is because the id of the tab changes, like `Particles (3) -> Particles (2)`~~
- [x] ~~Binder properties view~~