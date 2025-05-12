using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataConeModel : AvfxDataWithParameters {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxEnum<GenerateMethod> GenerateMethodType = new( "Generate Method", "GeMT" );
        public readonly AvfxInt DivideX = new( "Divide X", "DivX" );
        public readonly AvfxInt DivideY = new( "Divide Y", "DivY" );
        public readonly AvfxCurve1Axis AX = new( "Angle X", "AnX", CurveType.Angle );
        public readonly AvfxCurve1Axis AY = new( "Angle Y", "AnY", CurveType.Angle );
        public readonly AvfxCurve1Axis AXR = new( "Angle X Random", "AnXR", CurveType.Angle );
        public readonly AvfxCurve1Axis AYR = new( "Angle Y Random", "AnYR", CurveType.Angle );
        public readonly AvfxCurve1Axis Radius = new( "Radius", "Rad" );
        public readonly AvfxCurve1Axis InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve1Axis InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );
        public readonly AvfxCurve1Axis InjectionAngle = new( "Injection Angle", "IjA", CurveType.Angle );
        public readonly AvfxCurve1Axis InjectionAngleRandom = new( "Injection Angle Random", "IjAR", CurveType.Angle );

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
