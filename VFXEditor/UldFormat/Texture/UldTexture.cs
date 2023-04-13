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
        private uint LoadedIconId = 0;
        private TextureWrap Icon;

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
            if( Path.Value != LoadedTexturePath ) { // Path changed
                LoadedTexturePath = Path.Value;
                Plugin.TextureManager.LoadPreviewTexture( Path.Value );
            }
            return Path.Value;
        }

        private void UpdateIcon() {
            if( IconId.Value != LoadedIconId ) { // Icon changed
                LoadedIconId = IconId.Value;
                Icon?.Dispose();
                Icon = null;
                if( LoadedIconId > 0 ) {
                    TexFile tex;
                    try { tex = Plugin.DataManager.GetIcon( LoadedIconId ); }
                    catch( Exception ) { tex = Plugin.DataManager.GetIcon( 0 ); }
                    Icon = Plugin.PluginInterface.UiBuilder.LoadImageRaw( SelectTabUtils.BgraToRgba( tex.ImageData ), tex.Header.Width, tex.Header.Height, 4 );
                }
            }
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
            Plugin.TextureManager.DrawTexture( UpdateTexture(), id );

            IconId.Draw( id, CommandManager.Uld );
            UpdateIcon();
            if( IconId.Value > 0 && Icon != null && Icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( Icon.ImGuiHandle, new Vector2( Icon.Width, Icon.Height ) );
            }

            Unk1.Draw( id, CommandManager.Uld );
        }

        public override string GetDefaultText() => $"Texture {GetIdx()}";

        public override string GetWorkspaceId() => $"Texture{GetIdx()}";
    }
}
