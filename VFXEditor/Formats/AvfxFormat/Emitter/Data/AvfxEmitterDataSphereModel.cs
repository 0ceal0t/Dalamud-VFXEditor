using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataSphereModel : AvfxData {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxEnum<GenerateMethod> GenerateMethodType = new( "Generate Method", "GeMT" );
        public readonly AvfxInt DivideX = new( "Divide X", "DivX" );
        public readonly AvfxInt DivideY = new( "Divide Y", "DivY" );
        public readonly AvfxCurve Radius = new( "Radius", "Rads" );
        public readonly AvfxCurve AZ = new( "Angle Z", "AnZ", CurveType.Angle );
        public readonly AvfxCurve AYR = new( "Angle Y Random", "AnYR", CurveType.Angle );
        public readonly AvfxCurve InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );

        public readonly UiDisplayList Display;

        public AvfxEmitterDataSphereModel() : base() {
            Parsed = new() {
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                Radius,
                AZ,
                AYR,
                InjectionSpeed,
                InjectionSpeedRandom
            };
            DivideX.SetValue( 1 );
            DivideY.SetValue( 1 );

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
