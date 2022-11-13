using ImGuiNET;
using System.IO;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxInt : AvfxDrawable {
        public readonly string Name;
        private int Size;
        private int Value = 0;

        public AvfxInt( string name, string avfxName, int size = 4 ) : base( avfxName ) {
            Name = name;
            Size = size;
        }

        public int GetValue() => Value;

        public void SetValue( int value ) {
            SetAssigned( true );
            Value = value;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Size = size;
            Value = Size == 4 ? reader.ReadInt32() : reader.ReadByte();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            if( Size == 4 ) writer.Write( Value );
            else writer.Write( ( byte )Value );
        }

        public override void Draw( string id ) {
            // Unassigned
            if( DrawAddButton( this, Name, id ) ) return;

            var value = Value;
            if( ImGui.InputInt( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new AvfxIntCommand( this, value ) );
            }

            DrawRemoveContextMenu( this, Name, id );
        }
    }
}
