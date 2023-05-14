using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxFloat : AvfxDrawable {
        public readonly ParsedFloat Parsed;

        public AvfxFloat( string name, string avfxName, float defaultValue ) : this( name, avfxName ) {
            SetValue( defaultValue );
        }

        public AvfxFloat( string name, string avfxName ) : base( avfxName ) {
            Parsed = new( name );
        }

        public float GetValue() => Parsed.Value;

        public void SetValue( float value ) {
            SetAssigned( true );
            Parsed.Value = value;
        }

        public override void ReadContents( BinaryReader reader, int _ ) => Parsed.Read( reader, _ );

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) => Parsed.Write( writer );

        public override void Draw() {
            // Unassigned
            AssignedCopyPaste( this, Parsed.Name );
            if( DrawAddButton( this, Parsed.Name ) ) return;

            Parsed.Draw( CommandManager.Avfx );

            DrawRemoveContextMenu( this, Parsed.Name );
        }
    }
}
