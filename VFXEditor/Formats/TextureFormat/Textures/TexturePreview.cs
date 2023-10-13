using Dalamud.Interface.Internal;
using ImGuiNET;
using OtterGui.Raii;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Textures {
    public class TexturePreview : TextureDrawable {
        private readonly ushort Height;
        private readonly ushort Width;
        private readonly ushort MipLevels;
        private readonly TextureFormat Format;
        private readonly IDalamudTextureWrap Wrap;

        private readonly bool Penumbra;

        public TexturePreview( TextureDataFile file, bool penumbra, string gamePath ) : base( gamePath ) {
            Format = file.Header.Format;
            MipLevels = file.Header.MipLevels;
            Width = file.Header.Width;
            Height = file.Header.Height;
            Wrap = Dalamud.PluginInterface.UiBuilder.LoadImageRaw( file.ImageData, file.Header.Width, file.Header.Height, 4 );
            Penumbra = penumbra;

            if( Wrap != null ) Plugin.TextureManager.Wraps.Add( Wrap );
        }

        public override void DrawImage() {
            if( Wrap == null ) return;
            ImGui.Image( Wrap.ImGuiHandle, new Vector2( Width, Height ) );
        }

        public override void DrawImage( uint u, uint v, uint w, uint h ) {
            if( Wrap == null ) return;
            var size = new Vector2( Width, Height );
            var uv0 = new Vector2( u, v ) / size;
            var uv1 = uv0 + new Vector2( w, h ) / size;
            ImGui.Image( Wrap.ImGuiHandle, new Vector2( w, h ), uv0, uv1 );
        }

        public override void DrawImage( float height ) {
            if( Wrap == null ) return;
            if( Height < height ) {
                DrawImage();
                return;
            }

            ImGui.Image( Wrap.ImGuiHandle, new Vector2( ( ( float )Width / Height ) * height, height ) );
        }

        public void DrawParams() => ImGui.TextDisabled( $"{Format} / {MipLevels} / {Width}x{Height}" );

        protected override void DrawControls() {
            DrawParams();

            if( Penumbra ) {
                ImGui.SameLine();
                using var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.YELLOW_COLOR );
                ImGui.Text( "[Penumbra]" );
            }

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetStyle().ItemSpacing.Y ) ) ) {
                DrawExportReplaceButtons();
            }
            DrawSettingsPopup();
        }

        protected override void OnReplace( string importPath ) => Plugin.TextureManager.ReplaceTexture( importPath, GamePath );

        protected override TextureDataFile GetRawData() => Dalamud.DataManager.GetFile<TextureDataFile>( GamePath );
    }
}