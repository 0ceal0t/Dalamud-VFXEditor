using ImGuiNET;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Texture {
    public class UldTexture : UldWorkspaceItem {
        public readonly ParsedString Path = new( "Path", maxSize: 44 );
        public readonly ParsedUInt IconId = new( "Icon Id" );
        private readonly ParsedUInt Unk1 = new( "Unknown 1" );

        private string LoadedTexturePath = "";
        private string LoadedIconPath = "";

        private bool ShowHD = false;

        public UldTexture() { }

        public UldTexture( BinaryReader reader, char minorVersion ) {
            Id.Read( reader );
            Path.Read( reader );
            Path.Pad( reader, 44 );
            IconId.Read( reader );
            if( minorVersion == '1' ) Unk1.Read( reader );
            else Unk1.Value = 0;

            UpdateTexture();
            UpdateIcon();
        }

        private string UpdateTexture() {
            if( LoadedTexturePath != TexturePath ) { // Path changed
                LoadedTexturePath = TexturePath;
                Plugin.TextureManager.LoadPreviewTexture( TexturePath );
            }
            return TexturePath;
        }

        private string UpdateIcon() {
            if( LoadedIconPath != IconPath ) { // Icon changed
                LoadedIconPath = IconPath;
                if( IconId.Value > 0 ) Plugin.TextureManager.LoadPreviewTexture( IconPath );
            }
            return IconPath;
        }

        public void Write( BinaryWriter writer, char minorVersion ) {
            Id.Write( writer );
            Path.Write( writer );
            Path.Pad( writer, 44 );
            IconId.Write( writer );
            if( minorVersion == '1' ) Unk1.Write( writer );
        }

        public override void Draw() {
            DrawRename();
            Id.Draw( CommandManager.Uld );

            Path.Draw( CommandManager.Uld );
            if( !string.IsNullOrEmpty( Path.Value ) ) {
                ImGui.Checkbox( "Show HD", ref ShowHD );
                if( ShowHD ) ImGui.TextDisabled( TexturePath );
                Plugin.TextureManager.DrawTexture( UpdateTexture() );
            }

            IconId.Draw( CommandManager.Uld );
            UpdateIcon();
            if( IconId.Value > 0 ) {
                ImGui.Checkbox( "Show HD", ref ShowHD );
                ImGui.TextDisabled( IconPath );
                Plugin.TextureManager.DrawTexture( UpdateIcon() );
            }

            Unk1.Draw( CommandManager.Uld );
        }

        public override string GetDefaultText() => $"Texture {GetIdx()}";

        public override string GetWorkspaceId() => $"Texture{GetIdx()}";

        private string TexturePath => ShowHD ? Path.Value.Replace( ".tex", "_hr1.tex" ) : Path.Value;

        private string IconPath => GetIconPath( ShowHD );

        public string GetIconPath( bool hd ) => string.Format( "ui/icon/{0:D3}000/{1:D6}{2}.tex", IconId.Value / 1000, IconId.Value, hd ? "_hr1" : "" );
    }
}
