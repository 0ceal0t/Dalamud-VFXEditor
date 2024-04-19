# VFXEditor [![Download count](https://img.shields.io/endpoint?url=https%3A%2F%2Fvz32sgcoal.execute-api.us-east-1.amazonaws.com%2FVFXEditor)](https://github.com/0ceal0t/Dalamud-VFXEditor)

**[Wiki](https://xiv.dev/game-data/visual-effects)** | **[Guides](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki)** | Icon by [PAPACHIN](https://www.xivmodarchive.com/user/192152)

_VFX, animation, sound, and physics editing plugin for Dalamud_

![](https://github.com/0ceal0t/Dalamud-VFXEditor/assets/18051158/83273164-d216-4758-9249-a2f38c03d6c2)

> Just want to hide certain VFXs? Use [EasyEyes](https://github.com/0ceal0t/EasyEyes) instead

### Supported File Types

| Extension | Description |
| --- | --- |
| `.avfx` | VFXs, such as particles and glow effects. Has no impact on a character's motion |
| `.pap` | The animations performed by a character (such as swinging a weapon, smiling, etc.) |
| `.tmb` | Timelines for when to trigger VFXs, animations, and sound effects. Edit this if you want to replace a skill in its entirety |
| `.scd` | Sound files (background music, sound effects) |
| `.eid` | Bind points used to attach VFXs to character models |
| `.uld` | Determines the layout of UI elements |
| `.atex` | Texture files for `.avfx` |
| `.tex` | Texture files for UI elements and game models |
| `.atch` | Attachment points for weapons and other accessories (like the Machinist bag and Astrologian deck) |
| `.sklb` | Skeleton and bone definitions |
| `.skp` | Skeleton paramters, such as IK |
| `.shpk` | Bundled packages of vertex and pixel shaders |
| `.shcd` | Individual shaders |
| `.mtrl` | Materials for models |
| `.mdl` | Meshes |

## Installation
1. This plugin requires [XIV Quick Launcher](https://github.com/goatcorp/FFXIVQuickLauncher) to run
2. Install the plugin from the `/xlplugins` menu
3. Once installed, open with `/vfxedit`

### Beta Builds
Add the following custom repository in `/xlsettings > Experimental`:
```
https://raw.githubusercontent.com/0ceal0t/Dalamud-VFXEditor/main/repo.json
```

The beta and main builds cannot both be enabled at the same time, so make sure to disable one before enabling the other. Open the beta version using `/vfxbeta`

## Usage
1. Select a _"Loaded Vfx"_ (the new effect you want to use)
2. Select a _"Vfx Being Replaced"_ (the effect which is being overriden. This can be the same as the source)
3. Make any modifications you want, then press `UPDATE`

## Support
Having problems? Check [troubleshooting tips](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Troubleshooting) or a [basic guide](https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Basic-Guide). If you have other questions, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues)

- [QuickLauncher Discord](https://github.com/goatcorp/FFXIVQuickLauncher#need-any-help)
- [Students of Baldesion](https://discord.gg/33jxhxH8)

## Contributing
If a VFX is behaving unexpectedly, or you are getting log messages indicating that it is not being parsed properly, please open an [Issue](https://github.com/0ceal0t/Dalamud-VFXEditor/issues).

- [.phyb research document](https://docs.google.com/document/d/1g0iSnvz9IjkGBVqXM5h3KfoyP_LOsr9LGKqiVhMZ_Us/edit)
- [.tmb/.pap research document](https://docs.google.com/document/d/1LhsTHO65pu7NcerhvoQBrYtgKyjSPggjx0JurwZVpw4/edit)
- [.scd research document](https://docs.google.com/document/d/1L9GKap9u703QJH9u1ymXCUEx4BMi1Tov4J5tvFRWp-w/edit)
- [.sklb research document](https://docs.google.com/document/d/13TBozIOwKHCMm1SMIhVUQtzaCg9bU18gDATHmXtqO1U/edit#heading=h.4fswckssvps1)

## TODO
- [ ] New `.pap` animation from gLTF (currently can only replace)
- [ ] Hex editor for replacing arbitrary files
- [ ] More investigation into `.pap` _Type_ and animation names
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
.kdlb
.bklb
.kdb (kinedriver)

Client.System.Resource.Handle.AnimationExtensionLoadResourceHandle
Client.System.Resource.Handle.BonamikLoadResourceHandle
Client.System.Resource.Handle.BonamikResourceHandle
Client.System.Resource.Handle.ExtraSkeletonLoadResourceHandle
Client.System.Resource.Handle.EyeAnimationResourceHandle
Client.System.Resource.Handle.FacialParameterEditResourceHandle
Client.System.Resource.Handle.KineDriverLoadResourceHandle
Client.System.Resource.Handle.KineDriverResourceHandle
spm = shader parameters

https://github.com/Irastris/ValkyrieUproject/tree/main/VALKYRIE_ELYSIUM/Source/KineDriverRt/Public
https://github.com/RussellJerome/TresGame/blob/main/Plugins/KineDriverRt/Source/KineDriverRt/Public/KineDriver_StructsAndEnums.h

chara/xls/bonamik/bonamik-demihuman.bklb
chara/xls/bonamik/bonamik-demihuman.bklb
chara/xls/bonamik/bonamik-human-base.bklb
chara/xls/bonamik/bonamik-human-base.bklb
chara/xls/bonamik/bonamik-human-equipment.bklb
chara/xls/bonamik/bonamik-human-equipment.bklb
chara/xls/bonamik/bonamik-human-face.bklb
chara/xls/bonamik/bonamik-human-face.bklb
chara/xls/bonamik/bonamik-human-hair.bklb
chara/xls/bonamik/bonamik-human-hair.bklb
chara/xls/bonamik/bonamik-monster.bklb
chara/xls/bonamik/bonamik-monster.bklb
chara/xls/bonamik/bonamik-weapon.bklb
chara/xls/bonamik/bonamik-weapon.bklb

chara/xls/extraskl/extra_weapon.eslb
chara/human/c1701/skeleton/face/f0002/kdi_c1701f0002.kdb

--------------------------------

chara/xls/animation/papLoadTable.plt
chara/xls/boneDeformer/human.pbd
chara/xls/equipmentParameter/equipmentVfxParameter.evp
chara/xls/animation/animation_work_table-demihuman.awt
chara/xls/animation/AnimationExchangeTable.aet
chara/xls/animation/animation_work_table-human.awt
chara/xls/animation/MotionLineTable.mlt
chara/xls/animation/animation_work_table-monster.awt
chara/xls/animation/animation_work_table-weapon.awt
```
