using System.IO;
using VfxEditor.AvfxFormat;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.AvfxFormat.Components {
    public abstract class AvfxLiteral<T, S> : AvfxDrawable where T : ParsedSimpleBase<S> {
        public readonly T Parsed;

        public AvfxLiteral( string avfxName, T parsed ) : base( avfxName ) {
            Parsed = parsed;
        }

        public S Value {
            get => Parsed.Value;
            set {
                SetAssigned( true );
                Parsed.Value = value;
            }
        }

        public override void ReadContents( BinaryReader reader, int size ) => Parsed.Read( reader, size );

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        public override void WriteContents( BinaryWriter writer ) => Parsed.Write( writer );

        public override void Draw() {
            // Unassigned
            AssignedCopyPaste( this, Parsed.Name );
            if( DrawAddButton( this, Parsed.Name ) ) return;

            Parsed.Draw( CommandManager.Avfx );

            DrawRemoveContextMenu( this, Parsed.Name );
        }
    }
}
