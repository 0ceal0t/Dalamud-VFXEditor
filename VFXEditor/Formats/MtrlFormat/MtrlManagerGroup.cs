using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.Formats.MtrlFormat.Stm;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat {
    public class MtrlManagerGroup : FileManagerGroup<MtrlManager, MtrlDocument, MtrlFile, WorkspaceMetaBasic> {
        public readonly TextureDataFile TileDiffuseFile;
        public readonly TextureDataFile TileNormalFile;
        public readonly TextureDataFile SphereFile;
        public readonly List<IDalamudTextureWrap> TileDiffuse = [];
        public readonly List<IDalamudTextureWrap> TileNormal = [];
        public readonly List<IDalamudTextureWrap> Sphere = [];

        public readonly StmFileLegacy StmFileLegacy;
        public readonly StmFile StmFile;

        public readonly List<MtrlStain> Stains = [];

        public MtrlManagerGroup() : base( "Mtrl Editor", "Mtrl" ) {
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
            StmFileLegacy = Dalamud.DataManager.GetFile<StmFileLegacy>( "chara/base_material/stainingtemplate.stm" )!;
            StmFile = Dalamud.DataManager.GetFile<StmFile>( "chara/base_material/stainingtemplate_gud.stm" )!;

            // Dyes
            foreach( var item in Dalamud.DataManager.GetExcelSheet<Stain>().Where( x => !string.IsNullOrEmpty( x.Name.ExtractText() ) ) ) {
                var bytes = BitConverter.GetBytes( item.Color );
                Stains.Add( new() {
                    Name = item.Name.ToString(),
                    Id = item.RowId,
                    Color = new( bytes[2] / 255f, bytes[1] / 255f, bytes[0] / 255f )
                } );
            }
        }

        protected override MtrlManager GetNewManager() => new( this );

        public override void Reset( bool pluginClosing ) {
            base.Reset( pluginClosing );

            // Clean up textures used for materials
            if( pluginClosing ) {
                foreach( var wrap in TileDiffuse ) { try { wrap?.Dispose(); } catch( Exception ) { } }
                foreach( var wrap in TileNormal ) { try { wrap?.Dispose(); } catch( Exception ) { } }
                foreach( var wrap in Sphere ) { try { wrap?.Dispose(); } catch( Exception ) { } }
                TileDiffuse.Clear();
                TileNormal.Clear();
                Sphere.Clear();
            }
        }
    }
}
