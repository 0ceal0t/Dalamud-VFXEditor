using ImGuiNET;
using System.IO;
using VfxEditor;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxInt : AvfxDrawable {
        public readonly ParsedInt Parsed;

        public AvfxInt( string name, string avfxName, int size = 4 ) : base( avfxName ) {
            Parsed = new( name, size );
        }

        public int GetValue() => Parsed.Value;

        public void SetValue( int value ) {
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
