using System.Collections.Generic;
using System.IO;

namespace VFXEditor.AVFXLib.Curve {
    public class AVFXCurveColor : AVFXBase {
        public readonly AVFXCurve RGB = new( "RGB" );
        public readonly AVFXCurve A = new( "A" );
        public readonly AVFXCurve SclR = new( "SclR" );
        public readonly AVFXCurve SclG = new( "SclG" );
        public readonly AVFXCurve SclB = new( "SclB" );
        public readonly AVFXCurve SclA = new( "SclA" );
        public readonly AVFXCurve Bri = new( "Bri" );
        public readonly AVFXCurve RanR = new( "RanR" );
        public readonly AVFXCurve RanG = new( "RanG" );
        public readonly AVFXCurve RanB = new( "RanB" );
        public readonly AVFXCurve RanA = new( "RanA" );
        public readonly AVFXCurve RBri = new( "RBri" );

        private readonly List<AVFXBase> Children;

        public AVFXCurveColor( string name = "Col" ) : base( name ) {
            Children = new List<AVFXBase> {
                RGB,
                A,
                SclR,
                SclG,
                SclB,
                SclA,
                Bri,
                RanR,
                RanG,
                RanB,
                RanA,
                RBri
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
