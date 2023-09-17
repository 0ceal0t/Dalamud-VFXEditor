using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataSphereModel : AvfxData {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxEnum<GenerateMethod> GenerateMethodType = new( "Generate Method", "GeMT" );
        public readonly AvfxInt DivideX = new( "Divide X", "DivX", value: 1 );
        public readonly AvfxInt DivideY = new( "Divide Y", "DivY", value: 1 );
        public readonly AvfxCurve Radius = new( "Radius", "Rads" );
        public readonly AvfxCurve AZ = new( "Angle Z", "AnZ", CurveType.Angle );
        public readonly AvfxCurve AYR = new( "Angle Y Random", "AnYR", CurveType.Angle );
        public readonly AvfxCurve InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );

        public readonly UiDisplayList Display;

        public AvfxEmitterDataSphereModel() : base() {
            Parsed = [
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                Radius,
                AZ,
                AYR,
                InjectionSpeed,
                InjectionSpeedRandom
            ];

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( RotationOrderType );
            Display.Add( GenerateMethodType );
            Display.Add( DivideX );
            Display.Add( DivideY );
            DisplayTabs.Add( Radius );
            DisplayTabs.Add( AZ );
            DisplayTabs.Add( AYR );
            DisplayTabs.Add( InjectionSpeed );
            DisplayTabs.Add( InjectionSpeedRandom );
        }
    }
}
