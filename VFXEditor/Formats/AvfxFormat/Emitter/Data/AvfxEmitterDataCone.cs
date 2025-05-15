using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataCone : AvfxDataWithParameters {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxCurve1Axis AX = new( "Angle X", "AnX", CurveType.Angle );
        public readonly AvfxCurve1Axis AY = new( "Angle Y", "AnY", CurveType.Angle );
        public readonly AvfxCurve1Axis AZ = new( "Angle Z", "AnZ", CurveType.Angle );
        public readonly AvfxCurve1Axis AXR = new( "Angle X Random", "AnXR", CurveType.Angle );
        public readonly AvfxCurve1Axis AYR = new( "Angle Y Random", "AnYR", CurveType.Angle );
        public readonly AvfxCurve1Axis AZR = new( "Angle Z Random", "AnZR", CurveType.Angle );
        public readonly AvfxCurve1Axis InnerSize = new( "Inner Size", "InS" );
        public readonly AvfxCurve1Axis InnerSizeRandom = new( "Inner Size Random", "InSR" );
        public readonly AvfxCurve1Axis OuterSize = new( "Outer Size", "OuS" );
        public readonly AvfxCurve1Axis OuterSizeRandom = new( "Outer Size Random", "OuSR" );
        public readonly AvfxCurve1Axis InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve1Axis InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );
        public readonly AvfxCurve1Axis InjectionAngle = new( "Injection Angle", "IjA", CurveType.Angle );
        public readonly AvfxCurve1Axis InjectionAngleRandom = new( "Injection Angle Random", "IjAR", CurveType.Angle );

        public AvfxEmitterDataCone() : base() {
            Parsed = [
                RotationOrderType,
                AX,
                AY,
                AZ,
                AXR,
                AYR,
                AZR,
                InnerSize,
                InnerSizeRandom,
                OuterSize,
                OuterSizeRandom,
                InjectionSpeed,
                InjectionSpeedRandom,
                InjectionAngle,
                InjectionAngleRandom,
            ];

            ParameterTab.Add( RotationOrderType );

            Tabs.Add( AX );
            Tabs.Add( AY );
            Tabs.Add( AZ );
            Tabs.Add( AXR );
            Tabs.Add( AYR );
            Tabs.Add( AZR );
            Tabs.Add( InnerSize );
            Tabs.Add( InnerSizeRandom );
            Tabs.Add( OuterSize );
            Tabs.Add( OuterSizeRandom );
            Tabs.Add( InjectionSpeed );
            Tabs.Add( InjectionSpeedRandom );
            Tabs.Add( InjectionAngle );
            Tabs.Add( InjectionAngleRandom );
        }
    }
}
