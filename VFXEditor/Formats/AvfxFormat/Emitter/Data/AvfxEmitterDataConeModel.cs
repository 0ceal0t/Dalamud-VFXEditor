using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataConeModel : AvfxDataWithParameters {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxEnum<GenerateMethod> GenerateMethodType = new( "Generate Method", "GeMT" );
        public readonly AvfxInt DivideX = new( "Divide X", "DivX" );
        public readonly AvfxInt DivideY = new( "Divide Y", "DivY" );
        public readonly AvfxCurve AX = new( "Angle X", "AnX", CurveType.Angle );
        public readonly AvfxCurve AY = new( "Angle Y", "AnY", CurveType.Angle );
        public readonly AvfxCurve AXR = new( "Angle X Random", "AnXR", CurveType.Angle );
        public readonly AvfxCurve AYR = new( "Angle Y Random", "AnYR", CurveType.Angle );
        public readonly AvfxCurve Radius = new( "Radius", "Rad" );
        public readonly AvfxCurve InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );
        public readonly AvfxCurve InjectionAngle = new( "Injection Angle", "IjA", CurveType.Angle );
        public readonly AvfxCurve InjectionAngleRandom = new( "Injection Angle Random", "IjAR", CurveType.Angle );

        public AvfxEmitterDataConeModel() : base() {
            Parsed = [
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                AX,
                AY,
                AXR,
                AYR,
                Radius,
                InjectionSpeed,
                InjectionSpeedRandom,
                InjectionAngle,
                InjectionAngleRandom
            ];

            ParameterTab.Add( RotationOrderType );
            ParameterTab.Add( GenerateMethodType );
            ParameterTab.Add( DivideX );
            ParameterTab.Add( DivideY );

            Tabs.Add( AX );
            Tabs.Add( AY );
            Tabs.Add( AXR );
            Tabs.Add( AYR );
            Tabs.Add( Radius );
            Tabs.Add( InjectionSpeed );
            Tabs.Add( InjectionSpeedRandom );
            Tabs.Add( InjectionAngle );
            Tabs.Add( InjectionAngleRandom );
        }
    }
}
