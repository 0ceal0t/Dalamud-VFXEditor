# VFXEditor
A VFX editing plugin for Dalamud (**[Wiki](https://xiv.dev/game-data/visual-effects)** | **[Guide](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Basic-Guide)**)

> Just want to hide certain VFXs? Use [this](https://github.com/0ceal0t/EasyEyes) instead

Icon by [PAPACHIN](https://www.xivmodarchive.com/user/192152)

![](https://raw.githubusercontent.com/0ceal0t/Dalamud-VFXEditor/main/assets/preview.png)

## Installation
1. This plugin requires [XIV Quick Launcher](https://github.com/goatcorp/FFXIVQuickLauncher) to run
2. Enable the plugins from the `/xlplugins` menu
3. Once installed, open with `/vfxedit`

## Usage
1. Select a "Data Source" VFX (the effect you want to edit)
2. Select a "Preview On" VFX (the effect to temporarily overwrite, can be the same as the source)
3. Make any modifications you want, then press "Update"

Having problems? Check [troubleshooting tips](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Troubleshooting) or a [basic guide](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Basic-Guide)

## Building
1. Build the solution
2. Place the files inside of `AppData\Roaming\XIVLauncher\devPlugins`, or wherever the `devPlugins` folder of your QuickLauncher installation is located
3. Run QuickLauncher

### Notes
* This plugin does not currently work with `LivePluginLoader`
* It might interact strangely with Penumbra

## Features
* Load and preview in-game VFXs (weapons, actions, status effects)
* Live VFX overlay
* Edit and export
* Preview, export, and replace textures
* Add, remove, and edit particles, emitters, etc.
* Export and replace models
* Export as `.avfx` or Textools Modpack

## Contributing
If a VFX is behaving unexpectedly, or you are getting log messages incidating that it is not being parsed properly, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues).

A lot of the data in `.avfx` files is not fully understood, and I'm regularly finding new fields, so any help is appreciated.

## TODO
- [ ] Menu shortcuts + keys move up / down
- [ ] Better PNG import support
- [ ] Scale when spawning on ground (it looks like ground-targeted aoes are spawned on a dummy actor)
- [ ] Investigate VFX flags, better structs
- [ ] Better nodegraph view? might not really be necessary yet
- [ ] Weapons / footsteps / etc. in live view
- [ ] Figure out splines (kinda done, need to add handle)
- [ ] Add pre and post behavior to curve editor
- [ ] Add VFX to weapons without it (see [here](https://docs.google.com/document/d/1M04dbdV1qUt0EzRalvwbB1oI3aPT6t8KEf9KgQfGn6E/edit#heading=h.s58fuxqb2bff). Would require modifiying VFX id in imc file, and also doing a raw file copy?)
- [ ] File loaded indication
- [ ] Better error / log display
- [ ] Better TMB parsing (regexes, lel)
- [ ] Update "try on" preview
- [ ] Auto-update npc csv file (kinda)
- [ ] Show binders on model (would require getting skeleton, since the binder is a bone, I'm pretty sure)

---
- [x] ~~Copy+Paste parameters (color keys, etc.3)~~
- [x] ~~Ground-targeted AOEs (might need to do it manually?)~~
- [x] ~~Duplicate nodes~~
- [x] ~~Get it to work with LPL (FASM is being annoying)~~
- [x] ~~Help text / hover text~~
- [x] ~~Fix crashes when replacing sound~~
- [x] ~~Render distance settings for overlay~~
- [x] ~~Fix live overlay view in cutscenes~~ (kinda)
- [x] ~~Mount select tab~~
- [x] ~~Housing select tab~~
- [x] ~~Timeline sequencer view~~
- [x] ~~Show emitter vertices~~
- [x] ~~Export multiple + dependencies~~
- [x] ~~Fix simple animation model indexes, make them dropdowns~~
- [x] ~~Better model import/export options~~
- [x] ~~Cutscene VFX select~~
- [x] ~~Basic VFX manipulator using ImGuizmo~~
- [x] ~~Gradient view?~~
- [x] ~~Clip overlay when outside of main viewport~~
- [x] ~~Better chart view for curves~~
- [x] ~~Texture replacement (+ Textools/Penumbra export)~~
- [x] ~~More texture export options~~
- [x] ~~Vfx live viewer~~
- [x] ~~Spawn vfx (on ground + on self)~~
- [x] ~~Lock required fields from being deleted~~
- [x] ~~Better default particles/binders/etc.~~
- [x] ~~Export raw texture~~
- [x] ~~UV view~~
- [x] ~~Change all 3 revised scale at the same time~~
- [x] ~~AVFX version~~
- [x] ~~Properly fix literal int list~~
- [x] ~~Linking between objects (`emitterIdx = 1` links to `Emitters[1]`, if it exists)~~
- [x] ~~Drag point snapping~~
- [x] ~~Wireframe view~~
- [x] ~~Curve editor bounding~~
- [x] ~~Curve key change order~~
- [x] ~~Curve editor zooming~~
- [x] ~~Small grid in curve editor, better handling for large curves~~
- [x] ~~3D model zoom~~
- [x] ~~Phong shader~~
- [x] ~~Drag rotate~~
- [x] ~~3D model view~~
- [x] ~~Reset on import~~
- [x] ~~Viewport resizing~~
- [x] ~~Fix texture export~~
- [x] ~~Multiple export (Textools + Penumbra)~~
- [x] ~~Multiple vfxs~~
- [x] ~~Fix BG to player conversion~~
- [x] ~~Specific vfx# in name~~
- [x] ~~When browsing for local file, just select it~~
- [x] ~~Better sidebar names to reflect target index~~
- [x] ~~Multiple model indexes (see `vfx/action/ab_virus1/eff/abi_virus1t1h.avfx` model particles)~~
- [x] ~~Multiple masks (see `vfx/action/mgc_siles1/eff/mgc_sile1t0c.avfx` TC1)~~
- [x] ~~Emote VFX select~~
- [x] ~~Fix Penumbra folder select (kinda)~~
- [x] ~~Fix expac textools export~~
- [x] ~~Monster list(??)~~
- [x] ~~Zone VFX select~~
- [x] ~~Auto fix bg vfx~~
- [x] ~~Fix model export (lots of broken shit)~~
- [x] ~~New weapon vfx~~
- [x] ~~Save individual parts (particles, emitters, etc.)~~
- [x] ~~Penumbra export~~
- [x] ~~Fix textools export~~
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
- [x] ~~Binder properties view~~
- [x] ~~Update README preview image~~