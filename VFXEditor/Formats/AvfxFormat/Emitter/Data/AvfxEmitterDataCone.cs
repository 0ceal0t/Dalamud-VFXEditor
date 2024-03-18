using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataCone : AvfxData {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxCurve AX = new( "Angle X", "AnX", CurveType.Angle );
        public readonly AvfxCurve AY = new( "Angle Y", "AnY", CurveType.Angle );
        public readonly AvfxCurve InnerSize = new( "Inner Size", "InS" );
        public readonly AvfxCurve OuterSize = new( "Outer Size", "OuS" );
        public readonly AvfxCurve InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );
        public readonly AvfxCurve InjectionAngle = new( "Injection Angle", "IjA", CurveType.Angle );

        public readonly UiDisplayList Display;

        public AvfxEmitterDataCone() : base() {
            Parsed = [
                RotationOrderType,
                AX,
                AY,
                InnerSize,
                OuterSize,
                InjectionSpeed,
                InjectionSpeedRandom,
                InjectionAngle
            ];

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( RotationOrderType );
            DisplayTabs.Add( AX );
            DisplayTabs.Add( AY );
            DisplayTabs.Add( InnerSize );
            DisplayTabs.Add( OuterSize );
            DisplayTabs.Add( InjectionSpeed );
            DisplayTabs.Add( InjectionSpeedRandom );
            DisplayTabs.Add( InjectionAngle );
        }
    }
}
