using ImGuiNET;
using OtterGui.Raii;
using System.Numerics;

namespace VfxEditor.Ui.Tools {
    public unsafe class LuaTab {
        public void Draw() {
            using var _ = ImRaii.PushId( "Lua" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Pool 1" ) ) {
                if( tab ) DrawPool( 1, 64 );
            }

            using( var tab = ImRaii.TabItem( "Pool 2" ) ) {
                if( tab ) DrawPool( 2, 32 );
            }

            using( var tab = ImRaii.TabItem( "Pool 3" ) ) {
                if( tab ) DrawPool( 3, 32 );
            }
        }

        private void DrawPool( int pool, int numVar ) {
            using var _ = ImRaii.PushId( pool );

            using var child = ImRaii.Child( "Child", new Vector2( -1 ), false );

            using var table = ImRaii.Table( "Table", 3, ImGuiTableFlags.RowBg );
            if( !table ) return;

            ImGui.TableSetupColumn( "Index", ImGuiTableColumnFlags.WidthFixed, 50 );
            ImGui.TableSetupColumn( "Int Value", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "Hex Value", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableHeadersRow();

            for( var i = 0; i < numVar; i++ ) {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.TextDisabled( $"{i}" );

                var value = ( ( uint )pool << 28 ) | ( ( uint )i );
                var varValue = Plugin.ResourceLoader.GetLuaVariable( Plugin.ResourceLoader.LuaManager, value );

                ImGui.TableNextColumn();
                ImGui.Text( $"{varValue}" );

                ImGui.TableNextColumn();
                ImGui.Text( $"0x{varValue:X8}" );
            }
        }
    }
}
