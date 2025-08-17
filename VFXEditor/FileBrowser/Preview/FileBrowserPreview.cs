using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using VfxEditor.FileBrowser.FolderFiles;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Utils;
using VFXEditor.Formats.TextureFormat;

namespace VfxEditor.FileBrowser.Preview {
    public class FileBrowserPreview {
        public static readonly List<string> ImageExtensions = ["jpeg", "jpg", "png", "dds", "atex", "tex"];

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
                if( UiUtils.TransparentButton( Plugin.Configuration.FileBrowserPreviewOpen ?
                    FontAwesomeIcon.AngleRight.ToIconString() :
                    FontAwesomeIcon.AngleLeft.ToIconString(), *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ) ) ) {
                    Plugin.Configuration.FileBrowserPreviewOpen = !Plugin.Configuration.FileBrowserPreviewOpen;
                    Plugin.Configuration.Save();
                }
            }

            if( !Plugin.Configuration.FileBrowserPreviewOpen ) return;

            ImGui.SameLine();
            using var group = ImRaii.Group();

            var size = ImGui.GetContentRegionAvail();

            if( Texture == null ) {
                var text = "No File to Preview";
                var textWidth = UiUtils.GetPaddedIconSize( FontAwesomeIcon.InfoCircle ) + ImGui.CalcTextSize( text ).X;

                if( size.X < textWidth ) return;

                ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( ( size.X - textWidth ) / 2, offset ) );
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.TextDisabled( FontAwesomeIcon.InfoCircle.ToIconString() );
                }

                ImGui.SameLine();
                ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 0, offset ) );
                ImGui.TextDisabled( text );

                return;
            }

            if( Texture.Handle == IntPtr.Zero ) return;

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
            ImGui.Image( Texture.Handle, imageSize );

            var descText = $"{Format} / {Mips} / {Texture.Width}x{Texture.Height}";
            var descWidth = ImGui.CalcTextSize( descText ).X;
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + ( size.X - descWidth ) / 2 );
            ImGui.TextDisabled( descText );
        }

        public void Load( FileBrowserFile file ) {
            Dispose();

            if( !Plugin.Configuration.FileBrowserImagePreview ) return;
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
                    Texture?.Dispose();
                    if( newTexture != null ) Texture = newTexture;
                }
                catch( Exception e ) {
                    Dalamud.Error( e, $"Error loading image preview for {path}" );
                    Dispose();
                }

                Lock.Release();
            } );
        }

        private static IDalamudTextureWrap? LoadDds( string path, out string format, out int mips ) {
            var dds = OtterTex.ScratchImage.LoadDDS( path );
            dds.GetRGBA( out var rgba );
            var image = rgba.Images[0];

            mips = rgba.ToTexHeader().MipCount;
            format = $"{image.Format}";

            return Dalamud.TextureProvider.CreateFromRaw( RawImageSpecification.Rgba32( image.Width, image.Height ), image.Span[..( image.Width * image.Height * 4 )] );
        }

        private static IDalamudTextureWrap? LoadTex( string path, out string format, out int mips ) {
            format = "";
            mips = 1;

            var file = TextureDataFile.LoadFromLocal( path );
            if( file == null ) return null;

            format = $"{file.Header.Format}";
            mips = file.Header.MipLevelsCount;
            return Dalamud.TextureProvider.CreateFromRaw( RawImageSpecification.Rgba32( file.Header.Width, file.Header.Height ), file.ImageData );
        }

        private static IDalamudTextureWrap LoadImage( string path, out string format, out int mips ) {
            format = path.Split( "." )[^1].ToUpper();
            mips = 1;
            return Dalamud.TextureProvider.GetFromFile( path ).RentAsync().Result;
        }

        public void Clear() => Dispose();

        public void Dispose() {
            Texture?.Dispose();
            Texture = null;
        }
    }
}
