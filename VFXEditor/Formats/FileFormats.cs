using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Formats.AtchFormat;
using VfxEditor.Formats.EidFormat;
using VfxEditor.Formats.KdbFormat;
using VfxEditor.Formats.MdlFormat;
using VfxEditor.Formats.MtrlFormat;
using VfxEditor.Formats.PapFormat;
using VfxEditor.Formats.PbdFormat;
using VfxEditor.Formats.PhybFormat;
using VfxEditor.Formats.ScdFormat;
using VfxEditor.Formats.ShcdFormat;
using VfxEditor.Formats.ShpkFormat;
using VfxEditor.Formats.SklbFormat;
using VfxEditor.Formats.SkpFormat;
using VfxEditor.Formats.TmbFormat;
using VfxEditor.Formats.UldFormat;
using VfxEditor.Select;
using VfxEditor.Select.Base;
using VfxEditor.Select.Formats;
using VfxEditor.Select.Tabs.Actions;
using VfxEditor.Select.Tabs.Bgm;
using VfxEditor.Select.Tabs.BgmQuest;
using VfxEditor.Select.Tabs.Character;
using VfxEditor.Select.Tabs.Common;
using VfxEditor.Select.Tabs.Cutscenes;
using VfxEditor.Select.Tabs.Emotes;
using VfxEditor.Select.Tabs.Event;
using VfxEditor.Select.Tabs.EventBase;
using VfxEditor.Select.Tabs.Instance;
using VfxEditor.Select.Tabs.Items;
using VfxEditor.Select.Tabs.Job;
using VfxEditor.Select.Tabs.Mounts;
using VfxEditor.Select.Tabs.Npc;
using VfxEditor.Select.Tabs.NpcID;
using VfxEditor.Select.Tabs.Orchestrions;
using VfxEditor.Select.Tabs.Skeleton;
using VfxEditor.Select.Tabs.Voice;
using VfxEditor.Select.Tabs.Zone;
using VfxEditor.Utils;

namespace VfxEditor.Formats {
    // The editors that are created from a FileFormat. Avfx and textures have their own classes.
    // To add a simple format: implement IReadableFile on its file class, then add it here and to Plugin.Groups
    public static class FileFormats {
        public static readonly FileFormat<TmbDocument, TmbFile, WorkspaceMetaBasic> Tmb = new() {
            Name = "Tmb",
            Category = FormatCategory.Animation,
            NewDocument = ( manager, writeLocation ) => new( manager, writeLocation ),
            WorkspaceDocument = ( manager, writeLocation, localPath, data ) => new( manager, writeLocation, localPath, data ),
            CustomSelectDialog = ( title, manager, isSourceDialog ) => new TmbSelectDialog( title, manager, isSourceDialog ),
        };

        public static readonly FileFormat<PapDocument, PapFile, WorkspaceMetaBasic> Pap = new() {
            Name = "Pap",
            Category = FormatCategory.Animation,
            NewDocument = ( manager, writeLocation ) => new( manager, writeLocation ),
            WorkspaceDocument = ( manager, writeLocation, localPath, data ) => new( manager, writeLocation, localPath, data ),
            SelectTabs = dialog => [
                new ItemTabPap( dialog, "Weapon" ),
                new ActionTabPap( dialog, "Action" ),
                new ActionTabPapNonPlayer( dialog, "Non-Player Action" ),
                new EmoteTabPap( dialog, "Emote" ),
                new NpcTabPap( dialog, "Npc" ),
                new NpcIDTabPap( dialog, "NpcID" ),
                new MountTabPap( dialog, "Mount" ),
                new CharacterTabPap( dialog, "Character" ),
                new JobTab( dialog, "Job" ),
                new EventTab( dialog, "Cutscene (Human)" ),
                new EventBaseTab( dialog, "Ambient NPC (Human)" ),
                new CommonTabPap( dialog, "Common" ),
            ],
        };

        public static readonly FileFormat<FormatDocument<ScdFile>, ScdFile, WorkspaceMetaBasic> Scd = FileFormat.Basic<ScdFile>( "Scd", FormatCategory.Sound, dialog => [
            new ActionTabScd( dialog, "Actions" ),
            new MountTabScd( dialog, "Mount" ),
            new OrchestrionTab( dialog, "Orchestrion" ),
            new ZoneTabScd( dialog, "Zone" ),
            new BgmTab( dialog, "BGM" ),
            new BgmQuestTab( dialog, "Quest BGM" ),
            new InstanceTab( dialog, "Instance" ),
            new VoiceTab( dialog, "Voice" ),
            new CommonTabScd( dialog, "Common" ),
        ] );

        public static readonly FileFormat<UldDocument, UldFile, WorkspaceMetaRenamed> Uld = new() {
            Name = "Uld",
            Category = FormatCategory.Ui,
            NewDocument = ( manager, writeLocation ) => new( manager, writeLocation ),
            WorkspaceDocument = ( manager, writeLocation, localPath, data ) => new( manager, writeLocation, localPath, data ),
            SelectTabs = dialog => [
                new CommonTabUld( dialog, "Common" ),
            ],
        };

