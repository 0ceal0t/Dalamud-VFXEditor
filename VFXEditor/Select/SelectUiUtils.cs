using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Select {
    public class SelectUiUtils {
        public static SelectResult GetSelectResult( string path, SelectResultType resultType, string resultName ) {
            var resultPrefix = resultType.ToString().ToUpper().Replace( "GAME", "" );
            return new SelectResult( resultType, resultName, $"[{resultPrefix}] {resultName}", path );
        }

        public static void DisplayNoVfx() {
            using( var style = ImRaii.PushColor( ImGuiCol.Text, UiUtils.DALAMUD_ORANGE ) ) {
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                ImGui.TextWrapped( $"This item does not have a VFX. See the link below for information on adding one" );
            }
            UiUtils.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Adding-a-VFX-to-an-Item-Without-One" );
        }

        public static bool Matches( string item, string query ) => item.Contains( query, StringComparison.CurrentCultureIgnoreCase );

        public static void DisplayPath( string path ) {
            using var style = ImRaii.PushColor( ImGuiCol.Text, new Vector4( 0.8f, 0.8f, 0.8f, 1 ) );
            ImGui.TextWrapped( path );
        }

        public static void DisplayPathWarning( string path, string warning ) {
            using( var style = ImRaii.PushColor( ImGuiCol.Text, UiUtils.DALAMUD_ORANGE ) ) {
                ImGui.TextWrapped( path );
            }
            UiUtils.Tooltip( warning );
        }

        public static void Copy( string path ) {
            using var style = ImRaii.PushColor( ImGuiCol.Button, UiUtils.DARK_GRAY );
            using var text = ImRaii.PushColor( ImGuiCol.Text, new Vector4( 0.8f, 0.8f, 0.8f, 1 ) );
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( FontAwesomeIcon.ClipboardList.ToIconString() ) ) ImGui.SetClipboardText( path );
        }

        public static void DisplayVisible( int count, float itemHeight, out int preItems, out int showItems, out int postItems ) {
            preItems = ( int )Math.Floor( ImGui.GetScrollY() / itemHeight );
            showItems = ( int )Math.Ceiling( ( ImGui.GetWindowSize().Y - ImGui.GetCursorPosY() ) / itemHeight );
            postItems = count - showItems - preItems;
        }

        public static void NpcThankYou() {
            ImGui.TextDisabled( "Npc file list provided by ResLogger" );
            ImGui.SameLine();
            if( ImGui.SmallButton( "Github##ResLogger" ) ) UiUtils.OpenUrl( "https://github.com/lmcintyre/ResLogger2" );
        }
    }
}
