using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace VfxEditor.Ui.Tools {
    public unsafe class LuaTab {
        public readonly static Dictionary<int, string> NamesPool1 = new() {
            { 0, "Game Language" },
            { 1, "Caption Language" },
            { 3, "Skeleton Id" },
            { 2, "Voice Language" },
            { 4, "Starting Town" },
            { 16, "Is Legacy Player" },
            { 18, "Class/Job" },
            { 35, "Is Gatherer" },
            { 36, "Is Crafter" }
        };

        public readonly static Dictionary<int, string> NamesPool2 = new();

        public readonly static Dictionary<int, string> NamesPool3 = new();

        public void Draw() {
            using var _ = ImRaii.PushId( "Lua" );

            var manager = Marshal.ReadIntPtr( Plugin.ResourceLoader.LuaManager );

            ImGui.TextDisabled( $"0x{manager:X8}" );
            if( ImGui.IsItemClicked() ) ImGui.SetClipboardText( $"{manager:X8}" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Pool 1" ) ) {
                if( tab ) DrawPool( 1, 64, NamesPool1, manager );
            }

            using( var tab = ImRaii.TabItem( "Pool 2" ) ) {
                if( tab ) DrawPool( 2, 32, NamesPool2, manager );
            }

            using( var tab = ImRaii.TabItem( "Pool 3" ) ) {
                if( tab ) DrawPool( 3, 32, NamesPool3, manager );
            }
        }

        private void DrawPool( int pool, int numVar, Dictionary<int, string> names, IntPtr manager ) {
            using var _ = ImRaii.PushId( pool );

            using var child = ImRaii.Child( "Child", new Vector2( -1 ), false );

            using var table = ImRaii.Table( "Table", 4, ImGuiTableFlags.RowBg );
            if( !table ) return;

            ImGui.TableSetupColumn( "Index", ImGuiTableColumnFlags.WidthFixed, 50 );
            ImGui.TableSetupColumn( "Name", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "Current Value", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "Hex", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableHeadersRow();

            for( var i = 0; i < numVar; i++ ) {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.TextDisabled( $"{i}" );

                var value = ( ( uint )pool << 28 ) | ( ( uint )i );
                var varValue = Plugin.ResourceLoader.GetLuaVariable( manager, value );

                ImGui.TableNextColumn();
                ImGui.Text( names.TryGetValue( i, out var name ) ? name : "" );

                ImGui.TableNextColumn();
                ImGui.Text( $"{varValue}" );

                ImGui.TableNextColumn();
                ImGui.Text( $"0x{varValue:X8}" );
            }
        }
    }
}
