using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Lumina.Data.Files;
using OtterTex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.Data;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Interop.Penumbra;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Textures {
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

        public bool GetActiveReplacement( out string gamePath, out string localPath ) {
            gamePath = GamePath;
            localPath = WriteLocation;
            return !string.IsNullOrEmpty( gamePath ) && !string.IsNullOrEmpty( localPath );
        }

        public bool ImportFile( string importPath ) {
            Preview = null;

            try {
                var importFileExtension = Path.GetExtension( importPath ).ToLowerInvariant();

                if( importFileExtension == ".dds" ) ImportDds( importPath );
                else if( importFileExtension == ".atex" || importFileExtension == ".tex" ) File.Copy( importPath, WriteLocation, true );
                else if( importFileExtension == ".png" ) ImportPng( importPath );
                else throw new NotSupportedException( $"Invalid extension {importFileExtension}" );

                Preview = new TexturePreview( TextureDataFile.LoadFromLocal( WriteLocation ), WriteLocation, false, GamePath );
                return true;
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Error importing {importPath} into {GamePath}" );
                return false;
            }
        }

        private void ImportDds( string importPath ) {
            using var dds = ScratchImage.LoadDDS( importPath );
            if( dds.Meta.Format.ToTexFormat() != TexFile.TextureFormat.Unknown ) {
                TexFileParser.SaveTex( dds, WriteLocation );
                return;
            }

            // Formats the game can't read (R8G8B8A8, sRGB variants, ...) are stored as plain B8G8R8A8
            Dalamud.Log( $"DDS format {dds.Meta.Format} has no .tex equivalent, converting to B8G8R8A8" );
            using var converted = dds.Meta.Format.IsCompressed() ?
                dds.Decompress( DXGIFormat.B8G8R8A8UNorm ) :
                dds.Convert( DXGIFormat.B8G8R8A8UNorm );
            TexFileParser.SaveTex( converted, WriteLocation );
        }

        private void ImportPng( string importPath ) {
            var pngFormat = Plugin.Configuration.PngFormat;
            var target = ToDxgiFormat( pngFormat );
            if( target == DXGIFormat.Unknown ) throw new NotSupportedException( $"Unsupported .png import format {pngFormat}" );

            using var png = ScratchImage.LoadWIC( File.ReadAllBytes( importPath ), WICParseFlags.IgnoreSRGB );
            using var bgra = png.Meta.Format == DXGIFormat.B8G8R8A8UNorm ? null : png.Convert( DXGIFormat.B8G8R8A8UNorm );
            var source = bgra ?? png;

            // Ui elements are required to only have 1 mip level
            var mips = GamePath.StartsWith( "ui/", StringComparison.Ordinal ) ? 1 :
                Math.Clamp( Plugin.Configuration.PngMips, 1, TexFileParser.MaxMipLevels( source.Meta.Width, source.Meta.Height, target ) );

            using var mipped = mips > 1 ? source.GenerateMipMaps( mips, FilterFlags.ForceNonWIC | FilterFlags.SeparateAlpha ) : null;
            source = mipped ?? source;

            using var final =
                target.IsCompressed() ? source.Compress( target, CompressFlags.Parallel | ( target == DXGIFormat.BC7UNorm ? CompressFlags.BC7Quick : 0 ) ) :
                target != DXGIFormat.B8G8R8A8UNorm ? source.Convert( target ) :
                null;

            TexFileParser.SaveTex( final ?? source, WriteLocation );
        }

        private static DXGIFormat ToDxgiFormat( TextureFormat format ) => format switch {
            TextureFormat.DXT1 => DXGIFormat.BC1UNorm,
            TextureFormat.DXT3 => DXGIFormat.BC2UNorm,
            TextureFormat.DXT5 => DXGIFormat.BC3UNorm,
            TextureFormat.BC5 => DXGIFormat.BC5UNorm,
            TextureFormat.BC7 => DXGIFormat.BC7UNorm,
            TextureFormat.A8R8G8B8 => DXGIFormat.B8G8R8A8UNorm,
            TextureFormat.R4G4B4A4 => DXGIFormat.B4G4R4A4UNorm,
            TextureFormat.R5G5B5A1 => DXGIFormat.B5G5R5A1UNorm,
            TextureFormat.A8 => DXGIFormat.A8UNorm,
            _ => DXGIFormat.Unknown
        };

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

        public override void DrawFullImage() => Preview?.DrawFullImage();

        public override void DrawImage() => Preview?.DrawImage();

        public override void DrawImage( uint u, uint v, uint w, uint h ) => Preview?.DrawImage( u, v, w, h );

        public override void DrawImage( float height ) => Preview?.DrawImage( height );

        protected override void DrawControls() {
            Preview?.DrawParams();

            ImGui.SameLine();
            using( var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.PARSED_GREEN ) ) {
                ImGui.Text( "[Replaced]" );
            }

            DrawExportReplaceButtons();

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetStyle().ItemSpacing.Y ) );
            ImGui.SameLine();
            if( UiUtils.RemoveButton( "Reset" ) ) Plugin.TextureManager.RemoveReplace( this );
        }

        protected override TextureDataFile GetRawData() => TextureDataFile.LoadFromLocal( WriteLocation );

        protected override TexturePreview GetPreview() => Preview;

        // =========================

        public string GetExportSource() => "";
        public string GetExportReplace() => string.IsNullOrEmpty( Name ) ? TrimPath( GamePath ) : Name;
        public bool IsHd() => GamePath.Contains( "_hr1" );
        public bool Matches( string text ) => GamePath.Contains( text, StringComparison.CurrentCultureIgnoreCase )
            || ( !string.IsNullOrEmpty( Name ) && Name.Contains( text, StringComparison.CurrentCultureIgnoreCase ) );

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
                if( split[1] == "custom" ) return split.Length >= 5 ? $"{split[3]}/{suffix}" : fallback;
                return split.Length >= 4 ? $"{split[2]}/{suffix}" : fallback;
            }

            return fallback;
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = string.Equals( GamePath, path, StringComparison.OrdinalIgnoreCase ) ? WriteLocation : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        public void PenumbraExport( string modFolder, string groupOption, Dictionary<string, string> filesOut ) {
            if( string.IsNullOrEmpty( WriteLocation ) || string.IsNullOrEmpty( GamePath ) ) return;
            PenumbraUtils.CopyFile( WriteLocation, modFolder, groupOption, GamePath, filesOut );
        }

        public void PenumbraExportManipulations( List<object> manipulationsOut ) { }

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simplePartsOut, ref int modOffset ) {
            if( string.IsNullOrEmpty( WriteLocation ) || string.IsNullOrEmpty( GamePath ) ) return;

            byte[] modData;
            if( GamePath.EndsWith( ".atex", StringComparison.Ordinal ) ) {
                // Makes Type 2 Data from .atex header + DDS data
                modData = TexToolsUtils.CreateType2Data( File.ReadAllBytes( WriteLocation ) );
            }
            else {
                var file = TextureDataFile.LoadFromLocal( WriteLocation );
                var header = file.Header;

                TextureUtils.GetDdsInfo(
                    file.GetDdsData(), header.Format, header.Width, header.Height, header.MipLevelsCount,
                    out var compressed, out var mipPartOffset, out var mipPartCount );

                using var ms = new MemoryStream();
                using var texWriter = new BinaryWriter( ms );

                // Write Type 4 Data
                texWriter.Write( TextureUtils.CreateType4Data( header.Format, mipPartOffset, mipPartCount, file.GetDdsData().Length,
                        header.MipLevelsCount, header.Width, header.Height ) );

                // Write .atex/.tex header
                var size = Marshal.SizeOf( header );
                var buffer = new byte[size];
                var handle = Marshal.AllocHGlobal( size );
                Marshal.StructureToPtr( header, handle, true );
                Marshal.Copy( handle, buffer, 0, size );
                Marshal.FreeHGlobal( handle );
                texWriter.Write( buffer );

                // Write compressed data
                texWriter.Write( compressed );

                modData = ms.ToArray();
            }

            simplePartsOut.Add( TexToolsUtils.CreateModResource( GamePath, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        protected override void OnReplace( string importPath ) { // since already replaced, need to refresh it
            var oldLocation = WriteLocation;
            var oldPreview = Preview;
            WriteLocation = Plugin.TextureManager.GetNewWriteLocation( GamePath );
            if( !ImportFile( importPath ) ) {
                // Keep the existing replacement instead of pointing at a file that was never written
                WriteLocation = oldLocation;
                Preview = oldPreview;
                return;
            }
            PenumbraTempMod.MarkDirty();
        }
    }
}