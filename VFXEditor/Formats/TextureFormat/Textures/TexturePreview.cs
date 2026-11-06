using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Textures {
    public class TexturePreview : TextureDrawable {
        public readonly ushort Height;
        public readonly ushort Width;
        public readonly ushort MipLevels;
        public readonly ushort Depth;
        public readonly TextureFormat Format;
        public readonly IDalamudTextureWrap Wrap;

        public readonly bool Penumbra;

        private readonly string SourcePath;

        public TexturePreview( TextureDataFile file, string sourcePath, bool penumbra, string gamePath ) : base( gamePath ) {
            SourcePath = sourcePath;
            Format = file.Header.Format;
            MipLevels = file.Header.MipLevelsCount;
            Width = file.Header.Width;
            Height = file.Header.Height;
            Depth = file.Header.Depth;
            Wrap = Dalamud.TextureProvider.CreateFromRaw( RawImageSpecification.Rgba32( file.Header.Width, file.Header.Height ), file.ImageData );
            Penumbra = penumbra;

            if( Wrap != null ) Plugin.TextureManager.Wraps.Add( Wrap );
        }

        public override void DrawFullImage() {
            if( Wrap == null ) return;
            ImGui.Image( Wrap.Handle, new Vector2( Width, Height ) );
        }

        public override void DrawImage() {
            if( Wrap == null ) return;
            var maxWidth = ImGui.GetContentRegionAvail().X;
            if( Width > maxWidth ) {
                ImGui.Image( Wrap.Handle, new Vector2( maxWidth, ( ( float )Height / Width ) * maxWidth ) );
                return;
            }
            ImGui.Image( Wrap.Handle, new Vector2( Width, Height ) );
        }

        public override void DrawImage( uint u, uint v, uint w, uint h ) {
            if( Wrap == null ) return;
            var size = new Vector2( Width, Height );
            var uv0 = new Vector2( u, v ) / size;
            var uv1 = uv0 + new Vector2( w, h ) / size;
            ImGui.Image( Wrap.Handle, new Vector2( w, h ), uv0, uv1 );
        }

        public override void DrawImage( float height ) {
            if( Wrap == null ) return;
            if( Height < height ) {
                DrawImage();
                return;
            }

            ImGui.Image( Wrap.Handle, new Vector2( ( ( float )Width / Height ) * height, height ) );
        }

        public void DrawParams() => ImGui.TextDisabled( $"{Format} / {MipLevels} MIPs / {Depth} Layers / {Width}x{Height}" );

        protected override void DrawControls() {
            DrawParams();

            if( Penumbra ) {
                ImGui.SameLine();
                using var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.YELLOW_ACCENT );
                ImGui.Text( "[Penumbra]" );
            }

            DrawExportReplaceButtons();
        }

        protected override void OnReplace( string importPath ) => Plugin.TextureManager.ReplaceTexture( importPath, GamePath );

        protected override TextureDataFile GetRawData() => System.IO.Path.IsPathRooted( SourcePath ) ?
            TextureDataFile.LoadFromLocal( SourcePath ) :
            Dalamud.DataManager.GetFile<TextureDataFile>( SourcePath );

        protected override TexturePreview GetPreview() => this;
    }
}