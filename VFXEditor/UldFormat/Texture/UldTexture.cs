using ImGuiNET;
using ImGuiScene;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.Select;

namespace VfxEditor.UldFormat.Texture {
    public class UldTexture : UldWorkspaceItem {
        public readonly ParsedString Path = new( "Path", maxSize: 44 );
        private readonly ParsedUInt IconId = new( "Icon Id" );
        private readonly ParsedUInt Unk1 = new( "Unknown 1" );

        private string LoadedTexturePath = "";
        private string LoadedIconPath = "";

        private bool ShowHd = false;

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

        public override void Draw( string id ) {
            DrawRename( id );
            Id.Draw( id, CommandManager.Uld );

            Path.Draw( id, CommandManager.Uld );
            if( !string.IsNullOrEmpty( Path.Value ) ) {
                ImGui.Checkbox( $"Show Hd{id}", ref ShowHd );
                if( ShowHd ) ImGui.TextDisabled( TexturePath );
                Plugin.TextureManager.DrawTexture( UpdateTexture(), id );
            }

            IconId.Draw( id, CommandManager.Uld );
            UpdateIcon();
            if( IconId.Value > 0 ) {
                ImGui.Checkbox( $"Show Hd{id}", ref ShowHd );
                ImGui.TextDisabled( IconPath );
                Plugin.TextureManager.DrawTexture( UpdateIcon(), id );
            }

            Unk1.Draw( id, CommandManager.Uld );
        }

        public override string GetDefaultText() => $"Texture {GetIdx()}";

        public override string GetWorkspaceId() => $"Texture{GetIdx()}";

        private string TexturePath => ShowHd ? Path.Value.Replace( ".tex", "_hr1.tex" ) : Path.Value;

        private string IconPath => string.Format( "ui/icon/{0:D3}000/{1:D6}{2}.tex", IconId.Value / 1000, IconId.Value, ShowHd ? "_hr1" : "" );
    }
}