        public static readonly FileFormat<SklbDocument, SklbFile, WorkspaceMetaBasic> Sklb = new() {
            Name = "Sklb",
            Category = FormatCategory.Skeleton,
            NewDocument = ( manager, writeLocation ) => new( manager, writeLocation ),
            WorkspaceDocument = ( manager, writeLocation, localPath, data ) => new( manager, writeLocation, localPath, data ),
            SelectTabs = dialog => SkeletonTabs( dialog, "skl", "sklb" ),
        };

        public static readonly FileFormat<FormatDocument<SkpFile>, SkpFile, WorkspaceMetaBasic> Skp = FileFormat.Basic<SkpFile>( "Skp", FormatCategory.Skeleton, dialog => [
            new SkeletonTabNpc( dialog, "Npc", "skl", "skp" ),
            new CharacterTabSkeleton( dialog, "Character", "skl", "skp", false ),
        ] );

        public static readonly FileFormat<FormatDocument<PhybFile>, PhybFile, WorkspaceMetaBasic> Phyb = FileFormat.Basic<PhybFile>( "Phyb", FormatCategory.Skeleton,
            dialog => SkeletonTabs( dialog, "phy", "phyb" ) );

        public static readonly FileFormat<FormatDocument<EidFile>, EidFile, WorkspaceMetaBasic> Eid = FileFormat.Basic<EidFile>( "Eid", FormatCategory.Skeleton, dialog => [
            new SkeletonTabNpc( dialog, "Npc", "eid", "eid" ),
            new CharacterTabSkeleton( dialog, "Character", "eid", "eid", false ),
            new SkeletonTabMount( dialog, "Mount", "eid", "eid" ),
        ] );

        public static readonly FileFormat<AtchDocument, AtchFile, WorkspaceMetaBasic> Atch = new() {
            Name = "Atch",
            Category = FormatCategory.Skeleton,
            NewDocument = ( manager, writeLocation ) => new( manager, writeLocation ),
            WorkspaceDocument = ( manager, writeLocation, localPath, data ) => new( manager, writeLocation, localPath, data ),
            SelectTabs = dialog => [
                new NpcTabAtch( dialog, "Npc" ),
                new CharacterTabAtch( dialog, "Character" ),
            ],
        };

        public static readonly FileFormat<FormatDocument<KdbFile>, KdbFile, WorkspaceMetaBasic> Kdb = FileFormat.Basic<KdbFile>( "Kdb", FormatCategory.Skeleton,
            dialog => SkeletonTabs( dialog, "kdi", "kdb" ) );

        public static readonly FileFormat<FormatDocument<PbdFile>, PbdFile, WorkspaceMetaBasic> Pbd = FileFormat.Basic<PbdFile>( "Pbd", FormatCategory.Skeleton, dialog => [
            new CommonTabPbd( dialog, "Common" ),
        ] );

        public static readonly FileFormat<FormatDocument<MdlFile>, MdlFile, WorkspaceMetaBasic> Mdl = FileFormat.Basic<MdlFile>( "Mdl", FormatCategory.Model, dialog => [
            new ItemTabMdl( dialog, "Item" ),
            new CharacterTabMdl( dialog, "Character" ),
            new NpcTabMdl( dialog, "Npc" ),
        ] );

        public static readonly FileFormat<FormatDocument<MtrlFile>, MtrlFile, WorkspaceMetaBasic> Mtrl = FileFormat.Basic<MtrlFile>( "Mtrl", FormatCategory.Model, dialog => [
            new ItemTabMtrl( dialog, "Item" ),
            new CharacterTabMtrl( dialog, "Character" ),
        ] );

        public static readonly FileFormat<FormatDocument<ShpkFile>, ShpkFile, WorkspaceMetaBasic> Shpk = FileFormat.Basic<ShpkFile>( "Shpk", FormatCategory.Model, dialog => [
            new CommonTabShader( dialog, "Common", "Shpk-Common", SelectDataUtils.CommonShpkPath, ".shpk" ),
        ] );

        public static readonly FileFormat<FormatDocument<ShcdFile>, ShcdFile, WorkspaceMetaBasic> Shcd = FileFormat.Basic<ShcdFile>( "Shcd", FormatCategory.Model, dialog => [
            new CommonTabShader( dialog, "Common", "Shcd-Common", SelectDataUtils.CommonShcdPath, ".shcd" ),
        ] );

        // Skeleton-like files which exist for armor, weapons, npcs, characters, and mounts
        private static IEnumerable<SelectTab> SkeletonTabs( SelectDialog dialog, string prefix, string extension ) => [
            new SkeletonTabArmor( dialog, "Armor", prefix, extension ),
            new SkeletonTabWeapon( dialog, "Weapon", prefix, extension ),
            new SkeletonTabNpc( dialog, "Npc", prefix, extension ),
            new CharacterTabSkeleton( dialog, "Character", prefix, extension, true ),
            new SkeletonTabMount( dialog, "Mount", prefix, extension ),
        ];
    }
}