using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
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
        public readonly List<IDalamudTextureWrap> TileDiffuse = [];
        public readonly List<IDalamudTextureWrap> TileNormal = [];

        public readonly StmDataFile StmFile;
        public readonly int[] Templates;
        public readonly List<MtrlStain> Stains = [];

        public MtrlManager() : base( "Mtrl Editor", "Mtrl" ) {
            SourceSelect = new MtrlSelectDialog( "Mtrl Select [LOADED]", this, true );
            ReplaceSelect = new MtrlSelectDialog( "Mtrl Select [REPLACED]", this, false );

            // Tiling textures
            // TODO
            try {
                TileDiffuseFile = Dalamud.DataManager.GetFile<TextureDataFile>( "chara/common/texture/tile_orb_array.tex" );
                TileNormalFile = Dalamud.DataManager.GetFile<TextureDataFile>( "chara/common/texture/tile_norm_array.tex" );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error loading files" );
            }

            // the G buffer shader only uses red and green from the normal map
            // but all 4 channels from the "orb" map

            if( TileDiffuseFile == null || TileNormalFile == null ) {
                Dalamud.Error( "Could not load tile files" );
            }
            else {
                foreach( var layer in TileDiffuseFile.Layers ) {
                    TileDiffuse.Add( Dalamud.TextureProvider.CreateFromRaw( RawImageSpecification.Rgba32( TileDiffuseFile.Header.Width, TileDiffuseFile.Header.Height ), layer ) );
                }
                foreach( var layer in TileNormalFile.Layers ) {
                    TileNormal.Add( Dalamud.TextureProvider.CreateFromRaw( RawImageSpecification.Rgba32( TileNormalFile.Header.Width, TileNormalFile.Header.Height ), layer ) );
                }
            }

            // Dye Templates
            StmFile = Dalamud.DataManager.GetFile<StmDataFile>( "chara/base_material/stainingtemplate.stm" );
            // ======== TODO: DT stain changes =======

            var templates = new List<int> {
                0
            };
            foreach( var entry in StmFile.Entries ) templates.Add( entry.Key );
            Templates = [.. templates];

            // Dyes
            foreach( var item in Dalamud.DataManager.GetExcelSheet<Stain>().Where( x => !string.IsNullOrEmpty( x.Name ) ) ) {
                var bytes = BitConverter.GetBytes( item.Color );
                Stains.Add( new() {
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
                }
                catch( Exception ) { }
                TileDiffuse.Clear();
                TileNormal.Clear();
            }
        }
    }
}
