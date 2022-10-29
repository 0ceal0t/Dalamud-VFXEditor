using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Binder {
    public class AVFXBinderProperty : AVFXBase {
        public readonly AVFXEnum<BindPoint> BindPointType = new( "BPT" );
        public readonly AVFXEnum<BindTargetPoint> BindTargetPointType = new( "BPTP" );
        public readonly AVFXString BinderName = new( "Name" );
        public readonly AVFXInt BindPointId = new( "BPID" );
        public readonly AVFXInt GenerateDelay = new( "GenD" );
        public readonly AVFXInt CoordUpdateFrame = new( "CoUF" );
        public readonly AVFXBool RingEnable = new( "bRng" );
        public readonly AVFXInt RingProgressTime = new( "RnPT" );
        public readonly AVFXFloat RingPositionX = new( "RnPX" );
        public readonly AVFXFloat RingPositionY = new( "RnPY" );
        public readonly AVFXFloat RingPositionZ = new( "RnPZ" );
        public readonly AVFXFloat RingRadius = new( "RnRd" );
        public readonly AVFXCurve3Axis Position = new( "Pos" );

        private readonly List<AVFXBase> Children;

        public AVFXBinderProperty( string name ) : base( name ) {
            Children = new List<AVFXBase> {
                BindPointType,
                BindTargetPointType,
                BinderName,
                BindPointId,
                GenerateDelay,
                CoordUpdateFrame,
                RingEnable,
                RingProgressTime,
                RingPositionX,
                RingPositionY,
                RingPositionZ,
                RingRadius,
                Position
            };

            BinderName.SetAssigned( false );
            BindTargetPointType.SetValue( BindTargetPoint.ByName );
            BindPointId.SetValue( 3 );
            CoordUpdateFrame.SetValue( -1 );
            RingProgressTime.SetValue( 1 );
            Position.SetAssigned( true );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
