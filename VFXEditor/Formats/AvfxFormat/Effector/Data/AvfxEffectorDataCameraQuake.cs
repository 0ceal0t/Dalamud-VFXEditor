using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxEffectorDataCameraQuake : AvfxData {
        public readonly AvfxCurve1Axis Attenuation = new( "Attenuation", "Att" );
        public readonly AvfxCurve1Axis AttenuationRandom = new( "Attenuation Random", "AttR" );
        public readonly AvfxCurve1Axis RadiusOut = new( "Radius Out", "RdO" );
        public readonly AvfxCurve1Axis RadiusOutRandom = new( "Radius Out Random", "RdOR" );
        public readonly AvfxCurve1Axis RadiusIn = new( "Radius In", "RdI" );
        public readonly AvfxCurve1Axis RadiusInRandom = new( "Radius In Random", "RdIR" );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot", CurveType.Angle );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos" );

        public AvfxEffectorDataCameraQuake() : base() {
            Parsed = [
                Attenuation,
                AttenuationRandom,
                RadiusOut,
                RadiusOutRandom,
                RadiusIn,
                RadiusInRandom,
                Rotation,
                Position
            ];

            Tabs.Add( Attenuation );
            Tabs.Add( AttenuationRandom );
            Tabs.Add( RadiusOut );
            Tabs.Add( RadiusOutRandom );
            Tabs.Add( RadiusIn );
            Tabs.Add( RadiusInRandom );
            Tabs.Add( Rotation );
            Tabs.Add( Position );
        }
    }
}
