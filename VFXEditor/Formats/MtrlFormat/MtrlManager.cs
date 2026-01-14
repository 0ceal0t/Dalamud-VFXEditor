using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.MtrlFormat.Stm;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat {
    public class MtrlStain {
        public string Name;
        public uint Id;
        public Vector3 Color;
    }

    public unsafe class MtrlManager : FileManager<MtrlDocument, MtrlFile, WorkspaceMetaBasic> {
        public readonly TextureDataFile TileDiffuseFile;
        public readonly TextureDataFile TileNormalFile;
        public readonly TextureDataFile SphereFile;
        public readonly List<IDalamudTextureWrap> TileDiffuse = [];
        public readonly List<IDalamudTextureWrap> TileNormal = [];
        public readonly List<IDalamudTextureWrap> Sphere = [];

        public readonly StmDataFile StmFileLegacy;
        public readonly StmDataFile StmFile;

        public readonly int[] Templates;
        public readonly List<MtrlStain> LegacyStains = [];

        public MtrlManager() : base( "Mtrl Editor", "Mtrl" ) {
            SourceSelect = new MtrlSelectDialog( "Mtrl Select [LOADED]", this, true );
            ReplaceSelect = new MtrlSelectDialog( "Mtrl Select [REPLACED]", this, false );

            try {
                TileDiffuseFile = TextureDataFile.LoadFromLocal( Path.Combine( Plugin.RootLocation, "Files", "tile_orb_array.tex" ) );
                TileNormalFile = TextureDataFile.LoadFromLocal( Path.Combine( Plugin.RootLocation, "Files", "tile_norm_array.tex" ) );
                SphereFile = TextureDataFile.LoadFromLocal( Path.Combine( Plugin.RootLocation, "Files", "sphere_d_array.tex" ) );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error loading files" );
            }

            if( TileDiffuseFile == null || TileNormalFile == null || SphereFile == null ) Dalamud.Error( "Could not load tile files" );
            else {
                foreach( var layer in TileDiffuseFile.Layers ) {
                    TileDiffuse.Add( Dalamud.TextureProvider.CreateFromRaw( RawImageSpecification.Rgba32( TileDiffuseFile.Header.Width, TileDiffuseFile.Header.Height ), layer ) );
                }
                foreach( var layer in TileNormalFile.Layers ) {
                    TileNormal.Add( Dalamud.TextureProvider.CreateFromRaw( RawImageSpecification.Rgba32( TileNormalFile.Header.Width, TileNormalFile.Header.Height ), layer ) );
                }
                foreach( var layer in SphereFile.Layers ) {
                    Sphere.Add( Dalamud.TextureProvider.CreateFromRaw( RawImageSpecification.Rgba32( SphereFile.Header.Width, SphereFile.Header.Height ), layer ) );
                }
            }

            // Dye Templates
            StmFileLegacy = Dalamud.DataManager.GetFile<StmDataFile>( "chara/base_material/stainingtemplate.stm" )!;
            StmFile = Dalamud.DataManager.GetFile<StmDataFileDawntrail>( "chara/base_material/stainingtemplate_gud.stm" )!;
            // https://github.com/TexTools/xivModdingFramework/blob/35d0ca49b5db25332756d2762e16c95b46a7f299/xivModdingFramework/Materials/FileTypes/STM.cs#L28
            // ======== TODO: DT stain changes =======

            var templates = new List<int> {
                0
            };
            foreach( var entry in StmFileLegacy.Entries ) templates.Add( entry.Key );
            Templates = [.. templates];

            // Dyes
            foreach( var item in Dalamud.DataManager.GetExcelSheet<Stain>().Where( x => !string.IsNullOrEmpty( x.Name.ExtractText() ) ) ) {
                var bytes = BitConverter.GetBytes( item.Color );
                LegacyStains.Add( new() {
                    Name = item.Name.ToString(),
                    Id = item.RowId,
                    Color = new( bytes[2] / 255f, bytes[1] / 255f, bytes[0] / 255f )
                } );
            }
        }

        protected override MtrlDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override MtrlDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );

        public override void Reset( ResetType type ) {
            base.Reset( type );

            // Clean up textures used for materials
            if( type == ResetType.PluginClosing ) {
                try {
                    foreach( var wrap in TileDiffuse ) wrap?.Dispose();
                    foreach( var wrap in TileNormal ) wrap?.Dispose();
                    foreach( var wrap in Sphere ) wrap?.Dispose();
                }
                catch( Exception ) { }
                TileDiffuse.Clear();
                TileNormal.Clear();
                Sphere.Clear();
            }
        }
    }
}
