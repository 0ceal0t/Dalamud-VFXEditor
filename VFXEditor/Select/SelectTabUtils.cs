using ImGuiNET;
using Lumina.Data.Files;
using System;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Select {
    public class SelectTabUtils {
        public static SelectResult GetSelectResult( string path, SelectResultType resultType, string resultName ) {
            var resultPrefix = resultType.ToString().ToUpper().Replace( "GAME", "" );
            return new SelectResult( resultType, $"[{resultPrefix}] {resultName}", path );
        }

        public static void DisplayNoVfx() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.PushStyleColor( ImGuiCol.Text, UiUtils.YELLOW_COLOR );
            ImGui.TextWrapped( $"This item does not have a VFX. See the link below for information on adding one" );
            ImGui.PopStyleColor();
            if( ImGui.SmallButton( "Guide##NoVfx" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Adding-a-VFX-to-an-Item-Without-One" );
        }

        public static bool Matches( string item, string query ) => item.ToLower().Contains( query.ToLower() );

        public static void DisplayPath( string path ) {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.8f, 0.8f, 0.8f, 1 ) );
            ImGui.TextWrapped( path );
            ImGui.PopStyleColor();
        }

        public static void DisplayPathWarning( string path, string warning ) {
            ImGui.PushStyleColor( ImGuiCol.Text, UiUtils.YELLOW_COLOR );
            ImGui.TextWrapped( $"{path} (!)" );
            ImGui.PopStyleColor();
            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos( ImGui.GetFontSize() * 35.0f );
                ImGui.TextUnformatted( warning );
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public static void Copy( string copyPath, string id ) {
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.15f, 0.15f, 0.15f, 1 ) );
            if( ImGui.Button( "Copy" + id ) ) ImGui.SetClipboardText( copyPath );
            ImGui.PopStyleColor();
        }

        public static void DisplayVisible( int count, out int preItems, out int showItems, out int postItems, out float itemHeight ) {
            var childHeight = ImGui.GetWindowSize().Y - ImGui.GetCursorPosY();
            var scrollY = ImGui.GetScrollY();
            var style = ImGui.GetStyle();
            itemHeight = ImGui.GetTextLineHeight() + style.ItemSpacing.Y;
            preItems = ( int )Math.Floor( scrollY / itemHeight );
            showItems = ( int )Math.Ceiling( childHeight / itemHeight );
            postItems = count - showItems - preItems;
        }

        public static void DrawIcon( ImGuiScene.TextureWrap icon ) {
            if( icon != null && icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( icon.ImGuiHandle, new Vector2( icon.Width, icon.Height ) );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            }
        }

        public static void NpcThankYou() {
            ImGui.TextDisabled( "Npc file list provided by ResLogger" );
            ImGui.SameLine();
            if( ImGui.SmallButton( "Github##ResLogger" ) ) UiUtils.OpenUrl( "https://github.com/lmcintyre/ResLogger2" );
        }

        public static byte[] BgraToRgba( byte[] data ) {
            var ret = new byte[data.Length];
            for( var i = 0; i < data.Length / 4; i++ ) {
                var idx = i * 4;
                ret[idx + 0] = data[idx + 2];
                ret[idx + 1] = data[idx + 1];
                ret[idx + 2] = data[idx + 0];
                ret[idx + 3] = data[idx + 3];
            }
            return ret;
        }
    }
}
