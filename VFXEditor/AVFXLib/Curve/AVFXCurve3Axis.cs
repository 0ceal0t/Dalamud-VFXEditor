using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.AVFXLib.Curve {
    public class AVFXCurve3Axis : AVFXBase {
        public readonly AVFXEnum<AxisConnect> AxisConnectType = new( "ACT" );
        public readonly AVFXEnum<RandomType> AxisConnectRandomType = new( "ACTR" );
        public readonly AVFXCurve X = new( "X" );
        public readonly AVFXCurve Y = new( "Y" );
        public readonly AVFXCurve Z = new( "Z" );
        public readonly AVFXCurve RX = new( "XR" );
        public readonly AVFXCurve RY = new( "YR" );
        public readonly AVFXCurve RZ = new( "ZR" );

        private readonly List<AVFXBase> Children;

        public AVFXCurve3Axis( string name ) : base( name ) {
            Children = new List<AVFXBase> {
                AxisConnectType,
                AxisConnectRandomType,
                X,
                Y,
                Z,
                RX,
                RY,
                RZ
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
