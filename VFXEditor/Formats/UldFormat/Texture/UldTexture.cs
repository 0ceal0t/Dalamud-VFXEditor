using ImGuiNET;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.UldFormat.Texture {
    public class UldTexture : UldWorkspaceItem {
        public readonly ParsedPaddedString Path = new( "Path", 44, 0x00 );
        public readonly ParsedUInt IconId = new( "Icon Id" );
        private readonly ParsedUInt Unk1 = new( "Unknown 1" );

        private bool ShowHd = false;

        public UldTexture() { }

        public UldTexture( BinaryReader reader, char minorVersion ) {
            Id.Read( reader );
            Path.Read( reader );
            IconId.Read( reader );
            if( minorVersion == '1' ) Unk1.Read( reader );
            else Unk1.Value = 0;
        }

        public void Write( BinaryWriter writer, char minorVersion ) {
            Id.Write( writer );
            Path.Write( writer );
            IconId.Write( writer );
            if( minorVersion == '1' ) Unk1.Write( writer );
        }

        public override void Draw() {
            DrawRename();
            Id.Draw();

            Path.Draw();

            if( !string.IsNullOrEmpty( Path.Value ) ) {
                ImGui.Checkbox( "Show HD", ref ShowHd );
                if( ShowHd ) ImGui.TextDisabled( TexturePath );
                Plugin.TextureManager.GetTexture( TexturePath )?.Draw();
            }

            IconId.Draw();
            if( IconId.Value > 0 ) {
                ImGui.Checkbox( "Show HD", ref ShowHd );
                ImGui.TextDisabled( IconPath );
                Plugin.TextureManager.GetTexture( IconPath )?.Draw();
            }

            Unk1.Draw();
        }

        public override string GetDefaultText() => $"Texture {GetIdx()}";

        public override string GetWorkspaceId() => $"Texture{GetIdx()}";

        private string TexturePath => GetTexturePath( ShowHd );

        public string GetTexturePath( bool hd ) => hd ? Path.Value.Replace( ".tex", "_hr1.tex" ) : Path.Value;

        private string IconPath => GetIconPath( ShowHd );

        public string GetIconPath( bool hd ) => string.Format( "ui/icon/{0:D3}000/{1:D6}{2}.tex", IconId.Value / 1000, IconId.Value, hd ? "_hr1" : "" );
    }
}
