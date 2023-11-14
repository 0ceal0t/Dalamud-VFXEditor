using System.Collections.Generic;
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

        public override void WriteContents( BinaryWriter writer ) => Parsed.Write( writer );

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }

        public override void Draw() {
            // Unassigned
            AssignedCopyPaste( Parsed.Name );
            if( DrawAssignButton( Parsed.Name ) ) return;

            Parsed.Draw();

            DrawUnassignPopup( Parsed.Name );
        }
    }
}
