using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.TmbFormat.Root;
using static VfxEditor.Interop.ResourceLoader;

namespace VfxEditor.Ui.Tools {
    public unsafe class LuaTab {
        private ulong ObjectId = 0;

        public void Draw() {
            using var _ = ImRaii.PushId( "Lua" );

            var manager = Marshal.ReadIntPtr( Plugin.ResourceLoader.LuaManager );
            if( manager == IntPtr.Zero ) return;

            var objectAddress = IntPtr.Zero;
            var objectName = "";

            foreach( var item in Dalamud.Objects ) {
                if( item.GameObjectId == ObjectId ) {
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
                    ObjectId = Plugin.PlayerObject.GameObjectId;
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

            foreach( var pool in LuaPool.Pools ) {
                using var tab = ImRaii.TabItem( $"Pool {pool.Id}" );
                if( tab ) DrawPool( pool, manager, objectAddress );
            }
        }

        private void DrawCombo( string objectName ) {
            using var combo = ImRaii.Combo( "##Combo", objectName );
            if( !combo ) return;

            foreach( var item in Dalamud.Objects ) {
                if( item.ObjectKind != ObjectKind.Player &&
                    item.ObjectKind != ObjectKind.MountType &&
                    item.ObjectKind != ObjectKind.EventNpc &&
                    item.ObjectKind != ObjectKind.Companion &&
                    item.ObjectKind != ObjectKind.BattleNpc ) continue;
                if( item.GameObjectId == 0xE0000000 ) continue;

                var name = GetObjectName( item );
                if( ImGui.Selectable( $"{name}##{item.GameObjectId}", item.GameObjectId == ObjectId ) ) {
                    ObjectId = item.GameObjectId;
                }
            }
        }

        private static string GetObjectName( IGameObject item ) {
            var name = item.Name.ToString();
            if( !string.IsNullOrEmpty( name ) ) return name;
            return $"[0x{item.GameObjectId:X4}]";
        }

        private static void DrawPool( LuaPool pool, IntPtr manager, IntPtr objectAddress ) {
            using var _ = ImRaii.PushId( pool.Id );

            using var child = ImRaii.Child( "Child", new Vector2( -1 ), false );

            using var table = ImRaii.Table( "Table", 4, ImGuiTableFlags.RowBg );
            if( !table ) return;

            ImGui.TableSetupColumn( "Index", ImGuiTableColumnFlags.WidthFixed, 60 );
            ImGui.TableSetupColumn( "Name", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "Current Value", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "Hex", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableHeadersRow();

            for( var i = 0; i < pool.Size; i++ ) {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.TextDisabled( $"[0x{i:X2}]  {i}" );

                var value = ( ( uint )pool.Id << 28 ) | ( ( uint )i );
                var varValue = GetVariableValue( value, manager, objectAddress );

                ImGui.TableNextColumn();
                ImGui.Text( pool.Names.TryGetValue( i, out var name ) ? name : "" );

                ImGui.TableNextColumn();
                ImGui.Text( $"{varValue}" );

                ImGui.TableNextColumn();
                ImGui.Text( $"0x{varValue:X4}" );
            }
        }

        // 48 89 5C 24 ? 48 89 74 24 ? 57 48 83 EC 20 44 8B 0D ? ? ? ? 48 8B F9 65 48 8B 04 25 ? ? ? ? 49 8B F0

        private static uint GetVariableValue( uint value, IntPtr manager, IntPtr objectAddress ) {
            if( objectAddress == IntPtr.Zero ) return Plugin.ResourceLoader.GetLuaVariable( manager, value );

            return value switch {
                0x10000013 or
                0x10000025 or
                0x10000026 or
                0x10000027 or
                0x10000028 or
                0x10000029 or
                0x10000033 or
                0x10000034 or
                0x10000039 or
                0x1000003A or
                0x1000003C or
                0x1000003B or
                0x1000003D
                => GetActorVariableValue( value, manager, objectAddress ),
                _ => Plugin.ResourceLoader.GetLuaVariable( manager, value )
            };
        }

        private static uint GetActorVariableValue( uint value, IntPtr manager, IntPtr objectAddress ) {
            var pos = Plugin.ResourceLoader.LuaActorVariables;

            for( var i = 0; i < 30; i++ ) {
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
