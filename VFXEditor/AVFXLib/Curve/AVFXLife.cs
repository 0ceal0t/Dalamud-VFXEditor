using System.Collections.Generic;
using System.IO;

namespace VFXEditor.AVFXLib.Curve {
    public class AVFXLife : AVFXBase {
        public bool Enabled = true;
        // Life is kinda strange, can either be -1 (4 bytes = ffffffff) or Val + ValR + RanT

        public readonly AVFXFloat Value = new( "Val" );
        public readonly AVFXFloat ValRandom = new( "ValR" );
        public readonly AVFXEnum<RandomType> ValRandomType = new( "Type" );

        private readonly List<AVFXBase> Children;

        public AVFXLife() : base( "Life" ) {
            Children = new List<AVFXBase> {
                Value,
                ValRandom,
                ValRandomType
            };

            Value.SetValue( -1 );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            if( size > 4 ) {
                Enabled = true;
                ReadNested( reader, Children, size );
                return;
            }
            Enabled = false;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            if( !Enabled ) {
                writer.Write( -1 );
                return;
            }

            WriteNested( writer, Children );
        }
    }
}
