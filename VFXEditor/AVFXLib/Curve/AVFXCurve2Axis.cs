using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Curve {
    public class AVFXCurve2Axis : AVFXBase {
        public readonly AVFXEnum<AxisConnect> AxisConnectType = new( "ACT" );
        public readonly AVFXEnum<RandomType> AxisConnectRandomType = new( "ACTR" );
        public readonly AVFXCurve X = new( "X" );
        public readonly AVFXCurve Y = new( "Y" );
        public readonly AVFXCurve RX = new( "XR" );
        public readonly AVFXCurve RY = new( "YR" );

        private readonly List<AVFXBase> Children;

        public AVFXCurve2Axis( string name ) : base( name ) {
            Children = new List<AVFXBase> {
                AxisConnectType,
                AxisConnectRandomType,
                X,
                Y,
                RX,
                RY
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
