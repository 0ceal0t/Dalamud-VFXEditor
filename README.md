# VFXEditor [![Download count](https://img.shields.io/endpoint?url=https%3A%2F%2Fvz32sgcoal.execute-api.us-east-1.amazonaws.com%2FVFXEditor)](https://github.com/0ceal0t/Dalamud-VFXEditor)

**[Wiki](https://xiv.dev/game-data/visual-effects)** | **[Guides](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki)** | Icon by [PAPACHIN](https://www.xivmodarchive.com/user/192152)

VFX, animation, sound, and physics editing plugin for Dalamud

> Just want to hide certain VFXs? Use [EasyEyes](https://github.com/0ceal0t/EasyEyes) instead

![](https://raw.githubusercontent.com/0ceal0t/Dalamud-VFXEditor/main/assets/preview2.png)

### Supported File Types

| Extension | Description |
| --- | --- |
| `.avfx` | VFXs, such as particles and glow effects. Has no impact on a character's motion |
| `.pap` | The animations performed by a character (such as swinging a weapon, smiling, etc.) |
| `.tmb` | Timelines for when to trigger VFXs, animations, and sound effects. Edit this if you want to replace a skill in its entirety |
| `.scd` | Sound files (background music, sound effects) |
| `.eid` | Bind points used to attach VFXs to character models |
| `.uld` | Determines the layout of UI elements |
| `.atex` | Texture files for VFXs |
| `.tex` | Texture files for UI elements and game models |
| `.atch` | Attachment points for weapons and other accessories (like the Machinist bag and Astrologian deck) |

## Installation
1. This plugin requires [XIV Quick Launcher](https://github.com/goatcorp/FFXIVQuickLauncher) to run
2. Install the plugin from the `/xlplugins` menu
3. Once installed, open with `/vfxedit`

## Usage
1. Select a "Loaded VFX" (the new effect you want to use)
2. Select a "VFX Being Replaced" (the effect which is being overriden. This can be the same as the source)
3. Make any modifications you want, then press `UPDATE`

## Help
Having problems? Check [troubleshooting tips](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Troubleshooting) or a [basic guide](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Basic-Guide). If you have other questions, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues) or ask in the [QuickLauncher discord](https://github.com/goatcorp/FFXIVQuickLauncher#need-any-help)

## Contributing
If a VFX is behaving unexpectedly, or you are getting log messages indicating that it is not being parsed properly, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues).

- [.phyb research document](https://docs.google.com/document/d/1g0iSnvz9IjkGBVqXM5h3KfoyP_LOsr9LGKqiVhMZ_Us/edit)
- [.tmb/.pap research document](https://docs.google.com/document/d/1LhsTHO65pu7NcerhvoQBrYtgKyjSPggjx0JurwZVpw4/edit)
- [.scd research document](https://docs.google.com/document/d/1L9GKap9u703QJH9u1ymXCUEx4BMi1Tov4J5tvFRWp-w/edit)
- [.sklb research document](https://docs.google.com/document/d/13TBozIOwKHCMm1SMIhVUQtzaCg9bU18gDATHmXtqO1U/edit#heading=h.4fswckssvps1)

## TODO
- [ ] More investigation into `.pap` _Type_ and animation names
- [ ] `.sklb` gizmo
- [ ] Toggle for `.pap` animation preview
- [ ] C192
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

chara/xls/animation/papLoadTable.plt
chara/xls/boneDeformer/human.pbd
chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.skp
chara/xls/equipmentParameter/equipmentVfxParameter.evp
chara/xls/animation/animation_work_table-demihuman.awt
chara/xls/animation/AnimationExchangeTable.aet
chara/xls/animation/animation_work_table-human.awt
chara/xls/animation/MotionLineTable.mlt
chara/xls/animation/animation_work_table-monster.awt
chara/xls/animation/animation_work_table-weapon.awt
```

- https://github.com/goaaats/ffxiv-explorer-fork/blob/a89a8af15e3ca5d73b7f75f756d3c1058a188744/src/main/java/com/fragmenterworks/ffxivextract/models/SHPK_File.java#L50
- https://github.com/goaaats/ffxiv-explorer-fork/blob/a89a8af15e3ca5d73b7f75f756d3c1058a188744/src/main/java/com/fragmenterworks/ffxivextract/models/SHCD_File.java
- http://timjones.io/blog/archive/2015/09/02/parsing-direct3d-shader-bytecode
- https://github.com/tgjones/shader-playground
- https://github.com/Microsoft/DirectXShaderCompiler
- https://github.com/AndresTraks/HlslDecompiler
- https://github.com/tgjones/slimshader
- https://github.com/GameTechDev/IntelShaderAnalyzer