using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Copy;
using VfxEditor.Formats.AvfxFormat.Assign;
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
                var commands = new List<ICommand> {
                    new AvfxAssignCommand( this, true, false, true )
                };
                foreach( var item in items ) commands.Add( new AvfxAssignCommand( item, true, false, true ) );
                CommandManager.Add( new CompoundCommand( commands ) );
            }
            return true;
        }

        public void DrawUnassignPopup<T>( List<T> items, string name ) where T : AvfxBase {
            if( UnassignPopup( name ) ) {
                var commands = new List<ICommand> {
                    new AvfxAssignCommand( this, false, false, true )
                };
                foreach( var item in items ) commands.Add( new AvfxAssignCommand( item, false, false, true ) );
                CommandManager.Add( new CompoundCommand( commands ) );
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

            writer.BaseStream.Position = sizePos;
            writer.Write( ( int )size );

            writer.BaseStream.Position = endPos;
        }

        public abstract void WriteContents( BinaryWriter writer );
    }
}
