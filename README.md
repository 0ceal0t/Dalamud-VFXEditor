# VFXEditor
[![Download count](https://img.shields.io/endpoint?url=https%3A%2F%2Fvz32sgcoal.execute-api.us-east-1.amazonaws.com%2FVFXEditor)](https://github.com/0ceal0t/Dalamud-VFXEditor)

VFX, animation, sound, and physics editing plugin for Dalamud (**[Wiki](https://xiv.dev/game-data/visual-effects)** | **[Guides](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki)**)

> Just want to hide certain VFXs? Use [EasyEyes](https://github.com/0ceal0t/EasyEyes) instead. For editing skeletons, check out [BlenderAssist](https://github.com/0ceal0t/BlenderAssist)

Icon by [PAPACHIN](https://www.xivmodarchive.com/user/192152)

![](https://raw.githubusercontent.com/0ceal0t/Dalamud-VFXEditor/main/assets/preview2.png)

## Installation
1. This plugin requires [XIV Quick Launcher](https://github.com/goatcorp/FFXIVQuickLauncher) to run
2. Install the plugin from the `/xlplugins` menu
3. Once installed, open with `/vfxedit`

## Usage
1. Select a "Loaded VFX" (the new effect you want to use)
2. Select a "VFX Being Replaced" (the effect which is being overriden. This can be the same as the source)
3. Make any modifications you want, then press `UPDATE`

## Help
Having problems? Check [troubleshooting tips](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Troubleshooting) or a [basic guide](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Basic-Guide)

If you have other questions, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues) or ask in the [QuickLauncher discord](https://github.com/goatcorp/FFXIVQuickLauncher#need-any-help)

### Notes
* It might interact strangely with Penumbra

## Features
* Load and preview in-game VFXs (weapons, actions, status effects)
* Live VFX overlay
* Edit and export as `.avfx` or Textools/Penumbra Modpack
* Preview, export, and replace textures
* Add, remove, and edit particles, emitters, etc.
* Export and replace models
* Modify and replace abilities and animations
* Export, play, and modify music and sound effects

## Contributing
If a VFX is behaving unexpectedly, or you are getting log messages indicating that it is not being parsed properly, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues).

A lot of the data in `.avfx`, `.tmb`, and `.pap` files is not fully understood, and I'm regularly finding new fields, so any help is appreciated.

## TODO
- [ ] `.pap` keyframe control
- [ ] Test multiple animations
- [ ] Test multiple roots
- [ ] Toggle for `.pap` animation preview
- [ ] Partition/Chain mapping/unmapped bones
- [ ] `.pap` GLTF import/export
- [ ] `.sklb` show overlay
- [ ] C192
- [ ] `.scd` modded parsing
- [ ] `.uld` update component type
- [ ] Weapon bind points (some bind point ids aren't in the racial `.eid`)
- [ ] More research into sound position (C063)
- [ ] Weird crackling when playing back 4-ch and 6-ch files. Maybe related to clipping/conversion?
- [ ] Make it so node names don't change when others are deleted (Particle 1, etc.)
- [ ] More accurate spline curve calculations
- [ ] Add VFX to weapons without it (see [here](https://docs.google.com/document/d/1M04dbdV1qUt0EzRalvwbB1oI3aPT6t8KEf9KgQfGn6E/edit#heading=h.s58fuxqb2bff). Would require modifiying VFX id in imc file, and also doing a raw file copy?)
- [ ] Sound icon on timeline
- [ ] Better PNG import support
- [ ] Investigate VFX flags, better structs
- [ ] Weapons / footsteps / etc. in live view
- [ ] Add pre and post behavior to curve editor
- [ ] Update "try on" preview

```
chara/xls/boneDeformer/human.pbd
chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.skp
bgcommon/world/air/shared/timelines/for_bg/tlbg_w_air_001_01a_closed.tmb
bgcommon/world/air/shared/timelines/for_bg/tlbg_w_air_001_01a_open.tmb
bgcommon/world/air/shared/timelines/for_bg/tlbg_w_air_001_01a_close.tmb
bgcommon/world/air/shared/timelines/for_bg/tlbg_w_air_001_01a_opened.tmb
bgcommon/world/air/shared/timelines/for_bg/tlbg_w_air_001_01a_loop.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_w_tbx_010_01_closed.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_w_tbx_013_01_open.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_a_tbx_001_01_opened.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_a_tbx_001_01_close.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_w_tbx_010_01_opend.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_a_tbx_001_01_open.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_a_tbx_001_01_fadeout.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_w_tbx_010_01_fadeout.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_w_tbx_010_01_open.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_w_tbx_013_01_close.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_w_tbx_010_01_close.tmb
bgcommon/world/tbx/shared/timelines/for_bg/tlbg_a_tbx_001_01_closed.tmb
bg/ffxiv/sea_s1/shared/timelines/for_bg/tlbg_s1f2_f1_fusy2_loop.tmb
bg/ffxiv/fst_f1/shared/timelines/for_bg/tlbg_f1t1_t1_sui00c_loop.tmb
bg/ffxiv/fst_f1/shared/timelines/for_bg/tlbg_f1t1_t1_sui00a_loop.tmb
bg/ffxiv/fst_f1/shared/timelines/for_bg/tlbg_f1t1_t1_suis1_loop.tmb
bg/ffxiv/fst_f1/shared/timelines/for_bg/tlbg_f1t1_t1_suis2_loop.tmb
bg/ffxiv/fst_f1/shared/timelines/for_bg/tlbg_f1t1_t1_sui00d_loop.tmb
```