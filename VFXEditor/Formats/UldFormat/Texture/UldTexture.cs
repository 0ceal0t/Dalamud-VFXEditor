using ImGuiNET;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.UldFormat.Texture {
    internal enum ThemeFlags {
        Light = 1,
        Classic_FF = 2,
        Clear_Blue = 4,
    }

    public class UldTexture : UldWorkspaceItem {
        public readonly ParsedPaddedString Path = new( "Path", 44, 0x00 );
        public readonly ParsedUInt IconId = new( "Icon Id" );
        private readonly ParsedFlag<ThemeFlags> ThemeFlags = new( name: "Theme Flags", showIntField: true );

        private bool ShowHd = false;

        public UldTexture( uint id ) : base( id ) { }

        public UldTexture( BinaryReader reader, char minorVersion ) : this( 0 ) {
            Id.Read( reader );
            Path.Read( reader );
            IconId.Read( reader );
            if( minorVersion == '1' ) ThemeFlags.Read( reader );
            else ThemeFlags.Value = 0;
        }

        public void Write( BinaryWriter writer, char minorVersion ) {
            Id.Write( writer );
            Path.Write( writer );
            IconId.Write( writer );
            if( minorVersion == '1' ) ThemeFlags.Write( writer );
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

            ThemeFlags.Draw();
        }

        public override string GetDefaultText() => $"Texture {GetIdx()}";

        public override string GetWorkspaceId() => $"Texture{GetIdx()}";

        private string TexturePath => GetTexturePath( ShowHd );

        public string GetTexturePath( bool hd ) => hd ? Path.Value.Replace( ".tex", "_hr1.tex" ) : Path.Value;

        private string IconPath => GetIconPath( ShowHd );

        public string GetIconPath( bool hd ) => string.Format( "ui/icon/{0:D3}000/{1:D6}{2}.tex", IconId.Value / 1000, IconId.Value, hd ? "_hr1" : "" );
    }
}
