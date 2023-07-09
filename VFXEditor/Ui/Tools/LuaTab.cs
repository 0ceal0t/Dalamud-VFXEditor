using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using static VfxEditor.Interop.ResourceLoader;

namespace VfxEditor.Ui.Tools {
    public unsafe class LuaTab {
        public static readonly Dictionary<int, string> NamesPool1 = new() {
            { 0x0, "[CUTSCENE] Game Language" },
            { 0x1, "[CUTSCENE] Caption Language" },
            { 0x2, "[CUTSCENE] Voice Language" },
            { 0x3, "[CUTSCENE] Skeleton Id" },
            { 0x4, "[CUTSCENE] Starting Town" },
            { 0x10, "[CUTSCENE] Legacy Player" },
            { 0x12, "[CUTSCENE] Class/Job" },
            { 0x13, "Is Player Character" },
            { 0x23, "[CUTSCENE] Is Gatherer" },
            { 0x24, "[CUTSCENE] Is Crafter" },
            { 0x25, "Is Mount" },
            { 0x26, "Mounted" },
            { 0x27, "Swimming" },
            { 0x28, "Underwater" },
            { 0x29, "Class/Job" },
            { 0x33, "Mount Id" },
            { 0x39, "GPose" },
        };

        public static readonly Dictionary<int, string> NamesPool2 = new();

        public static readonly Dictionary<int, string> NamesPool3 = new();

        private uint ObjectId = 0;

        public void Draw() {
            using var _ = ImRaii.PushId( "Lua" );

            var manager = Marshal.ReadIntPtr( Plugin.ResourceLoader.LuaManager );
            if( manager == IntPtr.Zero ) return;

            var objectAddress = IntPtr.Zero;
            var objectName = "";

            foreach( var item in Plugin.Objects ) {
                if( item.ObjectId == ObjectId ) {
                    objectAddress = item.Address;
                    objectName = GetObjectName( item );
                    break;
                }
            }

            if( objectAddress == 0 ) {
                ObjectId = 0; // couldn't find object, reset
                objectName = "[NONE]";

                // Try to reset back to player character
                if( Plugin.PlayerObject != null && Plugin.PlayerObject.Address != IntPtr.Zero ) {
                    ObjectId = Plugin.PlayerObject.ObjectId;
                    objectAddress = Plugin.PlayerObject.Address;
                    objectName = GetObjectName( Plugin.PlayerObject );
                }
            }

            ImGui.TextDisabled( $"Pools: 0x{manager:X8}" );
            if( ImGui.IsItemClicked() ) ImGui.SetClipboardText( $"{manager:X8}" );

            ImGui.SameLine();
            ImGui.TextDisabled( $"Dynamic: 0x{Plugin.ResourceLoader.LuaActorVariables:X8}" );
            if( ImGui.IsItemClicked() ) ImGui.SetClipboardText( $"{Plugin.ResourceLoader.LuaActorVariables:X8}" );

            DrawCombo( objectName );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Pool 1" ) ) {
                if( tab ) DrawPool( 1, 64, NamesPool1, manager, objectAddress );
            }

            using( var tab = ImRaii.TabItem( "Pool 2" ) ) {
                if( tab ) DrawPool( 2, 32, NamesPool2, manager, objectAddress );
            }

            using( var tab = ImRaii.TabItem( "Pool 3" ) ) {
                if( tab ) DrawPool( 3, 32, NamesPool3, manager, objectAddress );
            }
        }

        private void DrawCombo( string objectName ) {
            using var combo = ImRaii.Combo( "##Combo", objectName );
            if( !combo ) return;

            foreach( var item in Plugin.Objects ) {
                var name = GetObjectName( item );

                if( ImGui.Selectable( $"{name}##{item.ObjectId}", item.ObjectId == ObjectId ) ) {
                    ObjectId = item.ObjectId;
                }
            }
        }

        private string GetObjectName( GameObject item ) {
            var name = item.Name.ToString();
            if( !string.IsNullOrEmpty( name ) ) return name;
            return $"[0x{item.ObjectId:X8}]";
        }

        private void DrawPool( int pool, int numVar, Dictionary<int, string> names, IntPtr manager, IntPtr objectAddress ) {
            using var _ = ImRaii.PushId( pool );

            using var child = ImRaii.Child( "Child", new Vector2( -1 ), false );

            using var table = ImRaii.Table( "Table", 4, ImGuiTableFlags.RowBg );
            if( !table ) return;

            ImGui.TableSetupColumn( "Index", ImGuiTableColumnFlags.WidthFixed, 60 );
            ImGui.TableSetupColumn( "Name", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "Current Value", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "Hex", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableHeadersRow();

            for( var i = 0; i < numVar; i++ ) {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.TextDisabled( $"[0x{i:X2}]  {i}" );

                var value = ( ( uint )pool << 28 ) | ( ( uint )i );
                var varValue = GetVariableValue( value, manager, objectAddress );

                ImGui.TableNextColumn();
                ImGui.Text( names.TryGetValue( i, out var name ) ? name : "" );

                ImGui.TableNextColumn();
                ImGui.Text( $"{varValue}" );

                ImGui.TableNextColumn();
                ImGui.Text( $"0x{varValue:X8}" );
            }
        }

        private uint GetVariableValue( uint value, IntPtr manager, IntPtr objectAddress ) {
            if( objectAddress == IntPtr.Zero ) return Plugin.ResourceLoader.GetLuaVariable( manager, value );

            return value switch {
                0x10000013 => GetActorVariableValue( value, manager, objectAddress ),
                0x10000025 => GetActorVariableValue( value, manager, objectAddress ),
                0x10000026 => GetActorVariableValue( value, manager, objectAddress ),
                0x10000027 => GetActorVariableValue( value, manager, objectAddress ),
                0x10000028 => GetActorVariableValue( value, manager, objectAddress ),
                0x10000029 => GetActorVariableValue( value, manager, objectAddress ),
                0x10000033 => GetActorVariableValue( value, manager, objectAddress ),
                0x10000034 => GetActorVariableValue( value, manager, objectAddress ),
                0x10000039 => GetActorVariableValue( value, manager, objectAddress ),
                _ => Plugin.ResourceLoader.GetLuaVariable( manager, value )
            };
        }

        private static uint GetActorVariableValue( uint value, IntPtr manager, IntPtr objectAddress ) {
            var pos = Plugin.ResourceLoader.LuaActorVariables;

            for( var i = 0; i < 21; i++ ) {
                var posValue = Marshal.ReadIntPtr( pos );
                if( posValue == value ) {
                    var funcLocation = Marshal.ReadIntPtr( pos + 8 );
                    var actorFunc = Marshal.GetDelegateForFunctionPointer<LuaActorVariableDelegate>( funcLocation );
                    return actorFunc( objectAddress );
                }
                pos += 8;
            }

            return Plugin.ResourceLoader.GetLuaVariable( manager, value );
        }
    }
}
