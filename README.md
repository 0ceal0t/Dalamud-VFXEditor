# VFXEditor
A VFX editor plugin for FINAL FANTASY XIV which runs inside of the game ([WIKI](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki))

<img align="center" src="docs/preview.png" width="700px">

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
* Import and export models
* Export as `.avfx` or Textools Modpack

## Contributing
If a VFX is behaving unexpectedly, or you are getting log messages incidating that it is not being parsed properly, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues).

A lot of the data in `.avfx` files is not fully understood, and I'm regularly finding new fields, so any help is appreciated.

## TODO
- [ ] Properly fix literal int list(!!)
- [ ] Fix Penumbra folder select
- [ ] Save replacements(!)
- [ ] Save individual parts (particles, emitters, etc.)
- [ ] Test with Penumbra
- [ ] Flesh out Wiki
- [ ] Model 3D preview (is this even possible?)
- [ ] Better key UI
- [ ] Help text
- [ ] Penumbra integration(??)
- [ ] GOTO button
- [ ] Get it to work with LPL (FASM is being annoying)
- [ ] TexTools import
- [ ] Penumbra import
- [ ] Monster list(??)
- [ ] Cutscene VFX select
- [ ] Zone VFX select
- [ ] Emote VFX select
- [ ] Limit break VFX
- [ ] Mount VFX select
- [ ] Equipment (not weapons) VFX select
- [ ] Better error / log display
- [ ] Open from equipped gear
- [ ] Better TMB parsing
- [ ] Better placed "DELETE" button
- [ ] Texture replacement with Penumbra/Textools
- [ ] Try on menu / equipment gpose
- [x] ~~Penumbra export~~
- [x] ~~Fix textools export~~
- [x] ~~Multiple model indexes (see `vfx/action/ab_virus1/eff/abi_virus1t1h.avfx` model particles)~~
- [x] ~~Multiple masks (see `vfx/action/mgc_siles1/eff/mgc_sile1t0c.avfx` TC1)~~
- [x] ~~-Don't write to the game folder lmao~~
- [x] ~~Fix selectable lag~~
- [x] ~~Default VFX~~
- [x] ~~Test in GPose~~
- [x] ~~Better clips UI~~
- [x] ~~Model import~~
- [x] ~~Model export~~
- [x] ~~Export texture~~
- [x] ~~Model modification (order / adding / deleting)~~
- [x] ~~Better search (doesn't have to be exact match)~~
- [x] ~~Raw extract~~
- [x] ~~Open recent~~
- [x] ~~More settings~~
- [x] ~~Verify on each load + show output with icon~~
- [x] ~~UI Cleanup (fewer trees)~~
- [x] ~~TexTools export~~
- [x] ~~Fix `ItPr` / `ItEm` (emitter comes after particle, all counted as part of the same block? see `Flash` VFX)~~
- [x] ~~Fix `BvPr = 255` issue (see `Rolling Thunder / Target` VFX)~~
- [x] ~~Bind Prp1/Prp2 (see `Thunder 2` VFX)~~
- [x] ~~Rework texture and model views~~
- [x] ~~Emitter sound~~
- [x] ~~Fix issue when adding / removing an item switches tabs. This is because the id of the tab changes, like `Particles (3) -> Particles (2)`~~
- [x] ~~Binder properties view~~