using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.TextureFormat.CustomTeximpNet;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Textures {
    public enum PostConversion {
        None,
        A8,
        R4444
    }

    public class TextureReplace : TextureDrawable, IFileDocument {
        private string Name = "";
        private string WriteLocation;
        private TexturePreview Preview;

        public TextureReplace( string writeLocation, WorkspaceMetaTex data ) : this( data.ReplacePath, writeLocation ) {
            Name = data.Name ?? "";
        }

        public TextureReplace( string gamePath, string writeLocation ) : base( gamePath ) {
            WriteLocation = writeLocation;
        }

        public void ImportFile( string importPath ) {
            Preview?.Dispose();
            Preview = null;

            try {
                var importFileExtension = Path.GetExtension( importPath ).ToLower();

                if( importFileExtension == ".dds" ) {
                    var ddsFile = DDSFile.Read( importPath );
                    var format = TextureDataFile.DXGItoTextureFormat( ddsFile.Format );
                    if( format == TextureFormat.Null ) return;

                    using( var writer = new BinaryWriter( File.Open( WriteLocation, FileMode.Create ) ) ) {
                        DdsToAtex( format, File.ReadAllBytes( importPath ), writer );
                    }
                    ddsFile.Dispose();
                }
                else if( importFileExtension == ".atex" || importFileExtension == ".tex" ) {
                    File.Copy( importPath, WriteLocation, true );
                }
                else if( importFileExtension == ".png" ) {
                    using var surface = Surface.LoadFromFile( importPath );
                    surface.FlipVertically();

                    using var compressor = new Compressor();
                    var compFormat = TextureDataFile.TextureToCompressionFormat( Plugin.Configuration.PngFormat );
                    // use ETC1 to signify "NULL" because I'm not going to be using it
                    if( compFormat == CompressionFormat.ETC1 ) return;

                    // Ui elements are required to only have 1 mip level
                    var maxMips = ( ushort )( GamePath.StartsWith( "ui/" ) ? 1 : Plugin.Configuration.PngMips );
                    compressor.Input.SetMipmapGeneration( true, maxMips );
                    compressor.Input.SetData( surface );
                    compressor.Compression.Format = compFormat;
                    compressor.Compression.SetBGRAPixelFormat();
                    compressor.Process( out var ddsContainer );

                    using var ms = new MemoryStream();

                    // lord have mercy
                    CustomDDSFile.Write( ms, ddsContainer.MipChains, ddsContainer.Format, ddsContainer.Dimension );

                    var ddsData = ms.ToArray();

                    using( var writer = new BinaryWriter( File.Open( WriteLocation, FileMode.Create ) ) ) {
                        var postConversion = Plugin.Configuration.PngFormat switch {
                            TextureFormat.A8 => PostConversion.A8,
                            TextureFormat.R4G4B4A4 => PostConversion.R4444,
                            _ => PostConversion.None
                        };

                        DdsToAtex( Plugin.Configuration.PngFormat, ddsData, writer, postConversion );
                    }
                    ddsContainer.Dispose();
                }
                else {
                    throw new Exception( $"Invalid extension {importFileExtension}" );
                }

                Preview = new TexturePreview( TextureDataFile.LoadFromLocal( WriteLocation ), GamePath );
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Error importing {importPath} into {GamePath}" );
            }
        }

        public WorkspaceMetaTex WorkspaceExport( string rootPath, int idx ) {
            var extension = Path.GetExtension( WriteLocation );
            var path = $"VFX_{idx}{extension}";
            var fullPath = Path.Combine( rootPath, path );
            File.Copy( WriteLocation, fullPath, true );

            return new WorkspaceMetaTex {
                Name = Name,
                RelativeLocation = path,
                ReplacePath = GamePath
            };
        }

        public bool CanExport() => true;

        // ===========================

        public void DrawBody() {
            var width = ImGui.GetContentRegionAvail().X - 50;

            ImGui.SetNextItemWidth( width );
            ImGui.InputTextWithHint( "Name", TrimPath( GamePath ), ref Name, 255 );

            ImGui.SetNextItemWidth( width );
            ImGui.InputText( "Path", ref GamePath, 255 );

            DrawImage();
            DrawControls();
        }

        public override void DrawImage() => Preview?.DrawImage();

        public override void DrawImage( uint u, uint v, uint w, uint h ) => Preview?.DrawImage( u, v, w, h );

        protected override void DrawControls() {
            Preview?.DrawParams();

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            DrawExportReplaceButtons();
            ImGui.SameLine();
            if( UiUtils.RemoveButton( "Reset" ) ) Plugin.TextureManager.RemoveReplace( this );
        }

        protected override TextureDataFile GetRawData() => TextureDataFile.LoadFromLocal( WriteLocation );

        // =========================

        public string GetExportSource() => "";
        public string GetExportReplace() => string.IsNullOrEmpty( Name ) ? TrimPath( GamePath ) : Name;
        public bool IsHd() => GamePath.Contains( "_hr1" );
        public bool Matches( string text ) => GamePath.ToLower().Contains( text.ToLower() )
            || ( !string.IsNullOrEmpty( Name ) && Name.ToLower().Contains( text.ToLower() ) );

        public static string TrimPath( string path ) {
            if( string.IsNullOrEmpty( path ) ) return "";

            var split = path.Split( "/" );
            var suffix = split[^1].Split( '.' )[0].Replace( "_hr1", "" );

            if( split.Length < 3 ) return suffix;

            var fallback = $"{split[0]}/{suffix}";

            // bg/ex3/05_zon_z4/common/vfx/texture/t50_dist_001.atex
            // chara/weapon/w1603/obj/body/b0001/vfx/texture/ref_f.atex
            // chara/monster/m0610/obj/body/b0001/vfx/texture/glow001at.atex
            if( split[0] == "bg" || split[0] == "chara" ) return split.Length >= 4 ? $"{split[2]}/{suffix}" : fallback;

            // ui/uld/MiragePrismBoxFilterSetting.uld
            // ui/icon/056000/056512_hr1.tex
            if( split[0] == "ui" ) return fallback;

            // vfx/aoz3/mgc_rod106/texture/tk_aura001n1.atex
            // vfx/cut/qstpdn/qstpdn00250/texture/clsnmt0a060_obi00r.atex
            // vfx/action/ab_2sw_abl029/texture/uvdist01s.atex
            // vfx/ws/wbw_kagenui/texture/ligt019_c.atex
            // vfx/ui/texture/dist_uv_f.atex
            if( split[0] == "vfx" ) {
                if( split[1] == "ui" ) return suffix;
                if( split[1] == "cut" ) return split.Length >= 5 ? $"{split[3]}/{suffix}" : fallback;
                return split.Length >= 4 ? $"{split[2]}/{suffix}" : fallback;
            }

            return fallback;
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = GamePath.ToLower().Equals( path.ToLower() ) ? WriteLocation : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        public void PenumbraExport( string modFolder, Dictionary<string, string> filesOut ) {
            if( string.IsNullOrEmpty( WriteLocation ) || string.IsNullOrEmpty( GamePath ) ) return;

            PenumbraUtils.CopyFile( WriteLocation, modFolder, GamePath, filesOut );
        }

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simplePartsOut, ref int modOffset ) {
            if( string.IsNullOrEmpty( WriteLocation ) || string.IsNullOrEmpty( GamePath ) ) return;

            using var file = File.Open( WriteLocation, FileMode.Open );
            using var texReader = new BinaryReader( file );
            using var texMs = new MemoryStream();
            using var texWriter = new BinaryWriter( texMs );
            texWriter.Write( TexToolsUtils.CreateType2Data( texReader.ReadBytes( ( int )file.Length ) ) );
            var modData = texMs.ToArray();
            simplePartsOut.Add( TexToolsUtils.CreateModResource( GamePath, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        protected override void OnReplace( string importPath ) { // since already replaced, need to refresh it
            if( Plugin.Configuration.UpdateWriteLocation ) {
                WriteLocation = Plugin.TextureManager.NewWriteLocation;
            }
            ImportFile( importPath );
        }

        // ===========================

        public void Dispose() {
            Preview?.Dispose();
            Preview = null;
            File.Delete( WriteLocation );
            Plugin.CleanupExport( this );
        }

        // ===========================

        private static void DdsToAtex( TextureFormat format, byte[] dds, BinaryWriter writer, PostConversion post = PostConversion.None ) {
            // Get DDS info
            using var ddsMs = new MemoryStream( dds );
            using var ddsReader = new BinaryReader( ddsMs );
            ddsReader.BaseStream.Seek( 12, SeekOrigin.Begin );
            var height = ddsReader.ReadInt32();
            var width = ddsReader.ReadInt32();
            var pitch = ddsReader.ReadInt32();
            var depth = ddsReader.ReadInt32();
            var mipLevels = ddsReader.ReadInt32();

            writer.Write( TextureUtils.CreateTextureHeader( format, width, height, mipLevels ).ToArray() );

            // Add DDS data
            ddsReader.BaseStream.Seek( 128, SeekOrigin.Begin );
            var uncompressedLength = ddsMs.Length - 128;
            var ddsData = new byte[uncompressedLength];
            ddsReader.Read( ddsData, 0, ( int )uncompressedLength );
            // scuffed way to handle png -> A8. Just load is as BGRA, then only keep the A channel
            // Not currently used
            if( post == PostConversion.A8 ) ddsData = TextureDataFile.CompressA8( ddsData );

            writer.Write( ddsData );
        }
    }
}
