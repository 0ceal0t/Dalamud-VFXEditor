using ImGuiNET;
using System;
using System.IO;
using VfxEditor;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxBool : AvfxDrawable {
        public readonly ParsedBool Parsed;

        public AvfxBool( string name, string avfxName, int size = 4 ) : base( avfxName ) {
            Parsed = new( name, size );
        }

        public bool? GetValue() => Parsed.Value;

        public void SetValue( bool? value ) {
            SetAssigned( true );
            Parsed.Value = value;
        }

        public override void ReadContents( BinaryReader reader, int size ) => Parsed.Read( reader, size );

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) => Parsed.Write( writer );

        public override void Draw( string id ) {
            // Unassigned
            if( DrawAddButton( this, Parsed.Name, id ) ) return;

            Parsed.Draw( id, CommandManager.Avfx );

            DrawRemoveContextMenu( this, Parsed.Name, id );
        }
    }
}
