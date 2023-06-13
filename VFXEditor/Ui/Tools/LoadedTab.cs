using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VfxEditor.Structs;

namespace VfxEditor.Ui.Tools {
    // Kinda scuffed, but it (mostly) works

    public unsafe class LoadedTab {
        public void Draw() {
            using var child = ImRaii.Child( "LoadedChild" );

            foreach( var item in Plugin.Objects ) {
                var gameObject = ( GameObject* )item.Address;
                if( gameObject == null ) continue;

                var id = gameObject->ObjectID;
                if( id == 0 ) continue;

                using var _ = ImRaii.PushId( $"{id:X8}" );
                if( !ImGui.CollapsingHeader( $"{item.Name}" ) ) continue;

                using var indent = ImRaii.PushIndent();

                ImGui.TextDisabled( $"[{id:X8}] 0x{item.Address:X8}" );
                ImGui.SameLine();
                if( ImGui.SmallButton( "Copy" ) ) ImGui.SetClipboardText( $"{item.Address:X8}" );

                var drawObject = gameObject->DrawObject;
                if( drawObject == null ) continue;

                var data = Marshal.ReadIntPtr( new IntPtr( drawObject ) + 160 );
                if( data == IntPtr.Zero ) continue;

                var sklbTable = Marshal.ReadIntPtr( data + 88 );
                var tableStart = Marshal.ReadIntPtr( data + 96 );

                DrawTable( sklbTable, IntPtr.Zero, "SKLB" );

                var idx = 0;
                var tablePos = tableStart;
                var tablePtr = Marshal.ReadIntPtr( tablePos );

                while( tablePtr > 256 ) {
                    if( tablePtr != 0x3F800000 ) { // -1 = skip
                        var name = $"TABLE {idx}";

                        using var __ = ImRaii.PushId( name );

                        using var tree = ImRaii.TreeNode( name );
                        if( tree ) {
                            DrawResource( Marshal.ReadIntPtr( tablePtr ) );
                            DrawMultiTable( tablePtr + 8, "PAP" );
                        }
                    }

                    idx++;
                    tablePos += 8;
                    tablePtr = Marshal.ReadIntPtr( tablePos );
                }
            }
        }

        private void DrawMultiTable( IntPtr tableStart, string prefix ) {
            var tablePos = tableStart;
            var tablePtr = Marshal.ReadIntPtr( tablePos );

            var tablePtrs = new List<IntPtr>();
            while( tablePtr > 256 ) {
                if( tablePtr != 0x3F800000 ) { // -1 = skip
                    tablePtrs.Add( tablePtr );
                }
                tablePos += 8;
                tablePtr = Marshal.ReadIntPtr( tablePos );
            }

            for( var i = 0; i < tablePtrs.Count - 2; i++ ) {
                if( i == 5 ) continue; // for shaders and other misc stuff. format still WIP
                DrawTable( tablePtrs[i], tablePtrs[i + 1], $"{prefix} {i}" );
            }
        }

        private void DrawTable( IntPtr tablePtr, IntPtr nextTable, string name ) {
            if( tablePtr <= 256 ) return;

            using var _ = ImRaii.PushId( name );

            using var tree = ImRaii.TreeNode( name );
            if( !tree ) return;

            var idx = 0;
            var resourcePos = tablePtr;
            var resourcePtr = Marshal.ReadIntPtr( resourcePos );

            while( resourcePtr > 256 && !Equals( resourcePos, nextTable ) ) {
                try {
                    using var __ = ImRaii.PushId( $"Resource{idx}" );
                    DrawResource( resourcePtr );
                }
                catch( Exception ) {
                    PluginLog.Error( $"{resourcePtr:X8} {resourcePos:X8} {tablePtr:X8} {nextTable:X8}" );
                }

                idx++;
                resourcePos += 8;
                resourcePtr = Marshal.ReadIntPtr( resourcePos );
            }
        }

        private void DrawResource( IntPtr resourcePtr ) {
            if( resourcePtr <= 256 || resourcePtr == 0x3F800000 ) return;

            var resource = ( ResourceHandle* )resourcePtr;

            if( resource->FileNamePtr() == null ) return;

            var fileName = resource->FileName().ToString();
            if( string.IsNullOrEmpty( fileName ) ) return;

            ImGui.Text( fileName );
            if( ImGui.IsItemClicked() ) ImGui.SetClipboardText( fileName );
        }
    }
}
