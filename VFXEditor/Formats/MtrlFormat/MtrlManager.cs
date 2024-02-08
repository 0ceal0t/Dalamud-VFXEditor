using Dalamud.Interface.Internal;
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
    public class MtrlDye {
        public string Name;
        public uint Id;
        public Vector3 Color;
    }

    public unsafe class MtrlManager : FileManager<MtrlDocument, MtrlFile, WorkspaceMetaBasic> {
        public readonly TextureDataFile TileDiffuseFile;
        public readonly TextureDataFile TileNormalFile;
        public readonly List<IDalamudTextureWrap> TileDiffuse = [];
        public readonly List<IDalamudTextureWrap> TileNormal = [];

        public readonly StmDataFile Stm;
        public readonly int[] Templates;
        public readonly List<MtrlDye> Dyes = [];

        public MtrlManager() : base( "Mtrl Editor", "Mtrl" ) {
            SourceSelect = new MtrlSelectDialog( "Mtrl Select [LOADED]", this, true );
            ReplaceSelect = new MtrlSelectDialog( "Mtrl Select [REPLACED]", this, false );

            // Tiling textures
            TileDiffuseFile = Dalamud.DataManager.GetFile<TextureDataFile>( "chara/common/texture/-tile_d.tex" );
            TileNormalFile = Dalamud.DataManager.GetFile<TextureDataFile>( "chara/common/texture/-tile_n.tex" );
            foreach( var layer in TileDiffuseFile.Layers ) {
                TileDiffuse.Add( Dalamud.PluginInterface.UiBuilder.LoadImageRaw( layer, TileDiffuseFile.Header.Width, TileDiffuseFile.Header.Height, 4 ) );
            }
            foreach( var layer in TileNormalFile.Layers ) {
                TileNormal.Add( Dalamud.PluginInterface.UiBuilder.LoadImageRaw( layer, TileNormalFile.Header.Width, TileNormalFile.Header.Height, 4 ) );
            }

            // Dye Templates
            Stm = Dalamud.DataManager.GetFile<StmDataFile>( "chara/base_material/stainingtemplate.stm" );
            var templates = new List<int> {
                0
            };
            foreach( var entry in Stm.Entries ) templates.Add( entry.Key );
            Templates = templates.ToArray();

            // Dyes
            foreach( var item in Dalamud.DataManager.GetExcelSheet<Stain>().Where( x => !string.IsNullOrEmpty( x.Name ) ) ) {
                var bytes = BitConverter.GetBytes( item.Color );
                Dyes.Add( new() {
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
