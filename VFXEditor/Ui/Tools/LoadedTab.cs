using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VfxEditor.Structs;

namespace VfxEditor.Ui.Tools {
    public unsafe class LoadedTab {
        public void Draw() {
            // Kinda scuffed, but it (mostly) works
            using var child = ImRaii.Child( "LoadedChild" );

            foreach( var item in Dalamud.Objects ) {
                var gameObject = ( GameObject* )item.Address;
                if( gameObject == null ) continue;

                var id = gameObject->EntityId;
                if( id == 0 ) continue;

                var drawObject = gameObject->DrawObject;
                if( drawObject == null ) continue;
                if( drawObject->Object.GetObjectType() != ObjectType.CharacterBase ) return;

                using var _ = ImRaii.PushId( $"{item.Address:X8}" );
                if( !ImGui.CollapsingHeader( $"{item.Name}" ) ) continue;

                using var indent = ImRaii.PushIndent();

                ImGui.TextDisabled( $"[{id:X8}] 0x{item.Address:X8}" );
                ImGui.SameLine();
                if( ImGui.SmallButton( "Copy" ) ) ImGui.SetClipboardText( $"{item.Address:X8}" );

                DrawCharacterBase( ( CharacterBase* )drawObject );

                var childObject = drawObject->Object.ChildObject;
                if( childObject != null && childObject->GetObjectType() == ObjectType.CharacterBase ) {
                    using var __ = ImRaii.PushId( "Child Object" );

                    using var tree = ImRaii.TreeNode( "Child Object" );
                    if( !tree ) continue;

                    using var indent2 = ImRaii.PushIndent();

                    DrawCharacterBase( ( CharacterBase* )childObject );
                }
            }
        }

        private static void DrawCharacterBase( CharacterBase* characterBase ) {
            var skeleton = characterBase->Skeleton;
            if( skeleton == null ) return;

            var sklbTable = new IntPtr( skeleton->SkeletonResourceHandles );
            var animationTable = new IntPtr( skeleton->AnimationResourceHandles );

            DrawTable( sklbTable, IntPtr.Zero, "SKLB", true );

            if( skeleton->PartialSkeletonCount == 0 || Marshal.ReadIntPtr( new IntPtr( skeleton ) + 112 ) == IntPtr.Zero ) return;

            for( var idx = 0; idx < skeleton->PartialSkeletonCount; idx++ ) {
                var tablePos = animationTable + ( 8 * idx );
                var tablePtr = Marshal.ReadIntPtr( tablePos );

                if( tablePtr == 0x3F800000 || tablePtr == 0 ) continue;

                var name = $"TABLE {idx}";
                using var __ = ImRaii.PushId( name );
                using var tree = ImRaii.TreeNode( name );
                if( tree ) {
                    if( GetResource( Marshal.ReadIntPtr( tablePtr ), out var fileName ) ) DrawResource( fileName );
                    DrawAnimationTable( tablePtr + 8, "PAP" );
                }
            }
        }

        private static void DrawAnimationTable( IntPtr tableStart, string prefix ) {
            var idx = 0;

            for( var i = 0; i < 2; i++ ) {
                var tablePos = tableStart + ( 120 * i );

                for( var j = 0; j < 5; j++ ) {
                    var start = Marshal.ReadIntPtr( tablePos );
                    var end = Marshal.ReadIntPtr( tablePos + 8 );

                    DrawTable( start, end, $"{prefix} {idx}", false );
                    idx++;

                    tablePos += 24;
                }
            }
        }

        private static void DrawTable( IntPtr tablePtr, IntPtr nextTable, string name, bool stopAtZero ) {
            using var _ = ImRaii.PushId( name );

            using var tree = ImRaii.TreeNode( name );
            if( !tree ) return;

            var resources = GetResourcesFromTable( tablePtr, nextTable, stopAtZero );
            for( var idx = 0; idx < resources.Count; idx++ ) {
                using var __ = ImRaii.PushId( idx );
                DrawResource( resources[idx] );
            }
        }

        public static List<string> GetResourcesFromTable( IntPtr tablePtr, IntPtr nextTable, bool stopAtZero ) {
            var ret = new List<string>();
            if( tablePtr == IntPtr.Zero ) return ret;

            var resourcePos = tablePtr;
            var resourcePtr = Marshal.ReadIntPtr( resourcePos );

            while( ( !stopAtZero || resourcePtr != IntPtr.Zero ) && !Equals( resourcePos, nextTable ) ) {
                try {
                    if( GetResource( resourcePtr, out var fileName ) ) ret.Add( fileName );
                }
                catch( Exception ) {
                    Dalamud.Error( $"{resourcePtr:X8} {resourcePos:X8} {tablePtr:X8} {nextTable:X8}" );
                }

                resourcePos += 8;
                resourcePtr = Marshal.ReadIntPtr( resourcePos );
            }

            return ret;
        }

        private static bool GetResource( IntPtr resourcePtr, out string fileName ) {
            fileName = "";

            if( resourcePtr <= 256 || resourcePtr == 0x3F800000 ) return false;

            var resource = ( ResourceHandle* )resourcePtr;
            if( resource->FileName().IsEmpty ) return false;

            fileName = resource->FileName().ToString();
            if( string.IsNullOrEmpty( fileName ) ) return false;

            return true;
        }

        private static void DrawResource( string fileName ) {
            ImGui.Text( fileName );
            if( ImGui.IsItemClicked() ) ImGui.SetClipboardText( fileName );
        }
    }
}
