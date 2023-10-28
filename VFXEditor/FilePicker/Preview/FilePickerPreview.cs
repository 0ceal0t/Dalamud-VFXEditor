using Dalamud.Interface;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TeximpNet.DDS;
using VfxEditor.FilePicker.FolderFiles;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Utils;

namespace VfxEditor.FilePicker.Preview {
    public class FilePickerPreview {
        public static int Width => Plugin.Configuration.FilePickerPreviewOpen ? 300 : 15;
        private static readonly List<string> ImageExtensions = new() { "jpeg", "jpg", "png", "dds", "atex", "tex" };

        private static readonly SemaphoreSlim Lock = new( 1, 1 );
        private IDalamudTextureWrap Texture;
        private string FileName;
        private string Format;
        private int Mips;

        public unsafe void Draw() {
            var offset = ImGui.GetContentRegionAvail().Y / 2 - 10;
            using( var _ = ImRaii.Group() ) {
                ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 2, offset ) );
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                using var style = ImRaii.PushStyle( ImGuiStyleVar.FramePadding, new Vector2( 2, 2 ) );
                if( UiUtils.TransparentButton( Plugin.Configuration.FilePickerPreviewOpen ?
                    FontAwesomeIcon.AngleRight.ToIconString() :
                    FontAwesomeIcon.AngleLeft.ToIconString(), *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ) ) ) {
                    Plugin.Configuration.FilePickerPreviewOpen = !Plugin.Configuration.FilePickerPreviewOpen;
                    Plugin.Configuration.Save();
                }
            }

            if( !Plugin.Configuration.FilePickerPreviewOpen ) return;

            ImGui.SameLine();
            using var group = ImRaii.Group();

            var size = ImGui.GetContentRegionAvail();

            if( Texture == null ) {
                var text = "No Image to Preview";
                var textWidth = UiUtils.GetPaddedIconSize( FontAwesomeIcon.InfoCircle ) + ImGui.CalcTextSize( text ).X;

                ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( ( size.X - textWidth ) / 2, offset ) );
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.TextDisabled( FontAwesomeIcon.InfoCircle.ToIconString() );
                }

                ImGui.SameLine();
                ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 0, offset ) );
                ImGui.TextDisabled( text );

                return;
            }

            if( Texture.ImGuiHandle == IntPtr.Zero ) return;

            var width = ( float )Texture.Width;
            var height = ( float )Texture.Height;

            var maxImageSize = size - new Vector2( 0, ImGui.GetFrameHeightWithSpacing() );
            var widthStretchY = ( width / height ) * maxImageSize.Y;
            var heightStretchX = ( height / width ) * maxImageSize.X;

            var imageHeight = widthStretchY > maxImageSize.X ? heightStretchX : maxImageSize.Y; // stretch image X : stretch image Y
            var imageWidth = widthStretchY > maxImageSize.X ? maxImageSize.X : widthStretchY;

            var imageSize = new Vector2( imageWidth, imageHeight );
            var totalSize = imageSize + new Vector2( 0, ImGui.GetFrameHeightWithSpacing() );

            var imageOffset = ( size - totalSize ) / 2;
            ImGui.SetCursorPos( ImGui.GetCursorPos() + imageOffset );
            ImGui.Image( Texture.ImGuiHandle, imageSize );

            var descText = $"{Format} / {Mips} / {Texture.Width}x{Texture.Height}";
            var descWidth = ImGui.CalcTextSize( descText ).X;
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + ( size.X - descWidth ) / 2 );
            ImGui.TextDisabled( descText );
        }

        public void Load( FilePickerFile file ) {
            Dispose();

            if( !Plugin.Configuration.FilePickerImagePreview ) return;
            if( !ImageExtensions.Contains( file.Ext.ToLower() ) ) return;

            Task.Run( async () => {
                if( !await Lock.WaitAsync( 1000 ) ) return;

                var path = Path.Combine( file.FilePath, file.FileName );
                var extension = file.Ext.ToLower();

                try {
                    var newTexture = extension switch {
                        "dds" => LoadDds( path, out Format, out Mips ),
                        "tex" or "atex" => LoadTex( path, out Format, out Mips ),
                        _ => LoadImage( path, out Format, out Mips )
                    };

                    FileName = file.FileName;
                    if( newTexture != null ) Texture = newTexture;
                }
                catch( Exception e ) {
                    Dalamud.Error( e, $"Error loading image preview for {path}" );
                    Dispose();
                }

                Lock.Release();
            } );
        }

        private static IDalamudTextureWrap LoadDds( string path, out string format, out int mips ) {
            format = "";
            mips = 1;

            using var ddsFile = DDSFile.Read( path );
            if( ddsFile == null ) return null;

            using var ms = new MemoryStream( File.ReadAllBytes( path ) );
            using var reader = new BinaryReader( ms );

            reader.BaseStream.Seek( 12, SeekOrigin.Begin );
            var height = reader.ReadInt32();
            var width = reader.ReadInt32();
            reader.ReadInt32(); // pitch
            reader.ReadInt32(); // depth
            mips = reader.ReadInt32();

            reader.BaseStream.Seek( 128, SeekOrigin.Begin ); // skip header
            var data = reader.ReadBytes( ( int )ms.Length - 128 );

            var ddsFormat = TextureDataFile.DXGItoTextureFormat( ddsFile.Format );
            format = $"{ddsFormat}";
            var convertedData = TextureDataFile.BgraToRgba( TextureDataFile.Convert( data, ddsFormat, width, height ) );
            return Dalamud.PluginInterface.UiBuilder.LoadImageRaw( convertedData, width, height, 4 );
        }

        private static IDalamudTextureWrap LoadTex( string path, out string format, out int mips ) {
            format = "";
            mips = 1;

            var file = TextureDataFile.LoadFromLocal( path );
            if( file == null ) return null;

            format = $"{file.Header.Format}";
            mips = file.Header.MipLevels;
            return Dalamud.PluginInterface.UiBuilder.LoadImageRaw( file.ImageData, file.Header.Width, file.Header.Height, 4 );
        }

        private static IDalamudTextureWrap LoadImage( string path, out string format, out int mips ) {
            format = path.Split( "." )[^1].ToUpper();
            mips = 1;
            return Dalamud.PluginInterface.UiBuilder.LoadImage( path );
        }

        public void Clear() => Dispose();

        public void Dispose() {
            Texture?.Dispose();
            Texture = null;
        }
    }
}
