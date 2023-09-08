using System;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxEnum<T> : AvfxDrawable where T : Enum {
        public readonly ParsedEnum<T> Parsed;

        public AvfxEnum( string name, string avfxName, T defaultValue, Func<ICommand> extraCommandGenerator = null ) : this( name, avfxName, extraCommandGenerator ) {
            SetValue( defaultValue );
        }

        public AvfxEnum( string name, string avfxName, Func<ICommand> extraCommandGenerator = null ) : base( avfxName ) {
            Parsed = new( name, extraCommandGenerator );
        }

        public T GetValue() => Parsed.Value;

        public void SetValue( T value ) {
            SetAssigned( true );
            Parsed.Value = value;
        }

        public override void ReadContents( BinaryReader reader, int _ ) => Parsed.Read( reader );

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
