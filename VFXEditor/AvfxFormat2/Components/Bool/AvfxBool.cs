using ImGuiNET;
using System;
using System.IO;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxBool : AvfxDrawable {
        public readonly string Name;
        private int Size;
        private bool? Value = false;

        public AvfxBool( string name, string avfxName, int size = 4 ) : base( avfxName ) {
            Name = name;
            Size = size;
        }

        public bool? GetValue() => Value;

        public void SetValue( bool? value ) {
            SetAssigned( true );
            Value = value;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var b = reader.ReadByte();
            Value = b switch {
                0x00 => false,
                0x01 => true,
                0xff => null,
                _ => null
            };
            Size = size;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            byte v = Value switch {
                true => 0x01,
                false => 0x00,
                null => 0xff
            };
            writer.Write( v );
            WritePad( writer, Size - 1 );
        }

        public override void Draw( string id ) {
            // Unassigned
            if( DrawAddButton( this, Name, id ) ) return;

            var value = Value == true;
            if( ImGui.Checkbox( Name + id, ref value ) ) CommandManager.Avfx.Add( new AvfxBoolCommand( this, value ) );

            DrawRemoveContextMenu( this, Name, id );
        }
    }
}
