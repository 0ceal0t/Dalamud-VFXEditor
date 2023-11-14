using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Copy;
using VfxEditor.Formats.AvfxFormat.Assign;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract partial class AvfxBase {
        protected readonly string AvfxName;
        protected bool Assigned = true;

        public AvfxBase( string avfxName ) {
            AvfxName = avfxName;
        }

        public string GetAvfxName() => AvfxName;

        public bool IsAssigned() => Assigned;

        protected abstract IEnumerable<AvfxBase> GetChildren();

        public virtual void SetAssigned( bool assigned, bool recurse = false ) {
            Assigned = assigned;
        }

        protected void ResetChildren() {
            foreach( var child in GetChildren() ) {
                child?.SetAssigned( false );
                child?.ResetChildren();
            }
        }

        public void AssignedCopyPaste( string name ) {
            CopyManager.TrySetAssigned( this, name );
            if( CopyManager.TryGetAssigned( this, name, out var val ) ) {
                CommandManager.Paste( new AvfxAssignCommand( this, val, false, false ) );
            }
        }

        public bool DrawAssignButton( string name, bool recurse = false ) {
            if( IsAssigned() ) return false;

            if( name.StartsWith( "##" ) ) {
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) CommandManager.Add( new AvfxAssignCommand( this, true, recurse, false ) );
            }
            else {
                if( ImGui.SmallButton( $"+ {name}" ) ) CommandManager.Add( new AvfxAssignCommand( this, true, recurse, false ) );
            }
            return true;
        }

        public void DrawUnassignPopup( string name ) {
            if( UnassignPopup( name ) ) CommandManager.Add( new AvfxAssignCommand( this, false, false, false ) );
        }

        public bool DrawUnassignButton( string name ) {
            if( UiUtils.RemoveButton( $"Delete {name}", small: true ) ) {
                CommandManager.Add( new AvfxAssignCommand( this, false, false, false ) );
                return true;
            }
            return false;
        }

        // ===== USED FOR WEIRD CASES LIKE UIFLOAT2 / UIFLOAT3 ======

        public bool DrawAssignButton<T>( List<T> items, string name ) where T : AvfxBase {
            if( IsAssigned() ) return false;

            if( ImGui.SmallButton( $"+ {name}" ) ) {
                var command = new CompoundCommand();
                command.Add( new AvfxAssignCommand( this, true, false, true ) );
                foreach( var item in items ) command.Add( new AvfxAssignCommand( item, true, false, true ) );
                CommandManager.Add( command );
            }
            return true;
        }

        public void DrawUnassignPopup<T>( List<T> items, string name ) where T : AvfxBase {
            if( UnassignPopup( name ) ) {
                var command = new CompoundCommand();
                command.Add( new AvfxAssignCommand( this, false, false, true ) );
                foreach( var item in items ) command.Add( new AvfxAssignCommand( item, false, false, true ) );
                CommandManager.Add( command );
            }
        }

        // ==== PARSING =====

        public virtual void Read( BinaryReader reader, int size ) {
            Assigned = true;
            ResetChildren();
            ReadContents( reader, size );
        }

        public abstract void ReadContents( BinaryReader reader, int size ); // size is the contents size (does not include the name and size of this element)

        public void Write( BinaryWriter writer ) {
            if( !Assigned ) return;

            WriteAvfxName( writer, AvfxName );

            var sizePos = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder

            WriteContents( writer );

            var endPos = writer.BaseStream.Position;
            var size = endPos - sizePos - 4;

            WritePad( writer, CalculatePadding( ( int )size ) );

            endPos = writer.BaseStream.Position;

            writer.BaseStream.Seek( sizePos, SeekOrigin.Begin );
            writer.Write( ( int )size );

            writer.BaseStream.Seek( endPos, SeekOrigin.Begin );
        }

        public abstract void WriteContents( BinaryWriter writer );

        // ========= STATIC DRAWING =============

        private static bool UnassignPopup( string name ) {
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( $"Unassign/{name}" );

            using var popup = ImRaii.Popup( $"Unassign/{name}" );
            if( popup ) {
                if( ImGui.Selectable( $"Unassign {name.TrimStart( '#' )}" ) ) {
                    ImGui.CloseCurrentPopup();
                    return true;
                }
            }

            return false;
        }

        public static void DrawNamedItems<T>( List<T> items ) where T : INamedUiItem {
            SplitUnassigned( items, out var assigned, out var unassigned );

            if( unassigned.Count > 0 ) {
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                for( var idx = 0; idx < unassigned.Count; idx++ ) {
                    if( idx > 0 ) ImGui.SameLine();
                    unassigned[idx].Draw();
                }
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            }

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            foreach( var item in assigned ) {
                using var tabBarItem = ImRaii.TabItem( $"{item.GetText()}" );
                if( tabBarItem ) item.Draw();
            }
        }

        public static void DrawItems<T>( List<T> items ) where T : IUiItem {
            SplitUnassigned( items, out var assigned, out var unassigned );

            assigned.ForEach( x => x.Draw() );
            unassigned.ForEach( x => x.Draw() );
        }

        public static void SplitUnassigned<T>( List<T> items, out List<T> assigned, out List<T> unassigned ) {
            assigned = new();
            unassigned = new();
            foreach( var item in items ) {
                if( item is AvfxOptional optionalItem && !optionalItem.IsAssigned() ) unassigned.Add( item );
                else assigned.Add( item );
            }
        }
    }
}
